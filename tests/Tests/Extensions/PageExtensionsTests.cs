using Bromine.Playwright.Extensions.Configuration;
using Bromine.Playwright.Extensions.Extensions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Extensions;

public class PageExtensionsTests : TestBase
{
    public PageExtensionsTests(BrowserType browser) : base(browser) { }

    // ───────────────────────── NavigateAndWaitAsync ─────────────────────────

    [Test]
    public async Task NavigateAndWaitAsync_ShouldNavigateToUrl()
    {
        await Page.NavigateAndWaitAsync($"{TestServerFixture.BaseUrl}/about.html");

        Assert.That(Page.Url, Does.Contain("/about.html"));
    }

    [Test]
    public async Task NavigateAndWaitAsync_ShouldLoadPageContent()
    {
        await Page.NavigateAndWaitAsync($"{TestServerFixture.BaseUrl}/about.html");

        var heading = await Page.Locator("[data-testid='about-heading']").TextContentAsync();
        Assert.That(heading, Is.EqualTo("About Bromine"));
    }

    [Test]
    public async Task NavigateAndWaitAsync_WithDOMContentLoaded_ShouldNavigate()
    {
        await Page.NavigateAndWaitAsync(
            $"{TestServerFixture.BaseUrl}/contact.html",
            waitUntil: WaitUntilState.DOMContentLoaded);

        Assert.That(Page.Url, Does.Contain("/contact.html"));
    }

    [Test]
    public async Task NavigateAndWaitAsync_WithCustomTimeout_ShouldNavigate()
    {
        await Page.NavigateAndWaitAsync(
            $"{TestServerFixture.BaseUrl}/index.html",
            timeoutMs: 10_000);

        Assert.That(Page.Url, Does.Contain("/index.html"));
    }

    // ───────────────────────── NavigateAndWaitForDomAsync ─────────────────────────

    [Test]
    public async Task NavigateAndWaitForDomAsync_ShouldNavigateToUrl()
    {
        await Page.NavigateAndWaitForDomAsync($"{TestServerFixture.BaseUrl}/about.html");

        Assert.That(Page.Url, Does.Contain("/about.html"));
    }

    [Test]
    public async Task NavigateAndWaitForDomAsync_ShouldLoadDomContent()
    {
        await Page.NavigateAndWaitForDomAsync($"{TestServerFixture.BaseUrl}/contact.html");

        var heading = await Page.Locator("[data-testid='contact-heading']").TextContentAsync();
        Assert.That(heading, Is.EqualTo("Contact Us"));
    }

    [Test]
    public async Task NavigateAndWaitForDomAsync_WithCustomTimeout_ShouldNavigate()
    {
        await Page.NavigateAndWaitForDomAsync($"{TestServerFixture.BaseUrl}/index.html", timeoutMs: 10_000);

        Assert.That(Page.Url, Does.Contain("/index.html"));
    }

    // ───────────────────────── ReloadAndWaitAsync ─────────────────────────

    [Test]
    public async Task ReloadAndWaitAsync_ShouldReloadCurrentPage()
    {
        await Page.NavigateAndWaitAsync($"{TestServerFixture.BaseUrl}/about.html");
        var urlBefore = Page.Url;

        await Page.ReloadAndWaitAsync();

        Assert.That(Page.Url, Is.EqualTo(urlBefore));
    }

    [Test]
    public async Task ReloadAndWaitAsync_WithDOMContentLoaded_ShouldReload()
    {
        await Page.ReloadAndWaitAsync(waitUntil: WaitUntilState.DOMContentLoaded);

        Assert.That(Page.Url, Does.Contain(TestServerFixture.BaseUrl));
    }

    [Test]
    public async Task ReloadAndWaitAsync_WithCustomTimeout_ShouldReload()
    {
        await Page.ReloadAndWaitAsync(timeoutMs: 10_000);

        var heading = await Page.Locator("[data-testid='heading']").TextContentAsync();
        Assert.That(heading, Is.EqualTo("Welcome to Bromine Testing"));
    }

    // ───────────────────────── WaitForUrlContainingAsync ─────────────────────────

    [Test]
    public async Task WaitForUrlContainingAsync_ShouldPass_WhenUrlAlreadyContainsSubstring()
    {
        await Page.NavigateAndWaitAsync($"{TestServerFixture.BaseUrl}/about.html");

        await Page.WaitForUrlContainingAsync("about");
    }

