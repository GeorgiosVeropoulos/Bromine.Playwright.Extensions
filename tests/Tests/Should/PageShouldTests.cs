using System.Text.RegularExpressions;
using Bromine.Playwright.Extensions.Assertions;
using Bromine.Playwright.Extensions.Tests.Constants;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Should;

public class PageShouldTests : TestBase
{
    public PageShouldTests(BrowserType browser) : base(browser) { }

    // ───────────────────────── HaveTitleAsync ─────────────────────────

    [Test]
    public async Task HaveTitleAsync_ShouldPass_WhenTitleMatches()
    {
        await Page.Should().HaveTitleAsync(KnownPageTitles.BromineTestPage);
    }

    [Test]
    public async Task HaveTitleAsync_ShouldPass_AfterNavigation()
    {
        await Page.GotoAsync("/about.html");

        await Page.Should().HaveTitleAsync("About - Bromine Test");
    }

    [Test]
    public void HaveTitleAsync_ShouldThrow_WhenTitleDoesNotMatch()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveTitleAsync("Wrong Title");
        });
    }

    // ───────────────────────── Not.HaveTitleAsync ─────────────────────────

    [Test]
    public async Task Not_HaveTitleAsync_ShouldPass_WhenTitleDoesNotMatch()
    {
        await Page.Should().Not.HaveTitleAsync("Wrong Title");
    }

    [Test]
    public void Not_HaveTitleAsync_ShouldThrow_WhenTitleMatches()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().Not.HaveTitleAsync("Bromine Test Page");
        });
    }

    // ───────────────────────── HaveURLAsync (string) ─────────────────────────

    [Test]
    public async Task HaveURLAsync_ShouldPass_WithExactURL()
    {
        await Page.Should().HaveURLAsync($"{TestServerFixture.BaseUrl}/");
    }

    [Test]
    public async Task HaveURLAsync_ShouldPass_AfterNavigation()
    {
        await Page.GotoAsync("/about.html");

        await Page.Should().HaveURLAsync($"{TestServerFixture.BaseUrl}/about.html");
    }

    [Test]
    public void HaveURLAsync_ShouldThrow_WhenURLDoesNotMatch()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveURLAsync($"{TestServerFixture.BaseUrl}/nonexistent.html");
        });
    }

    // ───────────────────────── Not.HaveURLAsync ─────────────────────────

    [Test]
    public async Task Not_HaveURLAsync_ShouldPass_WhenURLDoesNotMatch()
    {
        await Page.Should().Not.HaveURLAsync($"{TestServerFixture.BaseUrl}/nonexistent.html");
    }

    [Test]
    public void Not_HaveURLAsync_ShouldThrow_WhenURLMatches()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().Not.HaveURLAsync($"{TestServerFixture.BaseUrl}/");
        });
    }

    // ───────────────────────── Chaining ─────────────────────────

    [Test]
    public async Task Chaining_ShouldPass_TitleAndURL()
    {
        await Page.Should()
            .HaveTitleAsync("Bromine Test Page")
            .HaveURLAsync($"{TestServerFixture.BaseUrl}/");
    }

    [Test]
    public async Task Chaining_ShouldPass_AfterNavigation()
    {
        await Page.GotoAsync("/contact.html");

        await Page.Should()
            .HaveTitleAsync("Contact - Bromine Test")
            .HaveURLAsync($"{TestServerFixture.BaseUrl}/contact.html");
    }

    [Test]
    public void Chaining_ShouldThrow_WhenSecondAssertionFails()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should()
                .HaveTitleAsync("Bromine Test Page")
                .HaveURLAsync($"{TestServerFixture.BaseUrl}/wrong.html");
        });
    }

    [Test]
    public void Chaining_ShouldThrow_WhenFirstAssertionFails()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should()
                .HaveTitleAsync("Wrong Title")
                .HaveURLAsync($"{TestServerFixture.BaseUrl}/");
        });
    }

    // ───────────────────────── Because ─────────────────────────

    [Test]
    public void Because_ShouldIncludeMessageOnFailure_Title()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveTitleAsync("Wrong Title", because: "we navigated to the home page");
        });

        Assert.That(ex!.Message, Does.Contain("we navigated to the home page"));
    }

    [Test]
    public void Because_ShouldIncludeMessageOnFailure_URL()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveURLAsync($"{TestServerFixture.BaseUrl}/wrong.html", because: "we should be on the home page");
        });

        Assert.That(ex!.Message, Does.Contain("we should be on the home page"));
    }

    [Test]
    public void Because_WithFormat_ShouldIncludeFormattedMessage()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveTitleAsync("Wrong", because: "expected title on {0} page", becauseArgs: ["home"]);
        });

        Assert.That(ex!.Message, Does.Contain("expected title on home page"));
    }

    // ───────────────────────── Chaining with Not ─────────────────────────

    [Test]
    public async Task Chaining_Not_ShouldPass_MixedAssertions()
    {
        await Page.Should()
            .HaveTitleAsync("Bromine Test Page")
            .Not.HaveURLAsync($"{TestServerFixture.BaseUrl}/about.html");
    }

    [Test]
    public async Task Chaining_Not_ShouldPass_AfterNavigation()
    {
        await Page.GotoAsync("/about.html");

        await Page.Should()
            .Not.HaveTitleAsync("Bromine Test Page")
            .HaveURLAsync($"{TestServerFixture.BaseUrl}/about.html");
    }

    // ───────────────────────── HaveTitleAsync (Regex) ─────────────────────────

    [Test]
    public async Task HaveTitleAsync_Regex_ShouldPass_WhenTitleMatchesPattern()
    {
        await Page.Should().HaveTitleAsync(new Regex("Bromine.*Page"));
    }

    [Test]
    public async Task HaveTitleAsync_Regex_ShouldPass_PartialMatch()
    {
        await Page.Should().HaveTitleAsync(new Regex("Test Page$"));
    }

    [Test]
    public void HaveTitleAsync_Regex_ShouldThrow_WhenTitleDoesNotMatchPattern()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveTitleAsync(new Regex("^Wrong.*"));
        });
    }

    [Test]
    public async Task Not_HaveTitleAsync_Regex_ShouldPass_WhenTitleDoesNotMatchPattern()
    {
        await Page.Should().Not.HaveTitleAsync(new Regex("^Wrong.*"));
    }

    [Test]
    public void Not_HaveTitleAsync_Regex_ShouldThrow_WhenTitleMatchesPattern()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().Not.HaveTitleAsync(new Regex("Bromine.*Page"));
        });
    }

    // ───────────────────────── HaveURLAsync (Regex) ─────────────────────────

    [Test]
    public async Task HaveURLAsync_Regex_ShouldPass_WhenURLMatchesPattern()
    {
        await Page.GotoAsync("/about.html");

        await Page.Should().HaveURLAsync(new Regex(".*/about\\.html$"));
    }

    [Test]
    public async Task HaveURLAsync_Regex_ShouldPass_PartialMatch()
    {
        await Page.Should().HaveURLAsync(new Regex("127\\.0\\.0\\.1"));
    }

    [Test]
    public void HaveURLAsync_Regex_ShouldThrow_WhenURLDoesNotMatchPattern()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveURLAsync(new Regex(".*/nonexistent\\.html$"));
        });
    }

    [Test]
    public async Task Not_HaveURLAsync_Regex_ShouldPass_WhenURLDoesNotMatchPattern()
    {
        await Page.Should().Not.HaveURLAsync(new Regex(".*/nonexistent\\.html$"));
    }

    [Test]
    public void Not_HaveURLAsync_Regex_ShouldThrow_WhenURLMatchesPattern()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().Not.HaveURLAsync(new Regex("127\\.0\\.0\\.1"));
        });
    }

    // ───────────────────────── Chaining with Regex ─────────────────────────

    [Test]
    public async Task Chaining_Regex_ShouldPass_TitleAndURL()
    {
        await Page.GotoAsync("/contact.html");

        await Page.Should()
            .HaveTitleAsync(new Regex("Contact.*Bromine"))
            .HaveURLAsync(new Regex(".*/contact\\.html$"));
    }

    [Test]
    public async Task Chaining_Regex_ShouldPass_MixedStringAndRegex()
    {
        await Page.Should()
            .HaveTitleAsync(new Regex("^Bromine"))
            .HaveURLAsync($"{TestServerFixture.BaseUrl}/");
    }
}
