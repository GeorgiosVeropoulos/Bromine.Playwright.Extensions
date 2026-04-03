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
}

