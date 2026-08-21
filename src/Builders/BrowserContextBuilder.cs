using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Builders;

/// <summary>
/// Fluent builder for creating a Playwright <see cref="IBrowserContext"/> with all options.
/// <example>
/// <code>
/// var context = await BrowserContextBuilder.For(browser)
///     .WithViewport(1920, 1080)
///     .WithPermissions("clipboard-read", "clipboard-write")
///     .WithHttpCredentials("user", "pass")
///     .WithGeolocation(37.7749, -122.4194)
///     .BypassCSP()
///     .WithHarRecording("./traces/network.har")
///     .WithVideoRecording("./traces/videos")
///     .BuildAsync();
/// </code>
/// </example>
/// </summary>
public class BrowserContextBuilder
{
    private readonly IBrowser _browser;
    private ViewportSize? _viewport;
    private bool? _bypassCSP;
    private bool? _isMobile;
    private bool? _hasTouch;
    private string? _locale;
    private string? _timezone;
    private ColorScheme? _colorScheme;
    private IEnumerable<string>? _permissions;
    private HttpCredentials? _httpCredentials;
    private Geolocation? _geolocation;
    private bool? _javaScriptEnabled;
    private bool? _acceptDownloads;
    private string? _userAgent;
    private float? _deviceScaleFactor;
    private Proxy? _proxy;
    private string? _baseUrl;
    private bool? _offline;
    private string? _storageStatePath;
    private float? _defaultNavigationTimeout;
    private float? _defaultTimeout;

    // HAR recording
    private string? _harPath;
    private bool? _harOmitContent;
    private string? _harUrlFilter;
    private HarMode? _harMode;

    // Video recording
    private string? _videoDir;
    private RecordVideoSize? _videoSize;

    // Tracing
    private bool _enableTracing;
    private bool _tracingScreenshots;
    private bool _tracingSnapshots;
    private bool _tracingSources;
    private bool _tracingLive;

    private BrowserContextBuilder(IBrowser browser)
    {
        _browser = browser;
    }

    /// <summary>
    /// Create a new context builder for the given browser.
    /// </summary>
    public static BrowserContextBuilder For(IBrowser browser) => new(browser);

    /// <summary>
    /// Set the viewport size.
    /// </summary>
    public BrowserContextBuilder WithViewport(int width, int height)
    {
        _viewport = new ViewportSize { Width = width, Height = height };
        return this;
    }

    /// <summary>
    /// Use no fixed viewport (viewport = null, full window).
    /// </summary>
    public BrowserContextBuilder WithNoViewport()
    {
        _viewport = ViewportSize.NoViewport;
        return this;
    }

    /// <summary>
    /// Bypass Content Security Policy.
    /// </summary>
    public BrowserContextBuilder BypassCSP(bool bypass = true)
    {
        _bypassCSP = bypass;
        return this;
    }

    /// <summary>
    /// Enable mobile emulation mode.
    /// </summary>
    public BrowserContextBuilder AsMobile(bool isMobile = true)
    {
        _isMobile = isMobile;
        return this;
    }

    /// <summary>
    /// Enable touch events.
    /// </summary>
    public BrowserContextBuilder WithTouch(bool hasTouch = true)
    {
        _hasTouch = hasTouch;
        return this;
    }

    /// <summary>
    /// Set the browser locale (e.g., "en-US", "de-DE").
    /// </summary>
    public BrowserContextBuilder WithLocale(string locale)
    {
        _locale = locale;
        return this;
    }

    /// <summary>
    /// Set the timezone (e.g., "America/New_York", "Europe/Athens").
    /// </summary>
    public BrowserContextBuilder WithTimezone(string timezone)
    {
        _timezone = timezone;
        return this;
    }

    /// <summary>
    /// Set the preferred color scheme ("light", "dark", "no-preference").
    /// </summary>
    public BrowserContextBuilder WithColorScheme(ColorScheme colorScheme)
    {
        _colorScheme = colorScheme;
        return this;
    }

    /// <summary>
    /// Grant browser permissions (e.g., "geolocation", "clipboard-read", "clipboard-write").
    /// </summary>
    public BrowserContextBuilder WithPermissions(params string[] permissions)
    {
        _permissions = permissions;
        return this;
    }

    /// <summary>
    /// Set HTTP credentials for HTTP authentication.
    /// </summary>
    public BrowserContextBuilder WithHttpCredentials(string username, string password)
    {
        _httpCredentials = new HttpCredentials { Username = username, Password = password };
        return this;
    }

    /// <summary>
    /// Set geolocation coordinates.
    /// </summary>
    public BrowserContextBuilder WithGeolocation(float latitude, float longitude, float? accuracy = null)
    {
        _geolocation = new Geolocation { Latitude = latitude, Longitude = longitude, Accuracy = accuracy };
        return this;
    }

