using System.Runtime.CompilerServices;
using Bromine.Playwright.Extensions.Reason;
using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Extensions;


/// <summary>
///  Base class for fluent assertion builders, allowing chaining of multiple assertions with "because" messages and proper exception handling.
///  Methods can be chained and will be executed in sequence when the final assertion is awaited.
/// If any assertion fails, the provided "because" message will be included in the exception for better context.
/// </summary>
public class FluentBase<TSelf> : ICriticalNotifyCompletion where TSelf : FluentBase<TSelf>
{
    private readonly List<(Func<Task> Step, Because because)> _steps = new();
    private Task? _runTask;
    protected bool NegateNext = false;

    protected void AddStep(Func<Task> step, Because because)
    {
        NegateNext = false;
        _steps.Add((step, because));
    }
    
    public TSelf Not
    {
        get
        {
            NegateNext = !NegateNext;
            return (TSelf) this;
        }
    }

    private Task Run() => _runTask ??= RunCore();
    /// <summary>
    /// Runs the chained tasks sequentially, catching any PlaywrightExceptions and rethrowing them with the appropriate "because" message if provided.
    /// </summary>
    /// <exception cref="PlaywrightException"></exception>
    private async Task RunCore()
    {
        foreach (var (step, because) in _steps)
        {
            try
            {
                await step().ConfigureAwait(false);
            }
            catch (PlaywrightException e)
            {
                if (string.IsNullOrWhiteSpace(because.Message))
                    throw;

                var formattedBecause = because.Args.Length > 0 
                    ? string.Format(because.Message, because.Args) 
                    : because.Message;
                throw new PlaywrightException($"{formattedBecause}\n{e.Message}", e);
            }
        }
    }
    
    protected ILocatorAssertions Expect(ILocator locator) => Microsoft.Playwright.Assertions.Expect(locator);
    protected IPageAssertions Expect(IPage page) => Microsoft.Playwright.Assertions.Expect(page);
    protected IAPIResponseAssertions Expect(IAPIResponse response) => Microsoft.Playwright.Assertions.Expect(response);
    
    
    public FluentBase<TSelf> GetAwaiter() => this;
    public bool IsCompleted => Run().IsCompleted;
    public void OnCompleted(Action continuation) => Run().GetAwaiter().OnCompleted(continuation);
    public void UnsafeOnCompleted(Action continuation) => Run().GetAwaiter().UnsafeOnCompleted(continuation);

    public void GetResult() => Run().GetAwaiter().GetResult();
}
