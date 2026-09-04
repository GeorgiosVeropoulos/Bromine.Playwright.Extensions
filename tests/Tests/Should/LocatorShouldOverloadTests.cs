using System.Text.RegularExpressions;
using Bromine.Playwright.Extensions.Assertions;
using Bromine.Playwright.Extensions.Extensions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Should;

/// <summary>
/// Covers the <see cref="FluentLocatorAssertions"/> overloads that
/// <see cref="LocatorShouldTests"/> never reaches — the collection-based
/// (<c>IEnumerable&lt;string&gt;</c> / <c>IEnumerable&lt;Regex&gt;</c>) variants and the
/// locator-level aria snapshot matcher.
/// </summary>
public class LocatorShouldOverloadTests : TestBase
{
    public LocatorShouldOverloadTests(BrowserType browser) : base(browser) { }

    private ILocator ClassItems => Page.Locator("[data-testid='class-item']");

    // ───────────────────────── HaveClassAsync(IEnumerable<string>) ─────────────────────────

    [Test]
    public async Task HaveClassAsync_EnumerableString_ShouldPass_WhenEachElementClassMatches()
    {
        await ClassItems.Should().HaveClassAsync(new[] { "chip chip-a", "chip chip-b", "chip chip-c" });
    }

    [Test]
    public async Task HaveClassAsync_EnumerableString_Not_ShouldPass_WhenClassesDoNotMatch()
    {
        await ClassItems.Should().Not.HaveClassAsync(new[] { "wrong", "wrong", "wrong" });
    }

    [Test]
    public void HaveClassAsync_EnumerableString_ShouldThrow_WhenClassesDoNotMatch()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
            await ClassItems.Should().HaveClassAsync(
                new[] { "chip chip-a", "chip chip-b", "chip chip-WRONG" },
                because: "each chip must keep its modifier class"));

        Assert.That(ex!.Message, Does.Contain("each chip must keep its modifier class"));
    }

    // ───────────────────────── HaveClassAsync(IEnumerable<Regex>) ─────────────────────────

    [Test]
    public async Task HaveClassAsync_EnumerableRegex_ShouldPass_WhenEachElementClassMatchesPattern()
    {
        await ClassItems.Should().HaveClassAsync(new[]
        {
            new Regex("chip-a"),
            new Regex("chip-b"),
            new Regex("chip-c")
        });
    }

    [Test]
    public async Task HaveClassAsync_EnumerableRegex_Not_ShouldPass_WhenPatternsDoNotMatch()
    {
        await ClassItems.Should().Not.HaveClassAsync(new[]
        {
            new Regex("nope-a"),
            new Regex("nope-b"),
            new Regex("nope-c")
        });
    }

    // ───────────────────────── ContainClassAsync(IEnumerable<string>) ─────────────────────────

    [Test]
    public async Task ContainClassAsync_EnumerableString_ShouldPass_WhenEachElementContainsClass()
    {
        await ClassItems.Should().ContainClassAsync(new[] { "chip-a", "chip-b", "chip-c" });
    }

    [Test]
    public async Task ContainClassAsync_EnumerableString_ShouldPass_ForSharedClass()
    {
        // Unlike HaveClassAsync, the partial matcher ignores the sibling modifier classes.
        await ClassItems.Should().ContainClassAsync(new[] { "chip", "chip", "chip" });
    }

    [Test]
    public async Task ContainClassAsync_EnumerableString_Not_ShouldPass_WhenClassIsAbsent()
    {
        await ClassItems.Should().Not.ContainClassAsync(new[] { "absent", "absent", "absent" });
    }

    // ───────────────────────── HaveTextAsync(IEnumerable<Regex>) ─────────────────────────

    [Test]
    public async Task HaveTextAsync_EnumerableRegex_ShouldPass_WhenEachElementTextMatchesPattern()
    {
        await ClassItems.Should().HaveTextAsync(new[]
        {
            new Regex("^Alpha$"),
            new Regex("^Beta$"),
            new Regex("^Gamma$")
        });
    }

    [Test]
    public async Task HaveTextAsync_EnumerableRegex_Not_ShouldPass_WhenPatternsDoNotMatch()
    {
        await ClassItems.Should().Not.HaveTextAsync(new[]
        {
            new Regex("^Delta$"),
            new Regex("^Epsilon$"),
            new Regex("^Zeta$")
        });
    }

    // ───────────────────────── MatchAriaSnapshotAsync (locator scope) ─────────────────────────

    [Test]
    public async Task MatchAriaSnapshotAsync_ShouldPass_ForSnapshotCapturedFromTheLocator()
    {
        var navigation = Page.Locator("[data-testid='navigation']");

        // Round-trip: what the locator reports must satisfy the locator-scoped matcher.
        var snapshot = await navigation.GetAriaSnapshotAsync();

        await navigation.Should().MatchAriaSnapshotAsync(snapshot);
    }

    [Test]
    public async Task MatchAriaSnapshotAsync_ShouldPass_ForPartialSnapshotOfTheLocator()
    {
        await Page.Locator("[data-testid='navigation']").Should()
            .MatchAriaSnapshotAsync("""
                                    - navigation "Main navigation":
                                      - link "About"
                                      - link "Contact"
                                    """);
    }

    [Test]
    public void MatchAriaSnapshotAsync_ShouldThrow_WhenLocatorSnapshotDoesNotMatch()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
            await Page.Locator("[data-testid='navigation']").Should()
                .MatchAriaSnapshotAsync(
                    """
                    - navigation "Some other nav"
                    """,
                    because: "the primary navigation is part of the contract"));

        Assert.That(ex!.Message, Does.Contain("the primary navigation is part of the contract"));
    }

    [Test]
    public async Task MatchAriaSnapshotAsync_Not_ShouldPass_WhenLocatorSnapshotDoesNotMatch()
    {
        await Page.Locator("[data-testid='navigation']").Should()
            .Not.MatchAriaSnapshotAsync("""
                                        - navigation "Some other nav"
                                        """);
    }

    // ───────────────────────── Chaining across the overloads ─────────────────────────

    [Test]
    public async Task Chaining_ShouldPass_AcrossCollectionOverloads()
    {
        await ClassItems.Should()
            .HaveCountAsync(3)
            .HaveClassAsync(new[] { "chip chip-a", "chip chip-b", "chip chip-c" })
            .ContainClassAsync(new[] { "chip", "chip", "chip" })
            .HaveTextAsync(new[] { "Alpha", "Beta", "Gamma" })
            .Not.HaveTextAsync(new[] { new Regex("^x$"), new Regex("^y$"), new Regex("^z$") });
    }
}

