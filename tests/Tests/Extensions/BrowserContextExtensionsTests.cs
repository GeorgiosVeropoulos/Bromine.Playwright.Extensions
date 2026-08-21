using Bromine.Playwright.Extensions.Builders;
using Bromine.Playwright.Extensions.Extensions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Extensions;

/// <summary>
/// Covers the context helpers built on Playwright 1.59's <c>IsClosed</c> and
/// <c>SetStorageStateAsync</c>.
/// </summary>
public class BrowserContextExtensionsTests : TestBase
{
    public BrowserContextExtensionsTests(BrowserType browser) : base(browser) { }

    private string _workDir = null!;

    [SetUp]
    public void CreateWorkDir()
    {
        _workDir = Directory
            .CreateDirectory(Path.Combine(Path.GetTempPath(), $"bromine-storage-{Guid.NewGuid()}"))
            .FullName;
    }

    [TearDown]
    public void RemoveWorkDir()
    {
        if (Directory.Exists(_workDir))
            Directory.Delete(_workDir, recursive: true);
    }

    // ───────────────────────── IsOpen / IsClosed ─────────────────────────

    [Test]
    public void IsOpen_ShouldBeTrue_ForLiveContext()
    {
        Assert.That(Context.IsOpen(), Is.True);
        Assert.That(Context.IsClosed, Is.False);
    }

    [Test]
    public async Task IsOpen_ShouldBeFalse_AfterClose()
    {
        // A context of its own, so closing it cannot disturb the fixture's teardown.
        var context = await BrowserContextBuilder.For(Browser).BuildAsync();

        Assert.That(context.IsOpen(), Is.True);

        await context.CloseAsync();

        Assert.That(context.IsClosed, Is.True);
        Assert.That(context.IsOpen(), Is.False);
    }

    // ───────────────────────── SwitchStorageStateAsync ─────────────────────────

    [Test]
    public async Task SwitchStorageStateAsync_ShouldApplyCookiesFromFile()
    {
        await Page.SetCookieAsync("session", "abc123");

        var statePath = Path.Combine(_workDir, "state.json");
        await Context.StorageStateAsync(new BrowserContextStorageStateOptions { Path = statePath });

        var other = await BrowserContextBuilder.For(Browser)
            .WithBaseUrl(TestServerFixture.BaseUrl)
            .BuildAsync();

        try
        {
            var before = await other.CookiesAsync();
            Assert.That(before.Any(c => c.Name == "session"), Is.False, "the fresh context should start clean");

            await other.SwitchStorageStateAsync(statePath);

            var after = await other.CookiesAsync();
            Assert.That(after.Any(c => c.Name == "session" && c.Value == "abc123"), Is.True);
        }
        finally
        {
            await other.CloseAsync();
        }
    }

    [Test]
    public async Task SwitchStorageStateAsync_ShouldReplaceExistingCookies()
    {
        // Empty state file: applying it must clear what the context already had.
        var emptyState = Path.Combine(_workDir, "empty.json");
        var fresh = await BrowserContextBuilder.For(Browser).BuildAsync();
        await fresh.StorageStateAsync(new BrowserContextStorageStateOptions { Path = emptyState });
        await fresh.CloseAsync();

        await Page.SetCookieAsync("session", "should-be-cleared");
        Assert.That((await Context.CookiesAsync()).Any(c => c.Name == "session"), Is.True);

        await Context.SwitchStorageStateAsync(emptyState);

        Assert.That((await Context.CookiesAsync()).Any(c => c.Name == "session"), Is.False);
    }

    [Test]
    public void SwitchStorageStateAsync_ShouldThrow_WhenFileIsMissing()
    {
        var missing = Path.Combine(_workDir, "does-not-exist.json");

        var ex = Assert.ThrowsAsync<FileNotFoundException>(async () =>
        {
            await Context.SwitchStorageStateAsync(missing);
        });

        Assert.That(ex!.Message, Does.Contain("does-not-exist.json"));
    }

    [Test]
    public async Task SwitchStorageStateAsync_ShouldThrow_WhenContextIsClosed()
    {
        var statePath = Path.Combine(_workDir, "state.json");
        await Context.StorageStateAsync(new BrowserContextStorageStateOptions { Path = statePath });

        var closed = await BrowserContextBuilder.For(Browser).BuildAsync();
        await closed.CloseAsync();

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await closed.SwitchStorageStateAsync(statePath);
        });

        Assert.That(ex!.Message, Does.Contain("closed"));
    }
}