    /// <summary>
    /// Enable or disable JavaScript.
    /// </summary>
    public BrowserContextBuilder WithJavaScript(bool enabled = true)
    {
        _javaScriptEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Accept downloads automatically.
    /// </summary>
    public BrowserContextBuilder AcceptDownloads(bool accept = true)
    {
        _acceptDownloads = accept;
        return this;
    }

    /// <summary>
    /// Set custom user agent string.
    /// </summary>
    public BrowserContextBuilder WithUserAgent(string userAgent)
    {
        _userAgent = userAgent;
        return this;
    }

    /// <summary>
    /// Set device scale factor.
    /// </summary>
    public BrowserContextBuilder WithDeviceScaleFactor(float factor)
    {
        _deviceScaleFactor = factor;
        return this;
    }

    /// <summary>
    /// Configure proxy settings.
    /// </summary>
    public BrowserContextBuilder WithProxy(string server, string? bypass = null, string? username = null, string? password = null)
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
    /// Set a base URL for relative navigations.
    /// </summary>
    public BrowserContextBuilder WithBaseUrl(string baseUrl)
    {
        _baseUrl = baseUrl;
        return this;
    }

    /// <summary>
    /// Emulate offline mode.
    /// </summary>
    public BrowserContextBuilder Offline(bool offline = true)
    {
        _offline = offline;
        return this;
    }

    /// <summary>
    /// Restore storage state from a file (cookies, localStorage).
    /// </summary>
    public BrowserContextBuilder WithStorageState(string path)
    {
        _storageStatePath = path;
        return this;
    }

    /// <summary>
    /// Set default navigation timeout for all pages in this context.
    /// </summary>
    public BrowserContextBuilder WithDefaultNavigationTimeout(float timeoutMs)
    {
        _defaultNavigationTimeout = timeoutMs;
        return this;
    }

    /// <summary>
    /// Set default timeout for all operations in this context.
    /// </summary>
    public BrowserContextBuilder WithDefaultTimeout(float timeoutMs)
    {
        _defaultTimeout = timeoutMs;
        return this;
    }

    /// <summary>
    /// Enable HAR recording.
    /// </summary>
    public BrowserContextBuilder WithHarRecording(string path, bool omitContent = false, string? urlFilter = null, HarMode? harMode = null)
    {
        _harPath = path;
        _harOmitContent = omitContent;
        _harUrlFilter = urlFilter;
        _harMode = harMode;
        return this;
    }

    /// <summary>
    /// Enable video recording.
    /// </summary>
    public BrowserContextBuilder WithVideoRecording(string directory, int? width = null, int? height = null)
    {
        _videoDir = directory;
        if (width.HasValue && height.HasValue)
        {
            _videoSize = new RecordVideoSize { Width = width.Value, Height = height.Value };
        }
        return this;
    }

    /// <summary>
    /// Enable tracing for this context. Call <c>context.Tracing.StopAsync()</c> to save the trace.
    /// </summary>
    public BrowserContextBuilder WithTracing(bool screenshots = true, bool snapshots = true, bool sources = true)
    {
        _enableTracing = true;
        _tracingScreenshots = screenshots;
        _tracingSnapshots = snapshots;
        _tracingSources = sources;
        return this;
    }

    /// <summary>
    /// Write the trace to an unarchived file that updates in real time instead of zipping it
    /// on stop, so it can be opened in the trace viewer while the test is still running.
    /// <para>
    /// Enables tracing on its own, so it can either replace <see cref="WithTracing"/> or refine
    /// it — <c>.WithTracing(sources: false).WithLiveTracing()</c> keeps the flags already set.
    /// Requires Playwright 1.59 or newer.
    /// </para>
    /// </summary>
    public BrowserContextBuilder WithLiveTracing(bool live = true)
    {
        // Called on its own it has to stand in for WithTracing, so adopt the same defaults.
        // Called after it, _enableTracing is already set and the explicit flags are left alone.
        if (!_enableTracing)
        {
            _tracingScreenshots = true;
            _tracingSnapshots = true;
            _tracingSources = true;
        }

        _enableTracing = true;
        _tracingLive = live;
        return this;
    }

    /// <summary>
    /// Apply a Playwright device descriptor (e.g., from <c>playwright.Devices["iPhone 13"]</c>).
    /// The returned builder can still override individual properties.
    /// </summary>
    public BrowserContextBuilder WithDevice(BrowserNewContextOptions deviceDescriptor)
    {
        _viewport = deviceDescriptor.ViewportSize;
        _userAgent = deviceDescriptor.UserAgent;
        _deviceScaleFactor = deviceDescriptor.DeviceScaleFactor;
        _isMobile = deviceDescriptor.IsMobile;
        _hasTouch = deviceDescriptor.HasTouch;
        return this;
    }

    /// <summary>
    /// Build the browser context with all configured options.
    /// </summary>
    public async Task<IBrowserContext> BuildAsync()
    {
        var options = new BrowserNewContextOptions
        {
            ViewportSize = _viewport,
            BypassCSP = _bypassCSP,
            IsMobile = _isMobile,
            HasTouch = _hasTouch,
            Locale = _locale,
            TimezoneId = _timezone,
            Permissions = _permissions,
            HttpCredentials = _httpCredentials,
            Geolocation = _geolocation,
            JavaScriptEnabled = _javaScriptEnabled,
            AcceptDownloads = _acceptDownloads,
            UserAgent = _userAgent,
            DeviceScaleFactor = _deviceScaleFactor,
            Proxy = _proxy,
            BaseURL = _baseUrl,
            Offline = _offline,
            StorageStatePath = _storageStatePath,
            RecordHarPath = _harPath,
            RecordHarOmitContent = _harOmitContent,
            RecordHarUrlFilter = _harUrlFilter,
            RecordHarMode = _harMode,
            RecordVideoDir = _videoDir,
            RecordVideoSize = _videoSize,
        };

        if (_colorScheme != null)
        {
            options.ColorScheme = _colorScheme;
        }

        var context = await _browser.NewContextAsync(options);

        if (_defaultNavigationTimeout.HasValue)
            context.SetDefaultNavigationTimeout(_defaultNavigationTimeout.Value);

        if (_defaultTimeout.HasValue)
            context.SetDefaultTimeout(_defaultTimeout.Value);

        if (_enableTracing)
        {
            await context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = _tracingScreenshots,
                Snapshots = _tracingSnapshots,
                Sources = _tracingSources,
                Live = _tracingLive
            });
        }

        return context;
    }
}

