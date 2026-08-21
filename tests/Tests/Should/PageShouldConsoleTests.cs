using Bromine.Playwright.Extensions.Assertions;
using Bromine.Playwright.Extensions.Extensions;
using Bromine.Playwright.Extensions.Tests.Support;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Should;

/// <summary>
/// Covers the console and page-error assertions added for Playwright 1.59.
/// </summary>
public class PageShouldConsoleTests : TestBase
{
    public PageShouldConsoleTests(BrowserType browser) : base(browser) { }

    [SetUp]
    public async Task GoToConsolePage()
    {
        await Page.GotoAsync("/console.html");
    }

    /// <summary>
    /// Click a button and wait until its console output has reached the driver, so the
    /// assertions under test are not racing the browser.
    /// </summary>
    private async Task ClickAndAwaitConsole(string testId, string expectedText)
    {
        await Page.Locator($"[data-testid={testId}]").ClickAsync();

        await Eventually.Async(
            () => Page.GetConsoleMessagesAsync(),
            m => m.Any(x => x.Text.Contains(expectedText)));
    }

    // ───────────────────────── HaveNoConsoleErrorsAsync ─────────────────────────

    [Test]
    public async Task HaveNoConsoleErrorsAsync_ShouldPass_OnCleanPage()
    {
        await Page.Should().HaveNoConsoleErrorsAsync();
    }

    [Test]
    public async Task HaveNoConsoleErrorsAsync_ShouldThrow_AfterConsoleError()
    {
        await ClickAndAwaitConsole("error-btn", "button error message");

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveNoConsoleErrorsAsync();
        });

        Assert.That(ex!.Message, Does.Contain("button error message"));
    }

    [Test]
    public async Task HaveNoConsoleErrorsAsync_ShouldPass_AfterClearingTheError()
    {
        await ClickAndAwaitConsole("error-btn", "button error message");

        await Page.ClearConsoleAsync();

        await Page.Should().HaveNoConsoleErrorsAsync();
    }

    [Test]
    public async Task HaveNoConsoleErrorsAsync_ShouldPass_WhenOnlyWarningsWereLogged()
    {
        await ClickAndAwaitConsole("warn-btn", "button warning message");

        await Page.Should().HaveNoConsoleErrorsAsync();
    }

    // ───────────────────────── Not.HaveNoConsoleErrorsAsync ─────────────────────────

    [Test]
    public async Task Not_HaveNoConsoleErrorsAsync_ShouldPass_AfterConsoleError()
    {
        await ClickAndAwaitConsole("error-btn", "button error message");

        await Page.Should().Not.HaveNoConsoleErrorsAsync();
    }

    [Test]
    public void Not_HaveNoConsoleErrorsAsync_ShouldThrow_OnCleanPage()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().Not.HaveNoConsoleErrorsAsync();
        });
    }

    // ───────────────────────── HaveNoPageErrorsAsync ─────────────────────────

    [Test]
    public async Task HaveNoPageErrorsAsync_ShouldPass_OnCleanPage()
    {
        await Page.Should().HaveNoPageErrorsAsync();
    }

    [Test]
    public async Task HaveNoPageErrorsAsync_ShouldThrow_AfterUncaughtException()
    {
        await Page.Locator("[data-testid=throw-btn]").ClickAsync();

        await Eventually.Async(() => Page.PageErrorsAsync(), e => e.Count > 0);

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveNoPageErrorsAsync();
        });

        Assert.That(ex!.Message, Does.Contain("uncaught button failure"));
    }

    [Test]
    public async Task Not_HaveNoPageErrorsAsync_ShouldPass_AfterUncaughtException()
    {
        await Page.Locator("[data-testid=throw-btn]").ClickAsync();

        await Eventually.Async(() => Page.PageErrorsAsync(), e => e.Count > 0);

        await Page.Should().Not.HaveNoPageErrorsAsync();
    }

    // ───────────────────────── HaveConsoleMessageAsync ─────────────────────────

    [Test]
    public async Task HaveConsoleMessageAsync_ShouldPass_ForLoadTimeMessage()
    {
        await Page.Should().HaveConsoleMessageAsync("page loaded cleanly");
    }

    [Test]
    public async Task HaveConsoleMessageAsync_ShouldPass_WithoutWaitingFirst()
    {
        // No Eventually here on purpose: the assertion's own polling is what is under test.
        await Page.Locator("[data-testid=log-btn]").ClickAsync();

        await Page.Should().HaveConsoleMessageAsync("button log message");
    }

    [Test]
    public void HaveConsoleMessageAsync_ShouldThrow_WhenMessageNeverArrives()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveConsoleMessageAsync("a message no button ever logs");
        });

        Assert.That(ex!.Message, Does.Contain("a message no button ever logs"));
    }

    [Test]
    public async Task Not_HaveConsoleMessageAsync_ShouldPass_WhenMessageWasNeverLogged()
    {
        await Page.Should().Not.HaveConsoleMessageAsync("a message no button ever logs");
    }

    [Test]
    public void Not_HaveConsoleMessageAsync_ShouldThrow_WhenMessageWasLogged()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().Not.HaveConsoleMessageAsync("page loaded cleanly");
        });
    }

    // ───────────────────────── Because ─────────────────────────

    [Test]
    public async Task Because_ShouldIncludeMessageOnFailure_ConsoleErrors()
    {
        await ClickAndAwaitConsole("error-btn", "button error message");

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveNoConsoleErrorsAsync(because: "the page should render without errors");
        });

        Assert.That(ex!.Message, Does.Contain("the page should render without errors"));
    }

    [Test]
    public void Because_WithFormat_ShouldIncludeFormattedMessage_ConsoleMessage()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveConsoleMessageAsync(
                "never logged",
                because: "the {0} step logs its progress",
                becauseArgs: ["checkout"]);
        });

        Assert.That(ex!.Message, Does.Contain("the checkout step logs its progress"));
    }

    // ───────────────────────── Chaining ─────────────────────────

    [Test]
    public async Task Chaining_ShouldPass_ConsoleAssertionsWithTitle()
    {
        await Page.Should()
            .HaveTitleAsync("Console - Bromine Test")
            .HaveNoConsoleErrorsAsync()
            .HaveNoPageErrorsAsync()
            .HaveConsoleMessageAsync("page loaded cleanly");
    }

    [Test]
    public async Task Chaining_ShouldThrow_WhenConsoleAssertionFails()
    {
        await ClickAndAwaitConsole("error-btn", "button error message");

        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should()
                .HaveTitleAsync("Console - Bromine Test")
                .HaveNoConsoleErrorsAsync();
        });
    }

    // ───────────────────────── sinceNavigationOnly ─────────────────────────

    [Test]
    public async Task HaveNoConsoleErrorsAsync_SinceNavigationOnly_ShouldIgnoreErrorsFromPreviousPage()
    {
        await ClickAndAwaitConsole("error-btn", "button error message");

        await Page.GotoAsync("/about.html");

        await Page.Should().HaveNoConsoleErrorsAsync(sinceNavigationOnly: true);
    }
}
