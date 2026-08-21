using Bromine.Playwright.Extensions.Configuration;
using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Extensions;

/// <summary>
/// Extension methods for <see cref="IRequest"/> built on the non-blocking
/// <see cref="IRequest.ExistingResponse"/> introduced in Playwright 1.59.
/// </summary>
public static class RequestExtensions
{
    /// <summary>
    /// True if the response for this request has already arrived.
    /// <para>
    /// Returns immediately — unlike <c>ResponseAsync()</c> it never waits, so it is safe to call
    /// from inside a route handler or a tight loop over many requests.
    /// </para>
    /// </summary>
    public static bool HasResponse(this IRequest request) => request.ExistingResponse is not null;

    /// <summary>
    /// Get the response for this request, bounded by a timeout.
    /// <para>
    /// Returns the already-received response with no await when there is one, so the common case
    /// costs nothing. Returns <c>null</c> if the response has not arrived within
    /// <paramref name="timeoutMs"/> (defaults to <see cref="PlaywrightDefaults.ActionTimeout"/>),
    /// rather than hanging as a bare <c>ResponseAsync()</c> would.
    /// </para>
    /// </summary>
    public static async Task<IResponse?> GetResponseAsync(
        this IRequest request,
        float? timeoutMs = null)
    {
        if (request.ExistingResponse is { } existing)
            return existing;

        var timeout = TimeSpan.FromMilliseconds(timeoutMs ?? PlaywrightDefaults.ActionTimeout);

        using var cancellation = new CancellationTokenSource();
        var responseTask = request.ResponseAsync();
        var timeoutTask = Task.Delay(timeout, cancellation.Token);

        var finished = await Task.WhenAny(responseTask, timeoutTask);
        if (finished != responseTask)
            return null;

        // Stop the timer task so it does not sit on the thread pool until the delay elapses.
        cancellation.Cancel();
        return await responseTask;
    }
}
