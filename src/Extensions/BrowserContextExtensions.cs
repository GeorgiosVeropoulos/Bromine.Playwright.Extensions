using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Extensions;

/// <summary>
/// Extension methods for <see cref="IBrowserContext"/> providing storage-state and
/// lifetime helpers.
/// </summary>
public static class BrowserContextExtensions
{
    /// <summary>
    /// True while the context is still usable — the inverse of
    /// <see cref="IBrowserContext.IsClosed"/>. Requires Playwright 1.59 or newer.
    /// </summary>
    public static bool IsOpen(this IBrowserContext context) => !context.IsClosed;

    /// <summary>
    /// Replace the context's cookies, localStorage and IndexedDB with the state in
    /// <paramref name="storageStatePath"/>, without building a new context.
    /// <para>
    /// The point is switching identity mid-test — sign in as one user, swap to another — which
    /// previously meant discarding the context and its pages. Existing state is cleared first.
    /// Requires Playwright 1.59 or newer.
    /// </para>
    /// </summary>
    /// <exception cref="InvalidOperationException">The context is closing or already closed.</exception>
    /// <exception cref="FileNotFoundException">No storage state file at the given path.</exception>
    public static async Task SwitchStorageStateAsync(
        this IBrowserContext context,
        string storageStatePath)
    {
        // Both guards front-run failures that Playwright would otherwise surface from deep
        // inside the driver, where the message says nothing about which call was at fault.
        if (context.IsClosed)
        {
            throw new InvalidOperationException(
                "Cannot switch storage state: the browser context is closing or already closed.");
        }

        if (!File.Exists(storageStatePath))
        {
            throw new FileNotFoundException(
                $"Storage state file not found: {storageStatePath}", storageStatePath);
        }

        await context.SetStorageStateAsync(storageStatePath);
    }

    /// <summary>
    /// Start recording network traffic to a HAR file at <paramref name="harPath"/>, creating the
    /// directory if needed. Stop it with <see cref="StopHarRecordingAsync"/>.
    /// <para>
    /// Complements <c>BrowserContextBuilder.WithHarRecording</c>, which can only be set when the
    /// context is created and therefore captures the whole session. This starts and stops mid-test,
    /// so a HAR can cover just the step under investigation. Requires Playwright 1.60 or newer.
    /// </para>
    /// <para>
    /// Playwright 1.60 returns a no-op disposable from the underlying call, so <c>await using</c>
    /// would silently never write the file — call the stop method.
    /// </para>
    /// </summary>
    public static async Task StartHarRecordingAsync(
        this IBrowserContext context,
        string harPath,
        HarContentPolicy? content = null,
        HarMode? mode = null,
        string? urlFilter = null)
    {
        var directory = Path.GetDirectoryName(harPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        _ = await context.Tracing.StartHarAsync(harPath, new TracingStartHarOptions
        {
            Content = content,
            Mode = mode,
            UrlFilter = urlFilter
        });
    }

    /// <summary>
    /// Stop HAR recording and write the file to the path it was started with.
    /// </summary>
    public static Task StopHarRecordingAsync(this IBrowserContext context)
        => context.Tracing.StopHarAsync();

    /// <summary>
    /// Record the network traffic of <paramref name="action"/> to a HAR file and return its path.
    /// The recording is stopped even if the action throws.
    /// </summary>
    public static async Task<string> RecordHarAsync(
        this IBrowserContext context,
        string harPath,
        Func<Task> action,
        HarContentPolicy? content = null,
        HarMode? mode = null,
        string? urlFilter = null)
    {
        await context.StartHarRecordingAsync(harPath, content, mode, urlFilter);
        try
        {
            await action();
        }
        finally
        {
            await context.StopHarRecordingAsync();
        }

        return harPath;
    }
}
