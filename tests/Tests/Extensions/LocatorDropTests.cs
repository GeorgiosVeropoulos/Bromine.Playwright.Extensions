using Bromine.Playwright.Extensions.Assertions;
using Bromine.Playwright.Extensions.Extensions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Extensions;

/// <summary>
/// Covers the drag-and-drop helpers built on Playwright 1.60's <c>ILocator.DropAsync</c>.
/// </summary>
public class LocatorDropTests : TestBase
{
    public LocatorDropTests(BrowserType browser) : base(browser) { }

    private string _workDir = null!;

    [SetUp]
    public async Task GoToDropPage()
    {
        _workDir = Directory
            .CreateDirectory(Path.Combine(Path.GetTempPath(), $"bromine-drop-{Guid.NewGuid()}"))
            .FullName;

        await Page.GotoAsync("/drop.html");
    }

    [TearDown]
    public void RemoveWorkDir()
    {
        if (Directory.Exists(_workDir))
            Directory.Delete(_workDir, recursive: true);
    }

    private ILocator Zone => Page.Locator("[data-testid=zone]");
    private ILocator DroppedFiles => Page.Locator("[data-testid=dropped-files]");
    private ILocator DroppedTypes => Page.Locator("[data-testid=dropped-types]");
    private ILocator DroppedText => Page.Locator("[data-testid=dropped-text]");
    private ILocator DropCount => Page.Locator("[data-testid=drop-count]");

    private async Task<string> WriteTempFileAsync(string name, string content)
    {
        var path = Path.Combine(_workDir, name);
        await File.WriteAllTextAsync(path, content);
        return path;
    }

    // ───────────────────────── DropFilesAsync (paths) ─────────────────────────

    [Test]
    public async Task DropFilesAsync_ShouldDeliverFileToTheDropZone()
    {
        var path = await WriteTempFileAsync("report.txt", "twelve chars");

        await Zone.DropFilesAsync([path]);

        // "twelve chars" is 12 bytes, so name:size pins content as well as identity.
        await DroppedFiles.Should().HaveTextAsync("report.txt:12");
        await DropCount.Should().HaveTextAsync("1");
    }

    [Test]
    public async Task DropFilesAsync_ShouldDeliverMultipleFiles()
    {
        var first = await WriteTempFileAsync("one.txt", "1");
        var second = await WriteTempFileAsync("two.txt", "22");

        await Zone.DropFilesAsync([first, second]);

        await DroppedFiles.Should().HaveTextAsync("one.txt:1,two.txt:2");
    }

    [Test]
    public async Task DropFilesAsync_ShouldAcceptAPosition()
    {
        var path = await WriteTempFileAsync("positioned.txt", "xyz");

        await Zone.DropFilesAsync([path], position: new Position { X = 10, Y = 10 });

        await DroppedFiles.Should().HaveTextAsync("positioned.txt:3");
    }

    // ───────────────────────── DropFilesAsync (in-memory) ─────────────────────────

    [Test]
    public async Task DropFilesAsync_ShouldDeliverInMemoryFile_WithoutTouchingDisk()
    {
        await Zone.DropFilesAsync([
            new FilePayload
            {
                Name = "generated.csv",
                MimeType = "text/csv",
                Buffer = "a,b\n1,2"u8.ToArray()
            }
        ]);

        await DroppedFiles.Should().HaveTextAsync("generated.csv:7");
        Assert.That(Directory.GetFiles(_workDir), Is.Empty, "nothing should have been written to disk");
    }

    // ───────────────────────── DropDataAsync / DropTextAsync ─────────────────────────

    [Test]
    public async Task DropTextAsync_ShouldDeliverPlainText()
    {
        await Zone.DropTextAsync("dropped-payload");

        await DroppedText.Should().HaveTextAsync("dropped-payload");
        await DroppedTypes.Should().ContainTextAsync("text/plain");
    }

    [Test]
    public async Task DropDataAsync_ShouldDeliverMultipleMimeTypes()
    {
        await Zone.DropDataAsync([
            new KeyValuePair<string, string>("text/plain", "as-text"),
            new KeyValuePair<string, string>("text/uri-list", "https://example.com/")
        ]);

        await DroppedText.Should().HaveTextAsync("as-text");
        await DroppedTypes.Should().ContainTextAsync("text/uri-list");
    }

    [Test]
    public async Task DropDataAsync_ShouldNotDeliverFiles()
    {
        await Zone.DropDataAsync([new KeyValuePair<string, string>("text/plain", "no files here")]);

        await DroppedFiles.Should().BeEmptyAsync();
    }

    // ───────────────────────── Repeated drops ─────────────────────────

    [Test]
    public async Task DropAsync_ShouldBeRepeatable()
    {
        await Zone.DropTextAsync("first");
        await Zone.DropTextAsync("second");

        await DropCount.Should().HaveTextAsync("2");
        await DroppedText.Should().HaveTextAsync("second");
    }
}
