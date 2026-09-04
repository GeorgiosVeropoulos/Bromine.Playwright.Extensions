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
    public async Task RecordScreencastAsync_ShouldRecordAtTheRequestedSize()
    {
        var path = Path.Combine(_workDir, "sized.webm");

        try
        {
            await Page.RecordScreencastAsync(
                path,
                new ScreencastStartOptions { Size = new ScreencastSize { Width = 640, Height = 480 } },
                async () =>
                {
                    await Page.Locator("[data-testid=counter-btn]").ClickAsync();
                });
        }
        catch (PlaywrightException ex) when (IsUnsupported(ex))
        {
            Assert.Ignore($"Screencast is not supported on this engine: {ex.Message}");
            return;
        }

        // The default video size is viewport-derived (800x450 for TestBase's viewport), so the
        // header carrying the requested size proves the overload forwarded it to Playwright.
        Assert.That(ReadWebMPixelSize(path), Is.EqualTo((640, 480)));
    }

    [Test]
    public async Task StartScreencastAsync_ShouldWriteToSavePath_WhenOptionsNameAnotherPath()
    {
        var savePath = Path.Combine(_workDir, "real.webm");
        var decoyPath = Path.Combine(_workDir, "decoy.webm");
        var options = new ScreencastStartOptions { Path = decoyPath };

        try
        {
            await Page.StartScreencastAsync(savePath, options);
        }
        catch (PlaywrightException ex) when (IsUnsupported(ex))
        {
            Assert.Ignore($"Screencast is not supported on this engine: {ex.Message}");
            return;
        }

        await Page.StopScreencastAsync();

        Assert.That(File.Exists(savePath), Is.True, "the savePath argument should win over options.Path");
        Assert.That(File.Exists(decoyPath), Is.False);
        Assert.That(options.Path, Is.EqualTo(decoyPath), "the caller's options instance should not be mutated");
    }

    /// <summary>
    /// Reads the video dimensions from a WebM header: EBML encodes them as
    /// <c>PixelWidth</c> (id <c>0xB0</c>) and <c>PixelHeight</c> (id <c>0xBA</c>), each followed
    /// by a one-byte length marker <c>0x82</c> (= 2 data bytes, which covers any size this suite
    /// asks for) and the value big-endian. A linear scan is enough at this file size, and beats
    /// taking a container-parsing dependency for one assertion.
    /// </summary>
    private static (int Width, int Height) ReadWebMPixelSize(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var end = Math.Min(4096, bytes.Length) - 3;

        int Find(byte id)
        {
            for (var i = 0; i < end; i++)
                if (bytes[i] == id && bytes[i + 1] == 0x82)
                    return (bytes[i + 2] << 8) | bytes[i + 3];
            return -1;
        }

        return (Find(0xB0), Find(0xBA));
    }

    [Test]
    public async Task ShowScreencastActionsAsync_ShouldAcceptACursor()
    {
        var path = Path.Combine(_workDir, "cursor.webm");

        try
        {
            await Page.StartScreencastAsync(path);
        }
        catch (PlaywrightException ex) when (IsUnsupported(ex))
        {
            Assert.Ignore($"Screencast is not supported on this engine: {ex.Message}");
            return;
        }

        await Page.ShowScreencastActionsAsync(new ScreencastShowActionsOptions
        {
            Cursor = ScreencastCursor.Pointer,
            Position = AnnotatePosition.BottomRight
        });

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
