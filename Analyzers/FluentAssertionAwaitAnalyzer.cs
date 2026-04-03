using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Bromine.Playwright.Extensions.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FluentAssertionAwaitAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "BROE101";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Fluent assertion chain is not awaited",
        messageFormat: "This fluent assertion chain is not awaited and will never execute. Add 'await' before the expression.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
            "Fluent assertion chains built with .Should() must be awaited to execute. " +
            "Without 'await', the assertions are silently skipped.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeExpressionStatement, SyntaxKind.ExpressionStatement);
    }

    private static void AnalyzeExpressionStatement(SyntaxNodeAnalysisContext context)
    {
        var expressionStatement = (ExpressionStatementSyntax)context.Node;
        var expression = expressionStatement.Expression;

        // If it's already an await expression, it's fine
        if (expression is AwaitExpressionSyntax)
            return;

        // We're looking for invocations like: locator.Should().BeVisibleAsync()
        // or member access chains that end in an invocation
        if (expression is not InvocationExpressionSyntax invocation)
            return;

        // Check if the return type derives from FluentBase<T>
        var typeInfo = context.SemanticModel.GetTypeInfo(invocation, context.CancellationToken);
        var returnType = typeInfo.Type;

        if (returnType == null)
            return;

        if (IsFluentBaseType(returnType))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(Rule, expressionStatement.GetLocation()));
        }
    }

    private static bool IsFluentBaseType(ITypeSymbol? type)
    {
        while (type != null)
        {
            // Check by name + namespace to avoid needing a direct assembly reference
            if (type is INamedTypeSymbol named &&
                named.IsGenericType &&
                named.Name == "FluentBase" &&
                GetFullNamespace(named) == "Bromine.Playwright.Extensions.Extensions")
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    private static string GetFullNamespace(ITypeSymbol type)
    {
        var ns = type.ContainingNamespace;
        if (ns == null || ns.IsGlobalNamespace)
            return string.Empty;

        var parts = new System.Collections.Generic.List<string>();
        while (ns != null && !ns.IsGlobalNamespace)
        {
            parts.Insert(0, ns.Name);
            ns = ns.ContainingNamespace;
        }

        return string.Join(".", parts);
    }
}

