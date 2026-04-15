using Bromine.Playwright.Extensions.Builders;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Builders;

#nullable disable

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Children)]
public class PlaywrightBrowserBuilderTests
{
    private PlaywrightBrowserResult _result;

    [TearDown]
    public async Task TearDown()
    {
        if (_result != null)
            await _result.DisposeAsync();
    }

    // ───────────────────────── Create / BuildAsync ─────────────────────────

    [Test]
    public async Task Create_BuildAsync_ShouldReturnValidBrowserAndPlaywright()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithChromium()
            .Headless()
            .BuildAsync();

        Assert.That(_result.Playwright, Is.Not.Null);
        Assert.That(_result.Browser, Is.Not.Null);
        Assert.That(_result.Browser.IsConnected, Is.True);
    }

    [Test]
    public async Task Deconstruct_ShouldReturnPlaywrightAndBrowser()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithChromium()
            .Headless()
            .BuildAsync();

        var (playwright, browser) = _result;

        Assert.That(playwright, Is.Not.Null);
        Assert.That(browser, Is.Not.Null);
        Assert.That(browser.IsConnected, Is.True);
    }

    // ───────────────────────── Browser engines ─────────────────────────

    [Test]
    public async Task WithChromium_ShouldLaunchChromiumBrowser()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithChromium()
            .Headless()
            .BuildAsync();

        Assert.That(_result.Browser.BrowserType.Name, Is.EqualTo("chromium"));
    }

    [Test]
    public async Task WithFirefox_ShouldLaunchFirefoxBrowser()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithFirefox()
            .Headless()
            .BuildAsync();

        Assert.That(_result.Browser.BrowserType.Name, Is.EqualTo("firefox"));
    }

    [Test]
    public async Task WithWebkit_ShouldLaunchWebkitBrowser()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithWebkit()
            .Headless()
            .BuildAsync();

        Assert.That(_result.Browser.BrowserType.Name, Is.EqualTo("webkit"));
    }

    // ───────────────────────── Headless / Headed ─────────────────────────

    [Test]
    public async Task Headless_ShouldLaunchSuccessfully()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithChromium()
            .Headless()
            .BuildAsync();

        Assert.That(_result.Browser.IsConnected, Is.True);
    }

    [Test]
    public async Task Headless_False_ShouldBeSameAsHeaded()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithChromium()
            .Headless(false)
            .BuildAsync();

        Assert.That(_result.Browser.IsConnected, Is.True);
    }

    // ───────────────────────── Fluent chaining returns same builder ─────────────────────────

    [Test]
    public void FluentMethods_ShouldReturnSameBuilderInstance()
    {
        var builder = PlaywrightBrowserBuilder.Create();

        Assert.That(builder.WithChromium(), Is.SameAs(builder));
        Assert.That(builder.WithFirefox(), Is.SameAs(builder));
        Assert.That(builder.WithWebkit(), Is.SameAs(builder));
        Assert.That(builder.Headless(), Is.SameAs(builder));
        Assert.That(builder.Headed(), Is.SameAs(builder));
        Assert.That(builder.WithSlowMotion(50), Is.SameAs(builder));
        Assert.That(builder.WithTimeout(10_000), Is.SameAs(builder));
        Assert.That(builder.WithChannel("chrome"), Is.SameAs(builder));
        Assert.That(builder.WithExecutablePath("/usr/bin/chromium"), Is.SameAs(builder));
        Assert.That(builder.WithArgs("--no-sandbox"), Is.SameAs(builder));
        Assert.That(builder.WithDownloadsPath("/tmp"), Is.SameAs(builder));
        Assert.That(builder.WithChromiumSandbox(false), Is.SameAs(builder));
        Assert.That(builder.WithProxy("http://proxy:8080"), Is.SameAs(builder));
        Assert.That(builder.WithEnvironment(new Dictionary<string, string> { ["KEY"] = "VALUE" }), Is.SameAs(builder));
        Assert.That(builder.WithTracesDir("/tmp/traces"), Is.SameAs(builder));
        Assert.That(builder.HandleSigint(), Is.SameAs(builder));
        Assert.That(builder.HandleSigterm(), Is.SameAs(builder));
        Assert.That(builder.HandleSighup(), Is.SameAs(builder));
    }

    // ───────────────────────── Last browser type wins ─────────────────────────

    [Test]
    public async Task LastBrowserTypeWins_ShouldUseFirefox()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithChromium()
            .WithWebkit()
            .WithFirefox()
            .Headless()
            .BuildAsync();

        Assert.That(_result.Browser.BrowserType.Name, Is.EqualTo("firefox"));
    }

    // ───────────────────────── WithSlowMotion ─────────────────────────

    [Test]
    public async Task WithSlowMotion_ShouldLaunchSuccessfully()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithChromium()
            .Headless()
            .WithSlowMotion(10)
            .BuildAsync();

        Assert.That(_result.Browser.IsConnected, Is.True);
    }

    // ───────────────────────── WithTimeout ─────────────────────────

    [Test]
    public async Task WithTimeout_ShouldLaunchSuccessfully()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithChromium()
            .Headless()
            .WithTimeout(60_000)
            .BuildAsync();

        Assert.That(_result.Browser.IsConnected, Is.True);
    }

    // ───────────────────────── WithArgs ─────────────────────────

    [Test]
    public async Task WithArgs_ShouldLaunchSuccessfully()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithChromium()
            .Headless()
            .WithArgs("--disable-gpu")
            .BuildAsync();

        Assert.That(_result.Browser.IsConnected, Is.True);
    }

    // ───────────────────────── WithDownloadsPath ─────────────────────────

    [Test]
    public async Task WithDownloadsPath_ShouldLaunchSuccessfully()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"pw-downloads-{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        try
        {
            _result = await PlaywrightBrowserBuilder.Create()
                .WithChromium()
                .Headless()
                .WithDownloadsPath(tempDir)
                .BuildAsync();

            Assert.That(_result.Browser.IsConnected, Is.True);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    // ───────────────────────── WithChromiumSandbox ─────────────────────────

    [Test]
    public async Task WithChromiumSandbox_False_ShouldLaunchSuccessfully()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithChromium()
            .Headless()
            .WithChromiumSandbox(false)
            .BuildAsync();

        Assert.That(_result.Browser.IsConnected, Is.True);
    }

    // ───────────────────────── DisposeAsync ─────────────────────────

    [Test]
    public async Task DisposeAsync_ShouldCloseBrowser()
    {
        var result = await PlaywrightBrowserBuilder.Create()
            .WithChromium()
            .Headless()
            .BuildAsync();

        await result.DisposeAsync();

        Assert.That(result.Browser.IsConnected, Is.False);
        // Prevent TearDown from double-disposing
        _result = null;
    }

    // ───────────────────────── Full configuration chain ─────────────────────────

    [Test]
    public async Task FullConfigurationChain_ShouldLaunchSuccessfully()
    {
        _result = await PlaywrightBrowserBuilder.Create()
            .WithChromium()
            .Headless()
            .WithSlowMotion(10)
            .WithTimeout(30_000)
            .WithArgs("--disable-gpu")
            .WithChromiumSandbox(false)
            .HandleSigint(false)
            .HandleSigterm(false)
            .HandleSighup(false)
            .BuildAsync();

        Assert.That(_result.Browser.IsConnected, Is.True);
        Assert.That(_result.Browser.BrowserType.Name, Is.EqualTo("chromium"));
    }
}

