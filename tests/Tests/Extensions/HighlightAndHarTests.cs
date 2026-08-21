using Bromine.Playwright.Extensions.Extensions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Extensions;

/// <summary>
/// Covers the highlight and mid-test HAR helpers built on Playwright 1.60's
/// <c>Locator.HighlightAsync</c> and <c>Tracing.StartHarAsync</c>.
/// </summary>
public class HighlightAndHarTests : TestBase
{
    public HighlightAndHarTests(BrowserType browser) : base(browser) { }

    private string _workDir = null!;

    [SetUp]
    public void CreateWorkDir()
    {
        _workDir = Path.Combine(Path.GetTempPath(), $"bromine-har-{Guid.NewGuid()}");
    }

    [TearDown]
    public void RemoveWorkDir()
    {
        if (Directory.Exists(_workDir))
            Directory.Delete(_workDir, recursive: true);
    }

    // ───────────────────────── HighlightWhileAsync ─────────────────────────

    [Test]
    public async Task HighlightWhileAsync_ShouldRunTheAction()
    {
        var ran = false;

        await Page.Locator("[data-testid=heading]").HighlightWhileAsync(() =>
        {
            ran = true;
            return Task.CompletedTask;
        });

        Assert.That(ran, Is.True);
    }

    [Test]
    public async Task HighlightWhileAsync_ShouldAcceptACustomStyle()
    {
        await Page.Locator("[data-testid=heading]").HighlightWhileAsync(
            () => Page.Locator("[data-testid=counter-btn]").ClickAsync(),
            style: "outline: 3px solid magenta");

        // The click inside the scope must still have happened.
        Assert.That(await Page.Locator("[data-testid=counter-value]").TextContentAsync(), Is.EqualTo("1"));
    }

    [Test]
    public async Task HighlightWhileAsync_ShouldRemoveTheHighlight_WhenTheActionThrows()
    {
        var locator = Page.Locator("[data-testid=heading]");

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await locator.HighlightWhileAsync(() => throw new InvalidOperationException("boom"));
        });

        // Proof the highlight was torn down: a second scope can be opened and closed cleanly.
        await locator.HighlightWhileAsync(() => Task.CompletedTask);
    }

    // ───────────────────────── HAR recording ─────────────────────────

    [Test]
    public async Task RecordHarAsync_ShouldWriteAHarFileCoveringTheAction()
    {
        var harPath = Path.Combine(_workDir, "nested", "traffic.har");

        var returned = await Context.RecordHarAsync(harPath, async () =>
        {
            await Page.GotoAsync("/about.html");
        });

        Assert.That(returned, Is.EqualTo(harPath));
        Assert.That(File.Exists(harPath), Is.True, "the directory should have been created and the HAR written");
        Assert.That(new FileInfo(harPath).Length, Is.GreaterThan(0));

        var har = await File.ReadAllTextAsync(harPath);
        Assert.That(har, Does.Contain("about.html"), "the HAR should contain the request made inside the scope");
    }

    [Test]
    public async Task RecordHarAsync_ShouldStopRecording_WhenTheActionThrows()
    {
        var harPath = Path.Combine(_workDir, "failed.har");

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await Context.RecordHarAsync(harPath, () => throw new InvalidOperationException("boom"));
        });

        // Proof recording stopped: a second recording can be started and stopped.
        await Context.RecordHarAsync(Path.Combine(_workDir, "second.har"), () => Page.GotoAsync("/about.html"));

        Assert.That(File.Exists(Path.Combine(_workDir, "second.har")), Is.True);
    }

    [Test]
    public async Task StartAndStopHarRecordingAsync_ShouldExcludeTrafficOutsideTheWindow()
    {
        var harPath = Path.Combine(_workDir, "windowed.har");

        // Before the window — must not appear.
        await Page.GotoAsync("/contact.html");

        await Context.StartHarRecordingAsync(harPath, content: HarContentPolicy.Omit);
        await Page.GotoAsync("/about.html");
        await Context.StopHarRecordingAsync();

        var har = await File.ReadAllTextAsync(harPath);

        Assert.That(har, Does.Contain("about.html"));
        Assert.That(har, Does.Not.Contain("contact.html"));
    }

    [Test]
    public async Task StartHarRecordingAsync_ShouldForwardTheUrlFilter()
    {
        var harPath = Path.Combine(_workDir, "filtered.har");

        // Guards our option mapping, not Playwright's filter engine: TracingStartHarOptions has
        // three filter fields (UrlFilter, UrlFilterString, UrlFilterRegex) and putting the value
        // in the wrong one would silently capture everything.
        await Context.StartHarRecordingAsync(harPath, urlFilter: "**/about.html");
        await Page.GotoAsync("/about.html");
        await Page.GotoAsync("/contact.html");
        await Context.StopHarRecordingAsync();

        var har = await File.ReadAllTextAsync(harPath);

        Assert.That(har, Does.Contain("about.html"));
        Assert.That(har, Does.Not.Contain("contact.html"), "the filter argument should have taken effect");
    }
}
