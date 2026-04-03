using Bromine.Playwright.Extensions.Configuration;
using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Builders;

/// <summary>
/// Fluent builder for creating a Playwright <see cref="IBrowser"/> instance.
/// <example>
/// <code>
/// var (playwright, browser) = await PlaywrightBrowserBuilder.Create()
///     .WithChromium()
///     .Headless()
///     .WithSlowMotion(50)
///     .WithTimeout(10_000)
///     .BuildAsync();
/// </code>
/// </example>
/// </summary>
public class PlaywrightBrowserBuilder
{
    private BrowserType _browserType = BrowserType.Chromium;
    private bool _headless = true;
    private float? _slowMotion;
    private float? _timeout;
    private string? _channel;
    private string? _executablePath;
    private IEnumerable<string>? _args;
    private string? _downloadsPath;
    private bool? _chromiumSandbox;
    private Proxy? _proxy;
    private Dictionary<string, string>? _env;
    private bool? _handleSigint;
    private bool? _handleSigterm;
    private bool? _handleSighup;
    private string? _tracesDir;

    private PlaywrightBrowserBuilder() { }

    /// <summary>
    /// Create a new builder instance.
    /// </summary>
    public static PlaywrightBrowserBuilder Create() => new();

    /// <summary>
    /// Use Chromium browser engine (default).
    /// </summary>
    public PlaywrightBrowserBuilder WithChromium()
    {
        _browserType = BrowserType.Chromium;
        return this;
    }

    /// <summary>
    /// Use Firefox browser engine.
    /// </summary>
    public PlaywrightBrowserBuilder WithFirefox()
    {
        _browserType = BrowserType.Firefox;
        return this;
    }

    /// <summary>
    /// Use Webkit browser engine.
    /// </summary>
    public PlaywrightBrowserBuilder WithWebkit()
    {
        _browserType = BrowserType.Webkit;
        return this;
    }

    /// <summary>
    /// Run in headless mode (default: true).
    /// </summary>
    public PlaywrightBrowserBuilder Headless(bool headless = true)
    {
        _headless = headless;
        return this;
    }

    /// <summary>
    /// Run in headed (visible) mode.
    /// </summary>
    public PlaywrightBrowserBuilder Headed()
    {
        _headless = false;
        return this;
    }

    /// <summary>
    /// Slow down operations by the specified amount of milliseconds.
    /// Useful for debugging.
    /// </summary>
    public PlaywrightBrowserBuilder WithSlowMotion(float milliseconds)
    {
        _slowMotion = milliseconds;
        return this;
    }

    /// <summary>
    /// Maximum time in milliseconds to wait for the browser instance to start.
    /// </summary>
    public PlaywrightBrowserBuilder WithTimeout(float timeoutMs)
    {
        _timeout = timeoutMs;
        return this;
    }

    /// <summary>
    /// Browser distribution channel (e.g., "chrome", "chrome-beta", "msedge").
    /// </summary>
    public PlaywrightBrowserBuilder WithChannel(string channel)
    {
        _channel = channel;
        return this;
    }

    /// <summary>
    /// Path to a browser executable to use instead of the bundled one.
    /// </summary>
    public PlaywrightBrowserBuilder WithExecutablePath(string path)
    {
        _executablePath = path;
        return this;
    }

    /// <summary>
    /// Additional arguments to pass to the browser instance.
    /// </summary>
    public PlaywrightBrowserBuilder WithArgs(params string[] args)
    {
        _args = args;
        return this;
    }

    /// <summary>
    /// Set the downloads directory.
    /// </summary>
    public PlaywrightBrowserBuilder WithDownloadsPath(string path)
    {
        _downloadsPath = path;
        return this;
    }

    /// <summary>
    /// Configure Chromium sandbox mode.
    /// </summary>
    public PlaywrightBrowserBuilder WithChromiumSandbox(bool sandbox)
    {
        _chromiumSandbox = sandbox;
        return this;
    }

    /// <summary>
    /// Configure proxy settings for the browser.
    /// </summary>
    public PlaywrightBrowserBuilder WithProxy(string server, string? bypass = null, string? username = null, string? password = null)
    {
        _proxy = new Proxy
        {
            Server = server,
            Bypass = bypass,
            Username = username,
            Password = password
        };
        return this;
    }

    /// <summary>
    /// Set environment variables for the browser process.
    /// </summary>
    public PlaywrightBrowserBuilder WithEnvironment(Dictionary<string, string> env)
    {
        _env = env;
        return this;
    }

    /// <summary>
    /// Set the traces directory.
    /// </summary>
    public PlaywrightBrowserBuilder WithTracesDir(string path)
    {
        _tracesDir = path;
        return this;
    }

    /// <summary>
    /// Handle SIGINT signal.
    /// </summary>
    public PlaywrightBrowserBuilder HandleSigint(bool handle = true)
    {
        _handleSigint = handle;
        return this;
    }

    /// <summary>
    /// Handle SIGTERM signal.
    /// </summary>
    public PlaywrightBrowserBuilder HandleSigterm(bool handle = true)
    {
        _handleSigterm = handle;
        return this;
    }

    /// <summary>
    /// Handle SIGHUP signal.
    /// </summary>
    public PlaywrightBrowserBuilder HandleSighup(bool handle = true)
    {
        _handleSighup = handle;
        return this;
    }

    /// <summary>
    /// Build and launch the browser. Returns both the IPlaywright instance (for disposal)
    /// and the IBrowser.
    /// </summary>
    public async Task<PlaywrightBrowserResult> BuildAsync()
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        var browserType = _browserType switch
        {
            BrowserType.Firefox => playwright.Firefox,
            BrowserType.Webkit => playwright.Webkit,
            _ => playwright.Chromium
        };

        var options = new BrowserTypeLaunchOptions
        {
            Headless = _headless,
            SlowMo = _slowMotion,
            Timeout = _timeout,
            Channel = _channel,
            ExecutablePath = _executablePath,
            Args = _args,
            DownloadsPath = _downloadsPath,
            ChromiumSandbox = _chromiumSandbox,
            Proxy = _proxy,
            Env = _env,
            HandleSIGINT = _handleSigint,
            HandleSIGTERM = _handleSigterm,
            HandleSIGHUP = _handleSighup,
            TracesDir = _tracesDir
        };

        var browser = await browserType.LaunchAsync(options);
        return new PlaywrightBrowserResult(playwright, browser);
    }

    internal enum BrowserType
    {
        Chromium,
        Firefox,
        Webkit
    }
}

/// <summary>
/// Result of building a Playwright browser, containing both the IPlaywright and IBrowser instances.
/// Implements IAsyncDisposable for clean teardown.
/// </summary>
public class PlaywrightBrowserResult : IAsyncDisposable
{
    public IPlaywright Playwright { get; }
    public IBrowser Browser { get; }

    internal PlaywrightBrowserResult(IPlaywright playwright, IBrowser browser)
    {
        Playwright = playwright;
        Browser = browser;
    }

    /// <summary>
    /// Deconstruct into (IPlaywright, IBrowser) tuple.
    /// </summary>
    public void Deconstruct(out IPlaywright playwright, out IBrowser browser)
    {
        playwright = Playwright;
        browser = Browser;
    }

    /// <summary>
    /// Close browser and dispose Playwright.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await Browser.CloseAsync();
        Playwright.Dispose();
        GC.SuppressFinalize(this);
    }
}

