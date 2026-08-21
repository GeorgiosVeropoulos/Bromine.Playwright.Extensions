using Bromine.Playwright.Extensions.Extensions;
using Bromine.Playwright.Extensions.Tests.Support;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Extensions;

/// <summary>
/// Covers the console and page-error helpers added on top of Playwright 1.59's retroactive
/// <c>ConsoleMessagesAsync</c> / <c>PageErrorsAsync</c> APIs.
/// </summary>
public class PageConsoleTests : TestBase
{
    public PageConsoleTests(BrowserType browser) : base(browser) { }

    /// <summary>
    /// NUnit runs base-class SetUp first, so <see cref="TestBase"/> has already opened the page.
    /// </summary>
    [SetUp]
    public async Task GoToConsolePage()
    {
        await Page.GotoAsync("/console.html");
    }

    // ───────────────────────── GetConsoleMessagesAsync ─────────────────────────

    [Test]
    public async Task GetConsoleMessagesAsync_ShouldReturnMessageLoggedOnLoad()
    {
        var messages = await Eventually.Async(
            () => Page.GetConsoleMessagesAsync(),
            m => m.Any(x => x.Text.Contains("page loaded cleanly")));

        Assert.That(messages.Select(m => m.Text), Has.Some.Contains("page loaded cleanly"));
    }

    [Test]
    public async Task GetConsoleMessagesAsync_ShouldReturnMessageLoggedByClick()
    {
        await Page.Locator("[data-testid=log-btn]").ClickAsync();

        var messages = await Eventually.Async(
            () => Page.GetConsoleMessagesAsync(),
            m => m.Any(x => x.Text.Contains("button log message")));

        Assert.That(messages.Select(m => m.Text), Has.Some.Contains("button log message"));
    }

    [Test]
    public async Task GetConsoleMessagesAsync_ShouldIncludeWarnings()
    {
        await Page.Locator("[data-testid=warn-btn]").ClickAsync();

        var messages = await Eventually.Async(
            () => Page.GetConsoleMessagesAsync(),
            m => m.Any(x => x.Text.Contains("button warning message")));

        var warning = messages.FirstOrDefault(m => m.Text.Contains("button warning message"));

        Assert.That(warning, Is.Not.Null);
        Assert.That(warning!.Type, Is.EqualTo("warning"));
    }

    [Test]
    public async Task GetConsoleMessagesAsync_Timestamp_ShouldBePopulated()
    {
        var messages = await Eventually.Async(
            () => Page.GetConsoleMessagesAsync(),
            m => m.Count > 0);

        Assert.That(messages, Is.Not.Empty);
        // Milliseconds since the Unix epoch — any real value is far above zero.
        Assert.That(messages[0].Timestamp, Is.GreaterThan(0));
    }

    // ───────────────────────── GetConsoleErrorsAsync ─────────────────────────

    [Test]
    public async Task GetConsoleErrorsAsync_ShouldBeEmpty_OnCleanPage()
    {
        var errors = await Page.GetConsoleErrorsAsync();

        Assert.That(errors, Is.Empty);
    }

    [Test]
    public async Task GetConsoleErrorsAsync_ShouldReturnError_AfterClick()
    {
        await Page.Locator("[data-testid=error-btn]").ClickAsync();

        var errors = await Eventually.Async(
            () => Page.GetConsoleErrorsAsync(),
            e => e.Count > 0);

        Assert.That(errors, Is.Not.Empty);
        Assert.That(errors.Select(e => e.Text), Has.Some.Contains("button error message"));
        Assert.That(errors.All(e => e.Type == "error"), Is.True);
    }

    [Test]
    public async Task GetConsoleErrorsAsync_ShouldNotIncludeLogsOrWarnings()
    {
        await Page.Locator("[data-testid=log-btn]").ClickAsync();
        await Page.Locator("[data-testid=warn-btn]").ClickAsync();

        // Wait for both to land, so an empty result cannot just mean "not recorded yet".
        await Eventually.Async(
            () => Page.GetConsoleMessagesAsync(),
            m => m.Any(x => x.Text.Contains("button log message"))
                 && m.Any(x => x.Text.Contains("button warning message")));

        var errors = await Page.GetConsoleErrorsAsync();

        Assert.That(errors, Is.Empty);
    }

    // ───────────────────────── PageErrorsAsync ─────────────────────────

    [Test]
    public async Task PageErrorsAsync_ShouldRecordUncaughtException()
    {
        await Page.Locator("[data-testid=throw-btn]").ClickAsync();

        var errors = await Eventually.Async(
            () => Page.PageErrorsAsync(),
            e => e.Count > 0);

        Assert.That(errors, Is.Not.Empty);
        Assert.That(errors, Has.Some.Contains("uncaught button failure"));
    }

    // ───────────────────────── ClearConsoleAsync ─────────────────────────

    [Test]
    public async Task ClearConsoleAsync_ShouldDiscardRecordedMessages()
    {
        await Eventually.Async(
            () => Page.GetConsoleMessagesAsync(),
            m => m.Count > 0);

        await Page.ClearConsoleAsync();

        var messages = await Page.GetConsoleMessagesAsync();

        Assert.That(messages, Is.Empty);
    }

    [Test]
    public async Task ClearConsoleAsync_ShouldDiscardRecordedPageErrors()
    {
        await Page.Locator("[data-testid=throw-btn]").ClickAsync();

        await Eventually.Async(
            () => Page.PageErrorsAsync(),
            e => e.Count > 0);

        await Page.ClearConsoleAsync();

        var errors = await Page.PageErrorsAsync();

        Assert.That(errors, Is.Empty);
    }

    [Test]
    public async Task ClearConsoleAsync_ShouldOnlyHidePriorMessages()
    {
        await Page.ClearConsoleAsync();

        await Page.Locator("[data-testid=log-btn]").ClickAsync();

        var messages = await Eventually.Async(
            () => Page.GetConsoleMessagesAsync(),
            m => m.Any(x => x.Text.Contains("button log message")));

        Assert.That(messages.Select(m => m.Text), Has.Some.Contains("button log message"));
        Assert.That(messages.Select(m => m.Text), Has.None.Contains("page loaded cleanly"));
    }

    // ───────────────────────── sinceNavigationOnly filter ─────────────────────────

    [Test]
    public async Task GetConsoleMessagesAsync_SinceNavigationOnly_ShouldExcludePreviousPageMessages()
    {
        await Page.Locator("[data-testid=log-btn]").ClickAsync();

        await Eventually.Async(
            () => Page.GetConsoleMessagesAsync(),
            m => m.Any(x => x.Text.Contains("button log message")));

        await Page.GotoAsync("/about.html");

        var sinceNavigation = await Page.GetConsoleMessagesAsync(sinceNavigationOnly: true);

        Assert.That(sinceNavigation.Select(m => m.Text), Has.None.Contains("button log message"));
    }
}
