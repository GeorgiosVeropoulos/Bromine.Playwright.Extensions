using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Bromine.Playwright.Extensions.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FluentAssertionAwaitAnalyzer : DiagnosticAnalyzer
{
    public const string UnawaitedDiagnosticId = "BROE001";
    public const string AssignmentDiagnosticId = "BROW201";

    private static readonly DiagnosticDescriptor UnawaitedRule = new(
        id: UnawaitedDiagnosticId,
        title: "Fluent assertion chain is not awaited",
        messageFormat: "This fluent assertion chain is not awaited and will never execute. Add 'await' before the expression.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
            "Fluent assertion chains built with .Should() must be awaited to execute. " +
            "Without 'await', the assertions are silently skipped.");

    private static readonly DiagnosticDescriptor AssignmentRule = new(
        id: AssignmentDiagnosticId,
        title: "Fluent assertion builder assigned to a variable",
        messageFormat: "Do not assign fluent assertion chains to variables. Build and await the chain directly on a single line to prevent execution bugs.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning, // Set as a Warning per your request
        isEnabledByDefault: true,
        description:
            "Assigning a fluent assertion builder to a variable can lead to caching bugs if awaited multiple times. " +
            "Always chain and await the assertion inline.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(UnawaitedRule, AssignmentRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        // Check for missing awaits
        context.RegisterSyntaxNodeAction(AnalyzeExpressionStatement, SyntaxKind.ExpressionStatement);
        
        // Check for variable assignments (var assertions = locator.Should();)
        context.RegisterSyntaxNodeAction(AnalyzeLocalDeclaration, SyntaxKind.LocalDeclarationStatement);
        
        // Check for existing variable assignments (assertions = locator.Should();)
        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
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
        if (IsFluentBaseType(typeInfo.Type))
        {
            context.ReportDiagnostic(Diagnostic.Create(UnawaitedRule, expressionStatement.GetLocation()));
        }
    }

    private static void AnalyzeLocalDeclaration(SyntaxNodeAnalysisContext context)
    {
        var declaration = (LocalDeclarationStatementSyntax)context.Node;

        foreach (var variable in declaration.Declaration.Variables)
        {
            if (variable.Initializer?.Value is { } initializerValue)
            {
                var typeInfo = context.SemanticModel.GetTypeInfo(initializerValue, context.CancellationToken);
                if (IsFluentBaseType(typeInfo.Type))
                {
                    context.ReportDiagnostic(Diagnostic.Create(AssignmentRule, variable.GetLocation()));
                }
            }
        }
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;
        
        var typeInfo = context.SemanticModel.GetTypeInfo(assignment.Right, context.CancellationToken);
        if (IsFluentBaseType(typeInfo.Type))
        {
            context.ReportDiagnostic(Diagnostic.Create(AssignmentRule, assignment.GetLocation()));
        }
    }

    private static bool IsFluentBaseType(ITypeSymbol? type)
    {
        while (type != null)
        {
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