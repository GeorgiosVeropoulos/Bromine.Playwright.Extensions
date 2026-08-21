using Bromine.Playwright.Extensions.Assertions;
using Bromine.Playwright.Extensions.Extensions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Should;

/// <summary>
/// Covers <c>MatchAriaSnapshotAsync</c> on the page, added for Playwright 1.59.
/// </summary>
public class PageShouldAriaSnapshotTests : TestBase
{
    public PageShouldAriaSnapshotTests(BrowserType browser) : base(browser) { }

    private const string WrongSnapshot = """
                                         - heading "A Page That Does Not Exist" [level=1]
                                         """;

    [Test]
    public async Task MatchAriaSnapshotAsync_ShouldPass_ForSnapshotCapturedFromThePage()
    {
        // Round-trip: whatever the page reports must satisfy the assertion, which pins the
        // helper and the matcher to the same shape.
        var snapshot = await Page.GetAriaSnapshotAsync();

        await Page.Should().MatchAriaSnapshotAsync(snapshot);
    }

    [Test]
    public async Task MatchAriaSnapshotAsync_ShouldPass_ForPartialSnapshot()
    {
        await Page.Should().MatchAriaSnapshotAsync("""
                                                  - heading "Welcome to Bromine Testing" [level=1]
                                                  """);
    }

    [Test]
    public void MatchAriaSnapshotAsync_ShouldThrow_WhenSnapshotDoesNotMatch()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().MatchAriaSnapshotAsync(WrongSnapshot);
        });
    }

    [Test]
    public async Task Not_MatchAriaSnapshotAsync_ShouldPass_WhenSnapshotDoesNotMatch()
    {
        await Page.Should().Not.MatchAriaSnapshotAsync(WrongSnapshot);
    }

    [Test]
    public async Task Not_MatchAriaSnapshotAsync_ShouldThrow_WhenSnapshotMatches()
    {
        var snapshot = await Page.GetAriaSnapshotAsync();

        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().Not.MatchAriaSnapshotAsync(snapshot);
        });
    }

    [Test]
    public void Because_ShouldIncludeMessageOnFailure()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().MatchAriaSnapshotAsync(
                WrongSnapshot,
                because: "the landing page structure is part of the contract");
        });

        Assert.That(ex!.Message, Does.Contain("the landing page structure is part of the contract"));
    }

    [Test]
    public async Task Chaining_ShouldPass_WithTitleAndSnapshot()
    {
        await Page.Should()
            .HaveTitleAsync("Bromine Test Page")
            .MatchAriaSnapshotAsync("""
                                    - heading "Welcome to Bromine Testing" [level=1]
                                    """);
    }
}