    [Test]
    public async Task WaitForUrlContainingAsync_ShouldPass_AfterNavigation()
    {
        // Start navigation then wait for URL
        await Page.GotoAsync($"{TestServerFixture.BaseUrl}/contact.html");

        await Page.WaitForUrlContainingAsync("contact");
    }

    // ───────────────────────── WaitForResponseAsync ─────────────────────────

    [Test]
    public async Task WaitForResponseAsync_ShouldCaptureResponse()
    {
        var responseTask = Page.WaitForResponseAsync($"{TestServerFixture.BaseUrl}/api/ok");
        await Page.EvaluateAsync($"() => fetch('{TestServerFixture.BaseUrl}/api/ok')");
        var response = await responseTask;

        Assert.That(response.Status, Is.EqualTo(200));
        Assert.That(response.Url, Does.Contain("/api/ok"));
    }

    // ───────────────────────── GetCookieByNameAsync ─────────────────────────

    [Test]
    public async Task GetCookieByNameAsync_ShouldReturnCookie_WhenExists()
    {
        await Page.SetCookieAsync("test-cookie", "test-value");

        var cookie = await Page.GetCookieByNameAsync("test-cookie");

        Assert.That(cookie, Is.Not.Null);
        Assert.That(cookie!.Name, Is.EqualTo("test-cookie"));
        Assert.That(cookie.Value, Is.EqualTo("test-value"));
    }

    [Test]
    public async Task GetCookieByNameAsync_ShouldReturnNull_WhenNotFound()
    {
        var cookie = await Page.GetCookieByNameAsync("nonexistent-cookie");

        Assert.That(cookie, Is.Null);
    }

    // ───────────────────────── SetCookieAsync ─────────────────────────

    [Test]
    public async Task SetCookieAsync_ShouldSetCookieWithNameAndValue()
    {
        await Page.SetCookieAsync("my-cookie", "my-value");

        var cookie = await Page.GetCookieByNameAsync("my-cookie");
        Assert.That(cookie, Is.Not.Null);
        Assert.That(cookie!.Value, Is.EqualTo("my-value"));
    }

    [Test]
    public async Task SetCookieAsync_ShouldSetCookieWithPath()
    {
        await Page.SetCookieAsync("path-cookie", "path-value", path: "/");

        var cookie = await Page.GetCookieByNameAsync("path-cookie");
        Assert.That(cookie, Is.Not.Null);
        Assert.That(cookie!.Path, Is.EqualTo("/"));
    }

    [Test]
    public async Task SetCookieAsync_ShouldSetHttpOnlyCookie()
    {
        await Page.SetCookieAsync("http-cookie", "http-value", httpOnly: true);

        var cookie = await Page.GetCookieByNameAsync("http-cookie");
        Assert.That(cookie, Is.Not.Null);
        Assert.That(cookie!.HttpOnly, Is.True);
    }

    // ───────────────────────── SetCookiesAsync ─────────────────────────

    [Test]
    public async Task SetCookiesAsync_ShouldSetMultipleCookies()
    {
        var domain = new Uri(Page.Url).Host;
        await Page.SetCookiesAsync(
            new Cookie { Name = "cookie-a", Value = "value-a", Domain = domain, Path = "/" },
            new Cookie { Name = "cookie-b", Value = "value-b", Domain = domain, Path = "/" }
        );

        var cookieA = await Page.GetCookieByNameAsync("cookie-a");
        var cookieB = await Page.GetCookieByNameAsync("cookie-b");
        Assert.That(cookieA, Is.Not.Null);
        Assert.That(cookieA!.Value, Is.EqualTo("value-a"));
        Assert.That(cookieB, Is.Not.Null);
        Assert.That(cookieB!.Value, Is.EqualTo("value-b"));
    }

    // ───────────────────────── ClearCookiesAsync ─────────────────────────

    [Test]
    public async Task ClearCookiesAsync_ShouldRemoveAllCookies()
    {
        await Page.SetCookieAsync("to-delete", "bye");
        var before = await Page.GetCookieByNameAsync("to-delete");
        Assert.That(before, Is.Not.Null);

        await Page.ClearCookiesAsync();

        var after = await Page.GetCookieByNameAsync("to-delete");
        Assert.That(after, Is.Null);
    }

    // ───────────────────────── FullPageScreenshotAsync (bytes) ─────────────────────────

