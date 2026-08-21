using Microsoft.Playwright;
using Bromine.Playwright.Extensions.Configuration;

namespace Bromine.Playwright.Extensions.Extensions;

/// <summary>
/// Extension methods for <see cref="IPage"/> providing navigation, cookie, 
/// screenshot, and wait helpers.
/// </summary>
public static class PageExtensions
{
    /// <summary>
    /// Navigate to a URL and wait for the network to be idle.
    /// </summary>
    public static async Task NavigateAndWaitAsync(
        this IPage page,
        string url,
        WaitUntilState waitUntil = WaitUntilState.NetworkIdle,
        float? timeoutMs = null)
    {
        await page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = waitUntil,
            Timeout = timeoutMs ?? PlaywrightDefaults.NavigationTimeout
        });
    }

    /// <summary>
    /// Navigate to a URL and wait for DOMContentLoaded.
    /// </summary>
    public static async Task NavigateAndWaitForDomAsync(this IPage page, string url, float? timeoutMs = null)
    {
        await page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = timeoutMs ?? PlaywrightDefaults.NavigationTimeout
        });
    }

    /// <summary>
    /// Reload the page and wait for the specified state.
    /// </summary>
    public static async Task ReloadAndWaitAsync(
        this IPage page,
        WaitUntilState waitUntil = WaitUntilState.NetworkIdle,
        float? timeoutMs = null)
    {
        await page.ReloadAsync(new PageReloadOptions
        {
            WaitUntil = waitUntil,
            Timeout = timeoutMs ?? PlaywrightDefaults.NavigationTimeout
        });
    }

    /// <summary>
    /// Wait for the page URL to contain the specified substring.
    /// </summary>
    public static async Task WaitForUrlContainingAsync(
        this IPage page,
        string urlSubstring,
        float? timeoutMs = null)
    {
        await page.WaitForURLAsync(
            $"**/*{urlSubstring}*",
            new PageWaitForURLOptions
            {
                Timeout = timeoutMs ?? PlaywrightDefaults.NavigationTimeout
            });
    }

    /// <summary>
    /// Wait for a specific network response matching the URL pattern.
    /// </summary>
    public static async Task<IResponse> WaitForResponseAsync(
        this IPage page,
        string urlPattern,
        float? timeoutMs = null)
    {
        return await page.WaitForResponseAsync(
            urlPattern,
            new PageWaitForResponseOptions
            {
                Timeout = timeoutMs ?? PlaywrightDefaults.NavigationTimeout
            });
    }

    /// <summary>
    /// Get a cookie by name from the current browser context.
    /// Returns null if the cookie is not found.
    /// </summary>
    public static async Task<BrowserContextCookiesResult?> GetCookieByNameAsync(
        this IPage page,
        string cookieName)
    {
        var cookies = await page.Context.CookiesAsync();
        return cookies.FirstOrDefault(c =>
            string.Equals(c.Name, cookieName, StringComparison.Ordinal));
    }

    /// <summary>
    /// Set a cookie in the current browser context.
    /// </summary>
    public static async Task SetCookieAsync(
        this IPage page,
        string name,
        string value,
        string? domain = null,
        string path = "/",
        float? expires = null,
        bool? secure = null,
        bool? httpOnly = null,
        SameSiteAttribute? sameSite = null)
    {
        domain ??= new Uri(page.Url).Host;

        await page.Context.AddCookiesAsync(new[]
        {
            new Cookie
            {
                Name = name,
                Value = value,
                Domain = domain,
                Path = path,
                Expires = expires ?? -1,
                Secure = secure,
                HttpOnly = httpOnly,
                SameSite = sameSite
            }
        });
    }

    /// <summary>
    /// Set multiple cookies at once.
    /// </summary>
    public static async Task SetCookiesAsync(this IPage page, params Cookie[] cookies)
    {
        await page.Context.AddCookiesAsync(cookies);
    }

    /// <summary>
    /// Clear all cookies from the browser context.
    /// </summary>
    public static async Task ClearCookiesAsync(this IPage page)
    {
        await page.Context.ClearCookiesAsync();
    }

    /// <summary>
    /// Take a full-page screenshot and return it as a byte array.
    /// </summary>
    public static async Task<byte[]> FullPageScreenshotAsync(this IPage page)
    {
        return await page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
    }

    /// <summary>
    /// Take a full-page screenshot and save it to the specified path.
    /// </summary>
    public static async Task<byte[]> FullPageScreenshotAsync(this IPage page, string savePath)
    {
        return await page.ScreenshotAsync(new PageScreenshotOptions
        {
            FullPage = true,
            Path = savePath
        });
    }

    /// <summary>
    /// Take a screenshot and return it as a Base64 string.
    /// </summary>
    public static async Task<string> ScreenshotToBase64Async(this IPage page, bool fullPage = true)
    {
        var bytes = await page.ScreenshotAsync(new PageScreenshotOptions { FullPage = fullPage });
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Try to click a selector. Returns true if clicked, false if element was not found.
    /// Does NOT throw on failure.
    /// </summary>
    public static async Task<bool> TryClickAsync(
        this IPage page,
        string selector,
        float? timeoutMs = null)
    {
        try
        {
            var locator = page.Locator(selector);
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeoutMs ?? PlaywrightDefaults.ActionTimeout
            });
            await locator.ClickAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Wait for a selector to be visible, then return its text content.
    /// </summary>
    public static async Task<string?> GetVisibleTextAsync(
        this IPage page,
        string selector,
        float? timeoutMs = null)
    {
        var locator = page.Locator(selector);
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeoutMs ?? PlaywrightDefaults.ActionTimeout
        });
        return await locator.TextContentAsync();
    }

    /// <summary>
    /// Evaluate JavaScript expression and return the result as the specified type.
    /// </summary>
    public static async Task<T> EvaluateAsync<T>(this IPage page, string expression)
    {
        return await page.EvaluateAsync<T>(expression);
    }

    /// <summary>
    /// Scroll to the bottom of the page.
    /// </summary>
    public static async Task ScrollToBottomAsync(this IPage page)
    {
        await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
    }

    /// <summary>
    /// Scroll to the top of the page.
    /// </summary>
    public static async Task ScrollToTopAsync(this IPage page)
    {
        await page.EvaluateAsync("window.scrollTo(0, 0)");
    }

    /// <summary>
    /// Wait for the page to be in a stable state (no pending network requests).
    /// </summary>
    public static async Task WaitForStableStateAsync(
        this IPage page,
        float? timeoutMs = null)
    {
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle,
            new PageWaitForLoadStateOptions
            {
                Timeout = timeoutMs ?? PlaywrightDefaults.NavigationTimeout
            });
    }

    /// <summary>
    /// Click a locator and wait for a file download. Returns the saved file path.
    /// </summary>
    public static async Task<string> ClickAndDownloadAsync(
        this IPage page,
        string selector,
        string saveDirectory,
        float? timeoutMs = null)
    {
        Directory.CreateDirectory(saveDirectory);

        var download = await page.RunAndWaitForDownloadAsync(
            async () => await page.Locator(selector).ClickAsync(),
            new PageRunAndWaitForDownloadOptions
            {
                Timeout = timeoutMs ?? PlaywrightDefaults.ActionTimeout
            });

        var filename = download.SuggestedFilename;
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var ext = Path.GetExtension(filename);
        var nameWithoutExt = Path.GetFileNameWithoutExtension(filename);
        var uniqueName = $"{nameWithoutExt}_{timestamp}{ext}";
        var savePath = Path.Combine(saveDirectory, uniqueName);

        await download.SaveAsAsync(savePath);
        return savePath;
    }

    /// <summary>
    /// Get the console messages Playwright has recorded for this page, oldest first.
    /// <para>
    /// Unlike the <c>Page.Console</c> event this reads messages retroactively, so there is no
    /// handler to attach before navigating. Playwright currently retains the last 200 messages.
    /// Pass <paramref name="sinceNavigationOnly"/> to drop everything logged before the most
    /// recent navigation. Requires Playwright 1.59 or newer.
    /// </para>
    /// </summary>
    public static async Task<IReadOnlyList<IConsoleMessage>> GetConsoleMessagesAsync(
        this IPage page,
        bool sinceNavigationOnly = false)
    {
        return await page.ConsoleMessagesAsync(new PageConsoleMessagesOptions
        {
            Filter = sinceNavigationOnly
                ? ConsoleMessagesFilter.SinceNavigation
                : ConsoleMessagesFilter.All
        });
    }

    /// <summary>
    /// Get only the <c>error</c>-level console messages recorded for this page.
    /// </summary>
    public static async Task<IReadOnlyList<IConsoleMessage>> GetConsoleErrorsAsync(
        this IPage page,
        bool sinceNavigationOnly = false)
    {
        var messages = await page.GetConsoleMessagesAsync(sinceNavigationOnly);
        return messages
            .Where(m => string.Equals(m.Type, "error", StringComparison.Ordinal))
            .ToList();
    }

    /// <summary>
    /// Discard every console message and page error recorded so far.
    /// <para>
    /// Useful between the arrange and act phases of a test, so a later
    /// <c>HaveNoConsoleErrorsAsync</c> only sees what the action itself produced.
    /// </para>
    /// </summary>
    public static async Task ClearConsoleAsync(this IPage page)
    {
        await page.ClearConsoleMessagesAsync();
        await page.ClearPageErrorsAsync();
    }

    /// <summary>
    /// Capture the page's aria snapshot. Equivalent to snapshotting the <c>body</c> locator.
    /// <para>
    /// <paramref name="depth"/> only has an effect in <see cref="AriaSnapshotMode.Ai"/>, whose
    /// output is a nested tree; the default mode returns a flat list that Playwright 1.59 does
    /// not truncate. Use <see cref="GetAriaSnapshotForAiAsync"/> when depth matters.
    /// </para>
    /// </summary>
    public static async Task<string> GetAriaSnapshotAsync(
        this IPage page,
        int? depth = null,
        AriaSnapshotMode? mode = null,
        float? timeoutMs = null)
    {
        return await page.AriaSnapshotAsync(new PageAriaSnapshotOptions
        {
            Depth = depth,
            Mode = mode,
            Timeout = timeoutMs ?? PlaywrightDefaults.ActionTimeout
        });
    }

    /// <summary>
    /// Capture the page's aria snapshot in the AI-optimised shape
    /// (<see cref="AriaSnapshotMode.Ai"/>), for handing page structure to a model.
    /// </summary>
    public static Task<string> GetAriaSnapshotForAiAsync(
        this IPage page,
        int? depth = null,
        float? timeoutMs = null)
        => page.GetAriaSnapshotAsync(depth, AriaSnapshotMode.Ai, timeoutMs);

    /// <summary>
    /// Start recording a screencast to <paramref name="savePath"/>, creating the directory if
    /// it does not exist. The video is written when the screencast stops.
    /// <para>
    /// Either <c>await using</c> the returned handle or call <see cref="StopScreencastAsync"/>.
    /// Requires Playwright 1.59 or newer.
    /// </para>
    /// </summary>
    public static async Task<IAsyncDisposable> StartScreencastAsync(
        this IPage page,
        string savePath,
        int? quality = null)
    {
        var directory = Path.GetDirectoryName(savePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        return await page.Screencast.StartAsync(new ScreencastStartOptions
        {
            Path = savePath,
            Quality = quality
        });
    }

    /// <summary>
    /// Stop the screencast and write the video to the path it was started with.
    /// </summary>
    public static Task StopScreencastAsync(this IPage page) => page.Screencast.StopAsync();

    /// <summary>
    /// Record a screencast of <paramref name="action"/> and return the saved video path.
    /// The screencast is stopped even if the action throws.
    /// </summary>
    public static async Task<string> RecordScreencastAsync(
        this IPage page,
        string savePath,
        Func<Task> action,
        int? quality = null)
    {
        _ = await page.StartScreencastAsync(savePath, quality);
        try
        {
            await action();
        }
        finally
        {
            await page.StopScreencastAsync();
        }

        return savePath;
    }

    /// <summary>
    /// Annotate the running screencast with the action Playwright is performing, so the recorded
    /// video shows what each step was doing.
    /// <para>
    /// Turn the annotations back off with <see cref="HideScreencastActionsAsync"/>.
    /// </para>
    /// </summary>
    public static async Task ShowScreencastActionsAsync(
        this IPage page,
        AnnotatePosition? position = null,
        float? durationMs = null,
        int? fontSize = null)
    {
        // Playwright 1.59 returns a no-op disposable here, so scoping it with `await using`
        // would silently do nothing — HideScreencastActionsAsync is the real off switch.
        _ = await page.Screencast.ShowActionsAsync(new ScreencastShowActionsOptions
        {
            Position = position,
            Duration = durationMs,
            FontSize = fontSize
        });
    }

    /// <summary>
    /// Stop annotating the running screencast with actions.
    /// </summary>
    public static Task HideScreencastActionsAsync(this IPage page)
        => page.Screencast.HideActionsAsync();
}

