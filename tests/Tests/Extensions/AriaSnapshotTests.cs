using Bromine.Playwright.Extensions.Extensions;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Extensions;

/// <summary>
/// Covers the aria snapshot helpers.
/// <para>
/// These assert that our wrappers reach the right Playwright call with the right arguments —
/// scoped to the locator rather than the page, <c>Mode</c> and <c>Depth</c> actually forwarded.
/// They deliberately do not assert Playwright's snapshot syntax or its exact truncation rules:
/// that is upstream's contract, and pinning it here would turn an upstream improvement into a
/// red build in this repo. The caveats table in the README carries what we found instead.
/// </para>
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
    public async Task Page_GetAriaSnapshotForAiAsync_ShouldForwardAiMode()
    {
        var normal = await Page.GetAriaSnapshotAsync();
        var forAi = await Page.GetAriaSnapshotForAiAsync();

        Assert.That(forAi, Is.Not.Empty);
        // Differing output is enough to prove Mode reached Playwright, without pinning the
        // shape of what AI mode emits.
        Assert.That(forAi, Is.Not.EqualTo(normal));
    }

    [Test]
    public async Task Page_GetAriaSnapshotForAiAsync_ShouldForwardDepth()
    {
        var full = await Page.GetAriaSnapshotForAiAsync();
        var limited = await Page.GetAriaSnapshotForAiAsync(depth: 1);

        Assert.That(limited, Is.Not.EqualTo(full), "depth should have reached Playwright");
    }

    // ───────────────────────── Locator snapshots ─────────────────────────

    [Test]
    public async Task Locator_GetAriaSnapshotAsync_ShouldBeScopedToTheElement()
    {
        var snapshot = await Page.Locator("[data-testid=navigation]").GetAriaSnapshotAsync();

        Assert.That(snapshot, Does.Contain("About"));
        Assert.That(snapshot, Does.Contain("Contact"));
        // The wrapper must target the locator, not the whole page.
        Assert.That(snapshot, Does.Not.Contain("Welcome to Bromine Testing"));
    }

    [Test]
    public async Task Locator_GetAriaSnapshotAsync_ShouldAcceptAnExplicitTimeout()
    {
        // Exercises the timeout argument rather than the PlaywrightDefaults fallback, which is
        // the path every other call here takes.
        var snapshot = await Page.Locator("[data-testid=navigation]")
            .GetAriaSnapshotAsync(timeoutMs: 10_000);

        Assert.That(snapshot, Does.Contain("About"));
    }

    [Test]
    public async Task Locator_GetAriaSnapshotForAiAsync_ShouldForwardAiMode()
    {
        var locator = Page.Locator("[data-testid=navigation]");

        var normal = await locator.GetAriaSnapshotAsync();
        var forAi = await locator.GetAriaSnapshotForAiAsync();

        Assert.That(forAi, Is.Not.Empty);
        Assert.That(forAi, Is.Not.EqualTo(normal));
    }
}
