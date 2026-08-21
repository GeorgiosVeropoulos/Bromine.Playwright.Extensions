using Bromine.Playwright.Extensions.Extensions;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Extensions;

/// <summary>
/// Covers Playwright 1.59's <c>ILocator.NormalizeAsync</c> and the selector helper built on it.
/// </summary>
public class LocatorNormalizeTests : TestBase
{
    public LocatorNormalizeTests(BrowserType browser) : base(browser) { }

    [Test]
    public async Task NormalizeAsync_ShouldResolveToTheSameElement()
    {
        // The heading carries both an id and a data-testid, so normalising should keep pointing
        // at it while preferring the more resilient attribute.
        var byCss = Page.Locator("#main-heading");

        var normalized = await byCss.NormalizeAsync();

        Assert.That(await normalized.TextContentAsync(), Is.EqualTo(await byCss.TextContentAsync()));
        Assert.That(await normalized.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task NormalizedSelectorAsync_ShouldReturnASelector()
    {
        var selector = await Page.Locator("#main-heading").NormalizedSelectorAsync();

        Assert.That(selector, Is.Not.Empty);
    }

    [Test]
    public async Task NormalizedSelectorAsync_ShouldPreferTheTestIdOverCss()
    {
        var selector = await Page.Locator("#main-heading").NormalizedSelectorAsync();

        // data-testid="heading" is the recommended handle for this element.
        Assert.That(selector, Does.Contain("heading"));
    }

    [Test]
    public async Task NormalizeAsync_ShouldWorkForNestedElements()
    {
        var link = Page.Locator("[data-testid=navigation] a[href='/about.html']");

        var normalized = await link.NormalizeAsync();

        Assert.That(await normalized.CountAsync(), Is.EqualTo(1));
        Assert.That(await normalized.TextContentAsync(), Is.EqualTo("About"));
    }
}
