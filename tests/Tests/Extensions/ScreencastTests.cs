using Bromine.Playwright.Extensions.Extensions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Extensions;

/// <summary>
/// Covers the screencast helpers built on Playwright 1.59's <c>Page.Screencast</c>.
/// </summary>
public class ScreencastTests : TestBase
{
    public ScreencastTests(BrowserType browser) : base(browser) { }

    private string _workDir = null!;

    [SetUp]
    public void CreateWorkDir()
    {
        _workDir = Path.Combine(Path.GetTempPath(), $"bromine-screencast-{Guid.NewGuid()}");
    }

    [TearDown]
    public void RemoveWorkDir()
    {
        if (Directory.Exists(_workDir))
            Directory.Delete(_workDir, recursive: true);
    }

    /// <summary>
    /// Screencast support is engine-dependent, so an explicit "not supported" from the driver
    /// skips rather than fails. Any other error is a real failure and propagates.
    /// </summary>
    private static bool IsUnsupported(PlaywrightException ex)
        => ex.Message.Contains("not supported", StringComparison.OrdinalIgnoreCase)
           || ex.Message.Contains("unsupported", StringComparison.OrdinalIgnoreCase);

    [Test]
    public async Task RecordScreencastAsync_ShouldWriteAVideoFile()
    {
        var path = Path.Combine(_workDir, "run.webm");
        string saved;

        try
        {
            saved = await Page.RecordScreencastAsync(path, async () =>
            {
                await Page.Locator("[data-testid=counter-btn]").ClickAsync();
                await Page.Locator("[data-testid=counter-btn]").ClickAsync();
            });
        }
        catch (PlaywrightException ex) when (IsUnsupported(ex))
        {
            Assert.Ignore($"Screencast is not supported on this engine: {ex.Message}");
            return;
        }

        Assert.That(saved, Is.EqualTo(path));
        Assert.That(File.Exists(path), Is.True, "the screencast should have produced a video file");
        Assert.That(new FileInfo(path).Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task StartScreencastAsync_ShouldCreateTheTargetDirectory()
    {
        var nested = Path.Combine(_workDir, "nested", "deeper");
        var path = Path.Combine(nested, "run.webm");

        try
        {
            await Page.StartScreencastAsync(path);
        }
        catch (PlaywrightException ex) when (IsUnsupported(ex))
        {
            Assert.Ignore($"Screencast is not supported on this engine: {ex.Message}");
            return;
        }

        try
        {
            Assert.That(Directory.Exists(nested), Is.True);
        }
        finally
        {
            await Page.StopScreencastAsync();
        }
    }

    [Test]
    public async Task RecordScreencastAsync_ShouldStopTheScreencast_WhenTheActionThrows()
    {
        var path = Path.Combine(_workDir, "failed.webm");

        var thrown = false;
        try
        {
            await Page.RecordScreencastAsync(path, () => throw new InvalidOperationException("boom"));
        }
        catch (InvalidOperationException)
        {
            thrown = true;
        }
        catch (PlaywrightException ex) when (IsUnsupported(ex))
        {
            Assert.Ignore($"Screencast is not supported on this engine: {ex.Message}");
            return;
        }

        Assert.That(thrown, Is.True, "the action's exception should surface to the caller");

        // Proof the screencast was stopped: starting a fresh one would fail if the first
        // were still running.
        await Page.StartScreencastAsync(Path.Combine(_workDir, "second.webm"));
        await Page.StopScreencastAsync();
    }

    [Test]
    public async Task ShowScreencastActionsAsync_ShouldAnnotateARunningScreencast()
    {
        var path = Path.Combine(_workDir, "annotated.webm");

        try
        {
            await Page.StartScreencastAsync(path);
        }
        catch (PlaywrightException ex) when (IsUnsupported(ex))
        {
            Assert.Ignore($"Screencast is not supported on this engine: {ex.Message}");
            return;
        }

        await Page.ShowScreencastActionsAsync(
            position: AnnotatePosition.BottomRight,
            durationMs: 500,
            fontSize: 18);

        await Page.Locator("[data-testid=counter-btn]").ClickAsync();

        await Page.HideScreencastActionsAsync();
        await Page.StopScreencastAsync();

        Assert.That(File.Exists(path), Is.True);
        Assert.That(new FileInfo(path).Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task ShowScreencastActionsAsync_ShouldUseDefaults_WhenNoOptionsGiven()
    {
        var path = Path.Combine(_workDir, "defaults.webm");

        try
        {
            await Page.StartScreencastAsync(path);
        }
        catch (PlaywrightException ex) when (IsUnsupported(ex))
        {
            Assert.Ignore($"Screencast is not supported on this engine: {ex.Message}");
            return;
        }

        await Page.ShowScreencastActionsAsync();
        await Page.Locator("[data-testid=counter-btn]").ClickAsync();
        await Page.HideScreencastActionsAsync();
        await Page.StopScreencastAsync();

        Assert.That(File.Exists(path), Is.True);
    }
}
