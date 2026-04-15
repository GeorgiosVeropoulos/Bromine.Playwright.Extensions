using Bromine.Playwright.Extensions.Builders;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Builders;

#nullable disable

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Children)]
public class BrowserContextBuilderTests
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;

    [SetUp]
    public async Task SetUp()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_context != null)
            await _context.CloseAsync();
        if (_browser != null)
            await _browser.CloseAsync();
        _playwright?.Dispose();
    }

    // ───────────────────────── For / BuildAsync ─────────────────────────

    [Test]
    public async Task For_BuildAsync_ShouldCreateContext()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .BuildAsync();

        Assert.That(_context, Is.Not.Null);
        var page = await _context.NewPageAsync();
        Assert.That(page, Is.Not.Null);
        await page.CloseAsync();
    }

    // ───────────────────────── Fluent chaining ─────────────────────────

    [Test]
    public void FluentMethods_ShouldReturnSameBuilderInstance()
    {
        var builder = BrowserContextBuilder.For(_browser);

        Assert.That(builder.WithViewport(1920, 1080), Is.SameAs(builder));
        Assert.That(builder.WithNoViewport(), Is.SameAs(builder));
        Assert.That(builder.BypassCSP(), Is.SameAs(builder));
        Assert.That(builder.AsMobile(), Is.SameAs(builder));
        Assert.That(builder.WithTouch(), Is.SameAs(builder));
        Assert.That(builder.WithLocale("en-US"), Is.SameAs(builder));
        Assert.That(builder.WithTimezone("America/New_York"), Is.SameAs(builder));
        Assert.That(builder.WithColorScheme(ColorScheme.Dark), Is.SameAs(builder));
        Assert.That(builder.WithPermissions("geolocation"), Is.SameAs(builder));
        Assert.That(builder.WithHttpCredentials("user", "pass"), Is.SameAs(builder));
        Assert.That(builder.WithGeolocation(37.7749f, -122.4194f), Is.SameAs(builder));
        Assert.That(builder.WithJavaScript(), Is.SameAs(builder));
        Assert.That(builder.AcceptDownloads(), Is.SameAs(builder));
        Assert.That(builder.WithUserAgent("TestAgent/1.0"), Is.SameAs(builder));
        Assert.That(builder.WithDeviceScaleFactor(2), Is.SameAs(builder));
        Assert.That(builder.WithProxy("http://proxy:8080"), Is.SameAs(builder));
        Assert.That(builder.WithBaseUrl("http://localhost"), Is.SameAs(builder));
        Assert.That(builder.Offline(), Is.SameAs(builder));
        Assert.That(builder.WithDefaultNavigationTimeout(30_000), Is.SameAs(builder));
        Assert.That(builder.WithDefaultTimeout(10_000), Is.SameAs(builder));
        Assert.That(builder.WithHarRecording("/tmp/har.har"), Is.SameAs(builder));
        Assert.That(builder.WithVideoRecording("/tmp/videos"), Is.SameAs(builder));
        Assert.That(builder.WithTracing(), Is.SameAs(builder));
    }

    // ───────────────────────── WithViewport ─────────────────────────

    [Test]
    public async Task WithViewport_ShouldSetViewportSize()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithViewport(800, 600)
            .BuildAsync();

        var page = await _context.NewPageAsync();
        var size = page.ViewportSize;

        Assert.That(size!.Width, Is.EqualTo(800));
        Assert.That(size.Height, Is.EqualTo(600));
        await page.CloseAsync();
    }

    [Test]
    public async Task WithViewport_CustomResolution_ShouldApply()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithViewport(1920, 1080)
            .BuildAsync();

        var page = await _context.NewPageAsync();
        var size = page.ViewportSize;

        Assert.That(size!.Width, Is.EqualTo(1920));
        Assert.That(size.Height, Is.EqualTo(1080));
        await page.CloseAsync();
    }

    // ───────────────────────── WithLocale ─────────────────────────

    [Test]
    public async Task WithLocale_ShouldApplyLocale()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithLocale("de-DE")
            .BuildAsync();

        var page = await _context.NewPageAsync();
        var locale = await page.EvaluateAsync<string>("() => navigator.language");

        Assert.That(locale, Is.EqualTo("de-DE"));
        await page.CloseAsync();
    }

    // ───────────────────────── WithTimezone ─────────────────────────

    [Test]
    public async Task WithTimezone_ShouldApplyTimezone()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithTimezone("America/New_York")
            .BuildAsync();

        var page = await _context.NewPageAsync();
        var tz = await page.EvaluateAsync<string>("() => Intl.DateTimeFormat().resolvedOptions().timeZone");

        Assert.That(tz, Is.EqualTo("America/New_York"));
        await page.CloseAsync();
    }

    // ───────────────────────── WithColorScheme ─────────────────────────

    [Test]
    public async Task WithColorScheme_Dark_ShouldApply()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithColorScheme(ColorScheme.Dark)
            .BuildAsync();

        var page = await _context.NewPageAsync();
        var isDark = await page.EvaluateAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches");

        Assert.That(isDark, Is.True);
        await page.CloseAsync();
    }

    [Test]
    public async Task WithColorScheme_Light_ShouldApply()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithColorScheme(ColorScheme.Light)
            .BuildAsync();

        var page = await _context.NewPageAsync();
        var isLight = await page.EvaluateAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches");

        Assert.That(isLight, Is.True);
        await page.CloseAsync();
    }

    // ───────────────────────── WithUserAgent ─────────────────────────

    [Test]
    public async Task WithUserAgent_ShouldApplyCustomUserAgent()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithUserAgent("BromineTestBot/1.0")
            .BuildAsync();

        var page = await _context.NewPageAsync();
        var ua = await page.EvaluateAsync<string>("() => navigator.userAgent");

        Assert.That(ua, Is.EqualTo("BromineTestBot/1.0"));
        await page.CloseAsync();
    }

    // ───────────────────────── WithGeolocation ─────────────────────────

    [Test]
    public async Task WithGeolocation_ShouldApplyCoordinates()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithBaseUrl(TestServerFixture.BaseUrl)
            .WithPermissions("geolocation")
            .WithGeolocation(48.8566f, 2.3522f)
            .BuildAsync();

        var page = await _context.NewPageAsync();
        await page.GotoAsync("/index.html");

        var position = await page.EvaluateAsync<double[]>("""
            () => new Promise((resolve, reject) => {
                const timeout = setTimeout(() => reject(new Error('Geolocation timed out')), 5000);
                navigator.geolocation.getCurrentPosition(
                    pos => { clearTimeout(timeout); resolve([pos.coords.latitude, pos.coords.longitude]); },
                    err => { clearTimeout(timeout); reject(err); }
                );
            })
        """);

        Assert.That(position[0], Is.EqualTo(48.8566f).Within(0.01));
        Assert.That(position[1], Is.EqualTo(2.3522f).Within(0.01));
        await page.CloseAsync();
    }

    // ───────────────────────── Offline ─────────────────────────

    [Test]
    public async Task Offline_ShouldEmulateOfflineMode()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .Offline()
            .BuildAsync();

        var page = await _context.NewPageAsync();
        var isOnline = await page.EvaluateAsync<bool>("() => navigator.onLine");

        Assert.That(isOnline, Is.False);
        await page.CloseAsync();
    }

    // ───────────────────────── WithJavaScript ─────────────────────────

    [Test]
    public async Task WithJavaScript_Disabled_ShouldDisableJS()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithJavaScript(false)
            .BuildAsync();

        var page = await _context.NewPageAsync();
        await page.GotoAsync($"{TestServerFixture.BaseUrl}/index.html");

        // The counter button has an onclick JS handler that increments the counter.
        // With JS disabled, clicking should have no effect.
        await page.Locator("[data-testid='counter-btn']").ClickAsync();
        var counterText = await page.Locator("[data-testid='counter-value']").TextContentAsync();

        Assert.That(counterText, Is.EqualTo("0"));
        await page.CloseAsync();
    }

    // ───────────────────────── WithBaseUrl ─────────────────────────

    [Test]
    public async Task WithBaseUrl_ShouldAllowRelativeNavigation()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithBaseUrl(TestServerFixture.BaseUrl)
            .BuildAsync();

        var page = await _context.NewPageAsync();
        await page.GotoAsync("/index.html");

        Assert.That(page.Url, Does.Contain("/index.html"));
        await page.CloseAsync();
    }

    // ───────────────────────── BypassCSP ─────────────────────────

    [Test]
    public async Task BypassCSP_ShouldCreateContextSuccessfully()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .BypassCSP()
            .BuildAsync();

        Assert.That(_context, Is.Not.Null);
    }

    // ───────────────────────── AsMobile / WithTouch ─────────────────────────

    [Test]
    public async Task AsMobile_WithTouch_ShouldEnableTouchEvents()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .AsMobile()
            .WithTouch()
            .WithViewport(375, 812)
            .BuildAsync();

        var page = await _context.NewPageAsync();
        var maxTouchPoints = await page.EvaluateAsync<int>("() => navigator.maxTouchPoints");

        Assert.That(maxTouchPoints, Is.GreaterThan(0));
        Assert.That(page.ViewportSize!.Width, Is.EqualTo(375));
        await page.CloseAsync();
    }

    // ───────────────────────── WithDeviceScaleFactor ─────────────────────────

    [Test]
    public async Task WithDeviceScaleFactor_ShouldApply()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithDeviceScaleFactor(2)
            .BuildAsync();

        var page = await _context.NewPageAsync();
        var dpr = await page.EvaluateAsync<float>("() => window.devicePixelRatio");

        Assert.That(dpr, Is.EqualTo(2));
        await page.CloseAsync();
    }

    // ───────────────────────── AcceptDownloads ─────────────────────────

    [Test]
    public async Task AcceptDownloads_ShouldCreateContextSuccessfully()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .AcceptDownloads()
            .BuildAsync();

        Assert.That(_context, Is.Not.Null);
    }

    // ───────────────────────── WithDefaultTimeout / WithDefaultNavigationTimeout ─────────────────────────

    [Test]
    public async Task WithDefaultTimeout_ShouldCreateContextSuccessfully()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithDefaultTimeout(5_000)
            .BuildAsync();

        Assert.That(_context, Is.Not.Null);
    }

    [Test]
    public async Task WithDefaultNavigationTimeout_ShouldCreateContextSuccessfully()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithDefaultNavigationTimeout(15_000)
            .BuildAsync();

        Assert.That(_context, Is.Not.Null);
    }

    // ───────────────────────── WithHarRecording ─────────────────────────

    [Test]
    public async Task WithHarRecording_ShouldCreateContextAndRecordHar()
    {
        var harPath = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}.har");

        try
        {
            _context = await BrowserContextBuilder.For(_browser)
                .WithBaseUrl(TestServerFixture.BaseUrl)
                .WithHarRecording(harPath)
                .BuildAsync();

            var page = await _context.NewPageAsync();
            await page.GotoAsync("/index.html");
            await page.CloseAsync();
            await _context.CloseAsync();
            _context = null;

            Assert.That(File.Exists(harPath), Is.True);
            var content = await File.ReadAllTextAsync(harPath);
            Assert.That(content, Does.Contain("index.html"));
        }
        finally
        {
            if (File.Exists(harPath))
                File.Delete(harPath);
        }
    }

    // ───────────────────────── WithVideoRecording ─────────────────────────

    [Test]
    public async Task WithVideoRecording_ShouldCreateVideoFiles()
    {
        var videoDir = Path.Combine(Path.GetTempPath(), $"pw-videos-{Guid.NewGuid()}");

        try
        {
            _context = await BrowserContextBuilder.For(_browser)
                .WithBaseUrl(TestServerFixture.BaseUrl)
                .WithVideoRecording(videoDir, 320, 240)
                .BuildAsync();

            var page = await _context.NewPageAsync();
            await page.GotoAsync("/index.html");

            // Ensure the video is written to disk before closing
            var videoPath = await page.Video!.PathAsync();
            await page.CloseAsync();
            await _context.CloseAsync();
            _context = null;

            Assert.That(File.Exists(videoPath), Is.True);
            Assert.That(new FileInfo(videoPath).Length, Is.GreaterThan(0));
        }
        finally
        {
            if (Directory.Exists(videoDir))
                Directory.Delete(videoDir, recursive: true);
        }
    }

    // ───────────────────────── WithTracing ─────────────────────────

    [Test]
    public async Task WithTracing_ShouldEnableTracingAndProduceTraceFile()
    {
        var tracePath = Path.Combine(Path.GetTempPath(), $"trace-{Guid.NewGuid()}.zip");

        try
        {
            _context = await BrowserContextBuilder.For(_browser)
                .WithBaseUrl(TestServerFixture.BaseUrl)
                .WithTracing(screenshots: true, snapshots: true, sources: false)
                .BuildAsync();

            var page = await _context.NewPageAsync();
            await page.GotoAsync("/index.html");

            await _context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
            await page.CloseAsync();

            Assert.That(File.Exists(tracePath), Is.True);
            Assert.That(new FileInfo(tracePath).Length, Is.GreaterThan(0));
        }
        finally
        {
            if (File.Exists(tracePath))
                File.Delete(tracePath);
        }
    }

    // ───────────────────────── WithDevice ─────────────────────────

    [Test]
    public async Task WithDevice_ShouldApplyDeviceDescriptorSettings()
    {
        var device = _playwright.Devices["iPhone 13"];

        _context = await BrowserContextBuilder.For(_browser)
            .WithDevice(device)
            .BuildAsync();

        var page = await _context.NewPageAsync();
        var ua = await page.EvaluateAsync<string>("() => navigator.userAgent");
        var maxTouchPoints = await page.EvaluateAsync<int>("() => navigator.maxTouchPoints");

        Assert.That(ua, Does.Contain("iPhone"));
        Assert.That(maxTouchPoints, Is.GreaterThan(0));
        Assert.That(page.ViewportSize!.Width, Is.LessThan(500));
        await page.CloseAsync();
    }

    [Test]
    public async Task WithDevice_ThenOverrideViewport_ShouldUseOverriddenViewport()
    {
        var device = _playwright.Devices["iPhone 13"];

        _context = await BrowserContextBuilder.For(_browser)
            .WithDevice(device)
            .WithViewport(1024, 768)
            .BuildAsync();

        var page = await _context.NewPageAsync();

        Assert.That(page.ViewportSize!.Width, Is.EqualTo(1024));
        Assert.That(page.ViewportSize.Height, Is.EqualTo(768));
        await page.CloseAsync();
    }

    // ───────────────────────── WithHttpCredentials ─────────────────────────

    [Test]
    public async Task WithHttpCredentials_ShouldCreateContextSuccessfully()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithHttpCredentials("admin", "secret")
            .BuildAsync();

        Assert.That(_context, Is.Not.Null);
    }

    // ───────────────────────── Full configuration chain ─────────────────────────

    [Test]
    public async Task FullConfigurationChain_ShouldCreateValidContext()
    {
        _context = await BrowserContextBuilder.For(_browser)
            .WithViewport(1280, 720)
            .WithLocale("fr-FR")
            .WithTimezone("Europe/Paris")
            .WithColorScheme(ColorScheme.Dark)
            .WithUserAgent("BromineFullTest/1.0")
            .WithDeviceScaleFactor(1.5f)
            .WithBaseUrl(TestServerFixture.BaseUrl)
            .AcceptDownloads()
            .WithDefaultTimeout(10_000)
            .WithDefaultNavigationTimeout(20_000)
            .BuildAsync();

        var page = await _context.NewPageAsync();

        // Verify viewport
        Assert.That(page.ViewportSize!.Width, Is.EqualTo(1280));
        Assert.That(page.ViewportSize.Height, Is.EqualTo(720));

        // Verify locale
        var locale = await page.EvaluateAsync<string>("() => navigator.language");
        Assert.That(locale, Is.EqualTo("fr-FR"));

        // Verify timezone
        var tz = await page.EvaluateAsync<string>("() => Intl.DateTimeFormat().resolvedOptions().timeZone");
        Assert.That(tz, Is.EqualTo("Europe/Paris"));

        // Verify color scheme
        var isDark = await page.EvaluateAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches");
        Assert.That(isDark, Is.True);

        // Verify user agent
        var ua = await page.EvaluateAsync<string>("() => navigator.userAgent");
        Assert.That(ua, Is.EqualTo("BromineFullTest/1.0"));

        // Verify DPR
        var dpr = await page.EvaluateAsync<float>("() => window.devicePixelRatio");
        Assert.That(dpr, Is.EqualTo(1.5f));

        // Verify relative navigation works
        await page.GotoAsync("/index.html");
        Assert.That(page.Url, Does.Contain("/index.html"));

        await page.CloseAsync();
    }

    // ───────────────────────── WithStorageState (invalid path) ─────────────────────────

    [Test]
    public void WithStorageState_InvalidPath_ShouldThrowOnBuild()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            _context = await BrowserContextBuilder.For(_browser)
                .WithStorageState("/nonexistent/path/state.json")
                .BuildAsync();
        });
    }
}

