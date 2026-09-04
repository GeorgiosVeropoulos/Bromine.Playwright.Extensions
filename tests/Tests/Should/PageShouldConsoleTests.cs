using System.Diagnostics;
using Bromine.Playwright.Extensions.Assertions;
using Bromine.Playwright.Extensions.Configuration;
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
        // Discard whatever TestBase's opening navigation to "/" logged before moving on, so
        // these assertions only ever see console.html's own output. Defence in depth: the
        // server serves /favicon.ico precisely so there is nothing to discard.
        await Page.ClearConsoleAsync();
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

    // ───────────────────────── Not.HaveConsoleErrorsAsync ─────────────────────────

    [Test]
    public async Task Not_HaveConsoleErrorsAsync_ShouldPass_AfterClearingTheError()
    {
        await ClickAndAwaitConsole("error-btn", "button error message");

        await Page.ClearConsoleAsync();

        await Page.Should().Not.HaveConsoleErrorsAsync();
    }

    [Test]
    public async Task Not_HaveConsoleErrorsAsync_ShouldPass_WhenOnlyWarningsWereLogged()
    {
        await ClickAndAwaitConsole("warn-btn", "button warning message");

        await Page.Should().Not.HaveConsoleErrorsAsync();
    }

    [Test]
    public async Task Not_HaveConsoleErrorsAsync_ShouldFailFast_RatherThanWaitOutTheTimeout()
    {
        await ClickAndAwaitConsole("error-btn", "button error message");

        // Absence is checked once, so a failure must report well inside the assertion timeout
        // rather than spending it. Guards the asymmetric-retry design: were this polled, it
        // would take the full timeout and this would fail.
        var stopwatch = Stopwatch.StartNew();
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().Not.HaveConsoleErrorsAsync();
        });
        stopwatch.Stop();

        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(PlaywrightDefaults.AssertionTimeout / 2));
    }

    // ───────────────────────── HaveConsoleErrorsAsync ─────────────────────────

    [Test]
    public async Task HaveConsoleErrorsAsync_ShouldPass_WithoutWaitingFirst()
    {
        // No Eventually here on purpose: the assertion's own polling is what is under test.
        await Page.Locator("[data-testid=error-btn]").ClickAsync();

        await Page.Should().HaveConsoleErrorsAsync();
    }

    [Test]
    public void HaveConsoleErrorsAsync_ShouldThrow_OnCleanPage()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveConsoleErrorsAsync();
        });

        Assert.That(ex!.Message, Does.Contain("none were recorded"));
    }

    [Test]
    public async Task Not_HaveConsoleErrorsAsync_ShouldPass_OnCleanPage()
    {
        await Page.Should().Not.HaveConsoleErrorsAsync();
    }

    [Test]
    public async Task Not_HaveConsoleErrorsAsync_ShouldThrow_AfterConsoleError()
    {
        await ClickAndAwaitConsole("error-btn", "button error message");

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().Not.HaveConsoleErrorsAsync();
        });

        Assert.That(ex!.Message, Does.Contain("button error message"));
    }

    [Test]
    public async Task HaveConsoleErrorsAsync_SinceNavigationOnly_ShouldIgnoreErrorsFromPreviousPage()
    {
        await ClickAndAwaitConsole("error-btn", "button error message");

        await Page.GotoAsync("/about.html");

        await Page.Should().Not.HaveConsoleErrorsAsync(sinceNavigationOnly: true);
    }

    [Test]
    public void Because_ShouldIncludeMessageOnFailure_HaveConsoleErrors()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveConsoleErrorsAsync(because: "the bad input should have been reported");
        });

        Assert.That(ex!.Message, Does.Contain("the bad input should have been reported"));
    }

    // ───────────────────────── HavePageErrorsAsync ─────────────────────────

    [Test]
    public async Task HavePageErrorsAsync_ShouldPass_WithoutWaitingFirst()
    {
        await Page.Locator("[data-testid=throw-btn]").ClickAsync();

        await Page.Should().HavePageErrorsAsync();
    }

    [Test]
    public void HavePageErrorsAsync_ShouldThrow_OnCleanPage()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HavePageErrorsAsync();
        });

        Assert.That(ex!.Message, Does.Contain("none were recorded"));
    }

    [Test]
    public async Task Not_HavePageErrorsAsync_ShouldPass_OnCleanPage()
    {
        await Page.Should().Not.HavePageErrorsAsync();
    }

    [Test]
    public async Task Not_HavePageErrorsAsync_ShouldThrow_AfterUncaughtException()
    {
        await Page.Locator("[data-testid=throw-btn]").ClickAsync();

        await Eventually.Async(() => Page.PageErrorsAsync(), e => e.Count > 0);

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().Not.HavePageErrorsAsync();
        });

        Assert.That(ex!.Message, Does.Contain("uncaught button failure"));
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
    public async Task HaveConsoleMessageAsync_ShouldReportThatNothingWasLogged_WhenConsoleIsEmpty()
    {
        // The failure message has two shapes — one listing what was recorded, one saying nothing
        // was. This covers the empty shape, which needs a page that logs nothing at all.
        await Page.GotoAsync("/about.html");
        await Page.ClearConsoleAsync();

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveConsoleMessageAsync("nothing will be logged here");
        });

        Assert.That(ex!.Message, Does.Contain("no console messages were recorded"));
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
            await Page.Should().Not.HaveConsoleErrorsAsync(because: "the page should render without errors");
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
        // Each Not applies only to the assertion that follows it.
        await Page.Should()
            .HaveTitleAsync("Console - Bromine Test")
            .Not.HaveConsoleErrorsAsync()
            .Not.HavePageErrorsAsync()
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
                .Not.HaveConsoleErrorsAsync();
        });
    }
}
