using Bromine.Playwright.Extensions.Assertions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Should;

/// <summary>
/// Covers <c>HaveLocalStorageItemAsync</c> / <c>HaveSessionStorageItemAsync</c>, built on
/// Playwright 1.61's <c>Page.LocalStorage</c> / <c>Page.SessionStorage</c>.
/// <para>
/// Storage is mutable, so unlike the console assertions both directions retry — the
/// "…ShouldRetry…" tests pin that by making the assertion's outcome depend on a write that
/// happens after it starts polling.
/// </para>
/// </summary>
public class PageShouldStorageTests : TestBase
{
    public PageShouldStorageTests(BrowserType browser) : base(browser) { }

    [Test]
    public async Task HaveLocalStorageItemAsync_ShouldPass_WhenItemExists()
    {
        await Page.LocalStorage.SetItemAsync("user-id", "42");

        await Page.Should().HaveLocalStorageItemAsync("user-id");
    }

    [Test]
    public async Task HaveLocalStorageItemAsync_ShouldPass_WhenValueMatches()
    {
        await Page.LocalStorage.SetItemAsync("user-id", "42");

        await Page.Should().HaveLocalStorageItemAsync("user-id", "42");
    }

    [Test]
    public async Task HaveLocalStorageItemAsync_ShouldRetryUntilTheItemAppears()
    {
        await Page.EvaluateAsync("setTimeout(() => localStorage.setItem('late-key', 'late-value'), 300)");

        await Page.Should().HaveLocalStorageItemAsync("late-key", "late-value");
    }

    [Test]
    public void HaveLocalStorageItemAsync_ShouldThrow_WhenItemIsMissing()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveLocalStorageItemAsync("missing-key");
        });

        Assert.That(ex!.Message, Does.Contain("missing-key"));
        Assert.That(ex.Message, Does.Contain("the local storage is empty"));
    }

    [Test]
    public async Task HaveLocalStorageItemAsync_ShouldListStoredItems_WhenTheNamedItemIsMissing()
    {
        await Page.LocalStorage.SetItemAsync("other-key", "other-value");

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveLocalStorageItemAsync("missing-key");
        });

        Assert.That(ex!.Message, Does.Contain("other-key=other-value"));
    }

    [Test]
    public async Task HaveLocalStorageItemAsync_ShouldThrow_WhenTheValueDiffers()
    {
        await Page.LocalStorage.SetItemAsync("user-id", "42");

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveLocalStorageItemAsync("user-id", "43");
        });

        Assert.That(ex!.Message, Does.Contain("but found '42'"));
    }

    [Test]
    public void HaveLocalStorageItemAsync_ShouldIncludeBecauseInTheFailure()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveLocalStorageItemAsync("missing-key", because: "login should persist the session");
        });

        Assert.That(ex!.Message, Does.Contain("login should persist the session"));
    }

    [Test]
    public async Task Not_HaveLocalStorageItemAsync_ShouldPass_WhenItemIsAbsent()
    {
        await Page.Should().Not.HaveLocalStorageItemAsync("never-set");
    }

    [Test]
    public async Task Not_HaveLocalStorageItemAsync_ShouldPass_WhenTheValueDiffers()
    {
        await Page.LocalStorage.SetItemAsync("user-id", "42");

        await Page.Should().Not.HaveLocalStorageItemAsync("user-id", "43");
    }

    [Test]
    public async Task Not_HaveLocalStorageItemAsync_ShouldThrow_WhenItemIsPresent()
    {
        await Page.LocalStorage.SetItemAsync("user-id", "42");

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().Not.HaveLocalStorageItemAsync("user-id");
        });

        Assert.That(ex!.Message, Does.Contain("no item named 'user-id'"));
        Assert.That(ex.Message, Does.Contain("'42'"));
    }

    [Test]
    public async Task Not_HaveLocalStorageItemAsync_ShouldRetryUntilTheItemIsRemoved()
    {
        await Page.LocalStorage.SetItemAsync("logout-flag", "on");
        await Page.EvaluateAsync("setTimeout(() => localStorage.removeItem('logout-flag'), 300)");

        await Page.Should().Not.HaveLocalStorageItemAsync("logout-flag");
    }

    [Test]
    public async Task HaveSessionStorageItemAsync_ShouldPass_WhenItemExists()
    {
        await Page.SessionStorage.SetItemAsync("wizard-step", "3");

        await Page.Should().HaveSessionStorageItemAsync("wizard-step", "3");
    }

    [Test]
    public void HaveSessionStorageItemAsync_ShouldThrow_WhenItemIsMissing()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await Page.Should().HaveSessionStorageItemAsync("missing-key");
        });

        Assert.That(ex!.Message, Does.Contain("session storage"));
    }

    [Test]
    public async Task Not_HaveSessionStorageItemAsync_ShouldPass_WhenItemIsAbsent()
    {
        await Page.Should().Not.HaveSessionStorageItemAsync("never-set");
    }

    [Test]
    public async Task StorageAssertions_ShouldChain()
    {
        await Page.LocalStorage.SetItemAsync("a", "1");
        await Page.SessionStorage.SetItemAsync("b", "2");

        await Page.Should()
            .HaveLocalStorageItemAsync("a", "1")
            .HaveSessionStorageItemAsync("b", "2")
            .Not.HaveLocalStorageItemAsync("c");
    }
}
