using Bromine.Playwright.Extensions.Builders;
using Bromine.Playwright.Extensions.Configuration;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests;

#nullable  disable

/// <summary>
/// Base fixture that launches a fresh Playwright page per test.
/// <para>
/// It is parameterized per browser engine, so every derived fixture is discovered
/// three times — once for Chromium, Firefox and WebKit. In the IDE's Unit Tests /
/// Test Explorer window each engine shows up as its own node, e.g.
/// <c>PageShouldTests(Chromium)</c>, that you can run or debug independently.
/// Each variant is also tagged with a category (<c>Chromium</c> | <c>Firefox</c> |
/// <c>Webkit</c>) so you can filter from the CLI, e.g.
/// <c>dotnet test --filter "TestCategory=Webkit"</c>.
/// </para>
/// <para>
/// Runs headless by default. See <see cref="LocalTestSettings"/> to run headed, add slow
/// motion, or narrow which engines execute — all via a gitignored local file or env vars.
/// </para>
/// </summary>
[TestFixture(BrowserType.Chromium, Category = "Chromium")]
[TestFixture(BrowserType.Firefox, Category = "Firefox")]
[TestFixture(BrowserType.Webkit, Category = "Webkit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Children)]
public abstract class TestBase
{
    private readonly BrowserType _browser;

    /// <summary>
    /// NUnit constructs one fixture instance per <see cref="BrowserType"/> value above.
    /// Derived fixtures forward the engine through a pass-through constructor.
    /// </summary>
    protected TestBase(BrowserType browser) => _browser = browser;

    protected IPage Page { get; private set; }
    protected IBrowser Browser;
    protected IBrowserContext Context { get; private set; }

    [SetUp]
    public async Task SetUp()
    {
        PlaywrightDefaults.Reset();

        // Assertions run against a static local page, so they resolve in milliseconds when they
        // are going to pass. The full 5s default is only ever spent by assertions expected to
        // fail, of which this suite has ~32 — shortening the window is most of their cost.
        PlaywrightDefaults.AssertionTimeout = 2_000;

        var settings = LocalTestSettings.Current;

        if (!settings.ShouldRun(_browser))
            Assert.Ignore($"{_browser} is not in the enabled engines ({settings.EnabledEngines}).");

        PlaywrightBrowserResult result;
        try
        {
            // Shared per engine for the whole run; see SharedBrowsers for why.
            result = await SharedBrowsers.GetAsync(_browser);
        }
        catch (PlaywrightException ex) when (ex.Message.Contains("Executable doesn't exist"))
        {
            // Browser engine isn't installed on this machine (common on CI where only
            // Chromium is downloaded). Skip rather than fail so `dotnet test` stays green.
            Assert.Ignore(
                $"{_browser} is not installed. Install it with: " +
                $"pwsh tests/bin/Debug/net8.0/playwright.ps1 install {_browser.ToString().ToLowerInvariant()}");
            return; // unreachable: Assert.Ignore throws
        }

        // A fresh context per test is the real isolation boundary: its own cookies, storage
        // and cache. Costs milliseconds, unlike a browser launch.
        Browser = result.Browser;
        Context = await BrowserContextBuilder.For(result.Browser).WithBaseUrl(TestServerFixture.BaseUrl).BuildAsync();
        Page = await Context.NewPageAsync();

        await Page.GotoAsync("/");
    }


    [TearDown]
    public async Task TearDown()
    {
        if (Page != null)
            await Page.CloseAsync();
        if (Context != null)
            await Context.CloseAsync();

        // Browser is shared for the whole run — SharedBrowsersFixture disposes it (and the
        // IPlaywright driver) once at assembly teardown.
    }
}

/// <summary>
/// Playwright browser engines a <see cref="TestBase"/> fixture can target.
/// </summary>
public enum BrowserType
{
    Chromium,
    Firefox,
    Webkit
}
