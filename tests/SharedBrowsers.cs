using System.Collections.Concurrent;
using Bromine.Playwright.Extensions.Builders;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests;

/// <summary>
/// One Playwright browser per engine, shared for the whole test run.
/// <para>
/// Launching a browser costs roughly a second; creating a <c>BrowserContext</c> costs a few
/// milliseconds and is the isolation boundary Playwright actually recommends — a fresh context
/// has its own cookies, storage and cache. So tests share the engine process and each still
/// gets a clean context and page, which keeps the isolation while removing one browser launch
/// per test.
/// </para>
/// </summary>
internal static class SharedBrowsers
{
    private static readonly ConcurrentDictionary<BrowserType, Lazy<Task<PlaywrightBrowserResult>>> Pool = new();

    /// <summary>
    /// Returns the shared browser for the engine, launching it on first use.
    /// A failed launch is cached, so a missing engine fails fast for every test that wants it
    /// instead of retrying the launch once per test.
    /// </summary>
    public static Task<PlaywrightBrowserResult> GetAsync(BrowserType engine) =>
        Pool.GetOrAdd(engine,
                e => new Lazy<Task<PlaywrightBrowserResult>>(() => LaunchAsync(e)))
            .Value;

    private static async Task<PlaywrightBrowserResult> LaunchAsync(BrowserType engine)
    {
        var settings = LocalTestSettings.Current;
        var builder = PlaywrightBrowserBuilder.Create();

        builder = engine switch
        {
            BrowserType.Firefox => builder.WithFirefox(),
            BrowserType.Webkit => builder.WithWebkit(),
            _ => builder.WithChromium()
        };

        if (settings.Headed)
            builder.Headed();

        if (settings.SlowMoMs > 0)
            builder.WithSlowMotion(settings.SlowMoMs);

        return await builder.BuildAsync();
    }

    /// <summary>
    /// Closes every launched browser and disposes its IPlaywright driver process.
    /// </summary>
    public static async Task DisposeAllAsync()
    {
        foreach (var entry in Pool.Values)
        {
            if (!entry.IsValueCreated)
                continue;

            try
            {
                var result = await entry.Value;
                await result.DisposeAsync();
            }
            catch
            {
                // Best effort: a browser that never launched (or already died) must not
                // fail the run during teardown.
            }
        }

        Pool.Clear();
    }
}

/// <summary>
/// Assembly-level teardown for <see cref="SharedBrowsers"/>.
/// </summary>
[SetUpFixture]
public class SharedBrowsersFixture
{
    [OneTimeTearDown]
    public Task OneTimeTearDown() => SharedBrowsers.DisposeAllAsync();
}