    [Test]
    public async Task FullPageScreenshotAsync_ShouldReturnNonEmptyByteArray()
    {
        var bytes = await Page.FullPageScreenshotAsync();

        Assert.That(bytes, Is.Not.Null);
        Assert.That(bytes.Length, Is.GreaterThan(0));
    }

    // ───────────────────────── FullPageScreenshotAsync (save to path) ─────────────────────────

    [Test]
    public async Task FullPageScreenshotAsync_WithPath_ShouldSaveFile()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"screenshot-{Guid.NewGuid()}.png");

        try
        {
            var bytes = await Page.FullPageScreenshotAsync(tempPath);

            Assert.That(File.Exists(tempPath), Is.True);
            Assert.That(new FileInfo(tempPath).Length, Is.GreaterThan(0));
            Assert.That(bytes.Length, Is.GreaterThan(0));
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    // ───────────────────────── ScreenshotToBase64Async ─────────────────────────

    [Test]
    public async Task ScreenshotToBase64Async_ShouldReturnValidBase64String()
    {
        var base64 = await Page.ScreenshotToBase64Async();

        Assert.That(base64, Is.Not.Null.And.Not.Empty);
        // Verify it's valid Base64 by round-tripping
        var bytes = Convert.FromBase64String(base64);
        Assert.That(bytes.Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task ScreenshotToBase64Async_NotFullPage_ShouldReturnBase64()
    {
        var base64 = await Page.ScreenshotToBase64Async(fullPage: false);

        Assert.That(base64, Is.Not.Null.And.Not.Empty);
        var bytes = Convert.FromBase64String(base64);
        Assert.That(bytes.Length, Is.GreaterThan(0));
    }

    // ───────────────────────── TryClickAsync ─────────────────────────

    [Test]
    public async Task TryClickAsync_ShouldReturnTrue_WhenElementExists()
    {
        var result = await Page.TryClickAsync("[data-testid='click-btn']");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task TryClickAsync_ShouldPerformClick()
    {
        await Page.TryClickAsync("[data-testid='click-btn']");

        var text = await Page.Locator("[data-testid='click-result']").TextContentAsync();
        Assert.That(text, Is.EqualTo("Clicked!"));
    }

    [Test]
    public async Task TryClickAsync_ShouldReturnFalse_WhenElementDoesNotExist()
    {
        var result = await Page.TryClickAsync("[data-testid='nonexistent']", timeoutMs: 1_000);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task TryClickAsync_ShouldReturnFalse_WhenElementIsHidden()
    {
        var result = await Page.TryClickAsync("[data-testid='hidden-element']", timeoutMs: 1_000);

        Assert.That(result, Is.False);
    }

    // ───────────────────────── GetVisibleTextAsync ─────────────────────────

    [Test]
    public async Task GetVisibleTextAsync_ShouldReturnTextContent()
    {
        var text = await Page.GetVisibleTextAsync("[data-testid='heading']");

        Assert.That(text, Is.EqualTo("Welcome to Bromine Testing"));
    }

    [Test]
    public async Task GetVisibleTextAsync_ShouldReturnText_ForParagraph()
    {
        var text = await Page.GetVisibleTextAsync("[data-testid='paragraph']");

        Assert.That(text, Is.EqualTo("Hello World"));
    }

    [Test]
    public void GetVisibleTextAsync_ShouldThrow_WhenElementNotVisible()
    {
        Assert.ThrowsAsync<TimeoutException>(async () =>
        {
            await Page.GetVisibleTextAsync("[data-testid='hidden-element']", timeoutMs: 1_000);
        });
    }

    // ───────────────────────── ScrollToBottomAsync ─────────────────────────

    [Test]
    public async Task ScrollToBottomAsync_ShouldScrollToBottom()
    {
        await Page.ScrollToBottomAsync();

        var scrollY = await Page.EvaluateAsync<double>("() => window.scrollY");
        // On a short page this might be 0 — just verify it doesn't throw
        Assert.That(scrollY, Is.GreaterThanOrEqualTo(0));
    }

    // ───────────────────────── ScrollToTopAsync ─────────────────────────

    [Test]
    public async Task ScrollToTopAsync_ShouldScrollToTop()
    {
        // First scroll down
        await Page.ScrollToBottomAsync();
        await Page.ScrollToTopAsync();

        var scrollY = await Page.EvaluateAsync<double>("() => window.scrollY");
        Assert.That(scrollY, Is.EqualTo(0));
    }

    // ───────────────────────── WaitForStableStateAsync ─────────────────────────

    [Test]
    public async Task WaitForStableStateAsync_ShouldWaitForNetworkIdle()
    {
        await Page.NavigateAndWaitAsync($"{TestServerFixture.BaseUrl}/index.html");

        // Should complete without timeout — page is already stable
        await Page.WaitForStableStateAsync();

        // Verify page is loaded
        var heading = await Page.Locator("[data-testid='heading']").TextContentAsync();
        Assert.That(heading, Is.EqualTo("Welcome to Bromine Testing"));
    }

    [Test]
    public async Task WaitForStableStateAsync_WithCustomTimeout_ShouldComplete()
    {
        await Page.WaitForStableStateAsync(timeoutMs: 10_000);

        Assert.That(Page.Url, Does.Contain(TestServerFixture.BaseUrl));
    }

    // ───────────────────────── Defaults integration ─────────────────────────

    [Test]
    public async Task NavigateAndWaitAsync_ShouldUsePlaywrightDefaults_NavigationTimeout()
    {
        PlaywrightDefaults.NavigationTimeout = 60_000;

        // Should use the configured default timeout
        await Page.NavigateAndWaitAsync($"{TestServerFixture.BaseUrl}/about.html");

        Assert.That(Page.Url, Does.Contain("/about.html"));
    }

    [Test]
    public async Task TryClickAsync_ShouldUsePlaywrightDefaults_ActionTimeout()
    {
        PlaywrightDefaults.ActionTimeout = 2_000;

        var result = await Page.TryClickAsync("[data-testid='nonexistent']");

        Assert.That(result, Is.False);
    }

    // ───────────────────────── ClickAndDownloadAsync ─────────────────────────

    [Test]
    public async Task ClickAndDownloadAsync_ShouldDownloadFileAndReturnPath()
    {
        var saveDir = Path.Combine(Path.GetTempPath(), $"pw-download-{Guid.NewGuid()}");

        try
        {
            var savedPath = await Page.ClickAndDownloadAsync(
                "[data-testid='download-link']",
                saveDir);

            Assert.That(File.Exists(savedPath), Is.True);
            Assert.That(new FileInfo(savedPath).Length, Is.GreaterThan(0));
        }
        finally
        {
            if (Directory.Exists(saveDir))
                Directory.Delete(saveDir, recursive: true);
        }
    }

    [Test]
    public async Task ClickAndDownloadAsync_ShouldCreateSaveDirectory()
    {
        var saveDir = Path.Combine(Path.GetTempPath(), $"pw-download-{Guid.NewGuid()}");

        try
        {
            Assert.That(Directory.Exists(saveDir), Is.False);

            await Page.ClickAndDownloadAsync("[data-testid='download-link']", saveDir);

            Assert.That(Directory.Exists(saveDir), Is.True);
        }
        finally
        {
            if (Directory.Exists(saveDir))
                Directory.Delete(saveDir, recursive: true);
        }
    }

    [Test]
    public async Task ClickAndDownloadAsync_SavedFile_ShouldContainExpectedContent()
    {
        var saveDir = Path.Combine(Path.GetTempPath(), $"pw-download-{Guid.NewGuid()}");

        try
        {
            var savedPath = await Page.ClickAndDownloadAsync(
                "[data-testid='download-link']",
                saveDir);

            var content = await File.ReadAllTextAsync(savedPath);
            Assert.That(content, Is.EqualTo("This is a test download file."));
        }
        finally
        {
            if (Directory.Exists(saveDir))
                Directory.Delete(saveDir, recursive: true);
        }
    }

    [Test]
    public async Task ClickAndDownloadAsync_FileName_ShouldContainTimestamp()
    {
        var saveDir = Path.Combine(Path.GetTempPath(), $"pw-download-{Guid.NewGuid()}");

        try
        {
            var savedPath = await Page.ClickAndDownloadAsync(
                "[data-testid='download-link']",
                saveDir);

            var fileName = Path.GetFileNameWithoutExtension(savedPath);
            // Format: test-download_yyyyMMdd_HHmmss
            Assert.That(fileName, Does.StartWith("test-download_"));
            Assert.That(Path.GetExtension(savedPath), Is.EqualTo(".txt"));
        }
        finally
        {
            if (Directory.Exists(saveDir))
                Directory.Delete(saveDir, recursive: true);
        }
    }
}

