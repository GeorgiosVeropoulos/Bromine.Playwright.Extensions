using Bromine.Playwright.Extensions.Extensions;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Extensions;

/// <summary>
/// Covers the aria snapshot helpers built on Playwright 1.59's page-level
/// <c>AriaSnapshotAsync</c> and the new <c>Depth</c> / <c>Mode</c> options.
/// </summary>
public class AriaSnapshotTests : TestBase
{
    public AriaSnapshotTests(BrowserType browser) : base(browser) { }

    // ───────────────────────── Page snapshots ─────────────────────────

    [Test]
    public async Task Page_GetAriaSnapshotAsync_ShouldDescribeThePage()
    {
        var snapshot = await Page.GetAriaSnapshotAsync();

        Assert.That(snapshot, Is.Not.Empty);
        Assert.That(snapshot, Does.Contain("Welcome to Bromine Testing"));
    }

    [Test]
    public async Task Page_GetAriaSnapshotAsync_WithDepth_ShouldBeIgnoredInDefaultMode()
    {
        // Pins the documented caveat: the default snapshot is a flat list, and Playwright 1.59
        // does not truncate it. If a later version starts honouring depth here, this fails and
        // the docs need updating.
        var full = await Page.GetAriaSnapshotAsync();
        var shallow = await Page.GetAriaSnapshotAsync(depth: 1);

        Assert.That(shallow, Is.EqualTo(full));
    }

    [Test]
    public async Task Page_GetAriaSnapshotForAiAsync_ShouldReturnSnapshot()
    {
        var snapshot = await Page.GetAriaSnapshotForAiAsync();

        Assert.That(snapshot, Is.Not.Empty);
        // AI mode annotates nodes with refs, which the default mode does not.
        Assert.That(snapshot, Does.Contain("[ref="));
    }

    [Test]
    public async Task Page_GetAriaSnapshotForAiAsync_WithDepth_ShouldTruncateTheTree()
    {
        var full = await Page.GetAriaSnapshotForAiAsync();
        var depth1 = await Page.GetAriaSnapshotForAiAsync(depth: 1);
        var depth2 = await Page.GetAriaSnapshotForAiAsync(depth: 2);

        Assert.That(depth1.Length, Is.LessThan(depth2.Length));
        Assert.That(depth2.Length, Is.LessThan(full.Length));
    }

    // ───────────────────────── Locator snapshots ─────────────────────────

    [Test]
    public async Task Locator_GetAriaSnapshotAsync_ShouldBeScopedToTheElement()
    {
        var snapshot = await Page.Locator("[data-testid=navigation]").GetAriaSnapshotAsync();

        Assert.That(snapshot, Does.Contain("About"));
        Assert.That(snapshot, Does.Contain("Contact"));
        // Scoped to the nav, so the page heading must not leak in.
        Assert.That(snapshot, Does.Not.Contain("Welcome to Bromine Testing"));
    }

    [Test]
    public async Task Locator_GetAriaSnapshotAsync_WithDepth_ShouldBeAccepted()
    {
        var locator = Page.Locator("[data-testid=navigation]");

        // Depth is accepted but not honoured for locator snapshots in 1.59 — asserted rather
        // than assumed, so a behaviour change upstream surfaces here.
        var full = await locator.GetAriaSnapshotAsync();
        var shallow = await locator.GetAriaSnapshotAsync(depth: 1);

        Assert.That(shallow, Is.EqualTo(full));
    }

    [Test]
    public async Task Locator_GetAriaSnapshotForAiAsync_ShouldReturnSnapshot()
    {
        var snapshot = await Page.Locator("[data-testid=navigation]").GetAriaSnapshotForAiAsync();

        Assert.That(snapshot, Is.Not.Empty);
        Assert.That(snapshot, Does.Contain("[ref="));
    }
}
