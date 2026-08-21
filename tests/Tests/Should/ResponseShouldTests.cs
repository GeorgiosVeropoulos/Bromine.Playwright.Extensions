using Bromine.Playwright.Extensions.Assertions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Should;

/// <summary>
/// Covers the <c>.Should()</c> chain on <see cref="IResponse"/>, including
/// <c>HaveHttpVersionAsync</c> which is new in Playwright 1.59.
/// </summary>
public class ResponseShouldTests : TestBase
{
    public ResponseShouldTests(BrowserType browser) : base(browser) { }

    private async Task<IResponse> GotoAsync(string path)
    {
        var response = await Page.GotoAsync(path);
        Assert.That(response, Is.Not.Null, $"navigation to {path} produced no response");
        return response!;
    }

    // ───────────────────────── BeOKAsync ─────────────────────────

    [Test]
    public async Task BeOKAsync_ShouldPass_For200()
    {
        var response = await GotoAsync("/about.html");

        await response.Should().BeOKAsync();
    }

    [Test]
    public async Task BeOKAsync_ShouldThrow_For404()
    {
        var response = await GotoAsync("/api/not-found");

        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await response.Should().BeOKAsync();
        });
    }

    [Test]
    public async Task Not_BeOKAsync_ShouldPass_For404()
    {
        var response = await GotoAsync("/api/not-found");

        await response.Should().Not.BeOKAsync();
    }

    // ───────────────────────── HaveStatusAsync ─────────────────────────

    [Test]
    public async Task HaveStatusAsync_ShouldPass_WhenStatusMatches()
    {
        var response = await GotoAsync("/about.html");

        await response.Should().HaveStatusAsync(200);
    }

    [Test]
    public async Task HaveStatusAsync_ShouldPass_For404()
    {
        var response = await GotoAsync("/api/not-found");

        await response.Should().HaveStatusAsync(404);
    }

    [Test]
    public async Task HaveStatusAsync_ShouldThrow_WhenStatusDiffers()
    {
        var response = await GotoAsync("/about.html");

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await response.Should().HaveStatusAsync(500);
        });

        Assert.That(ex!.Message, Does.Contain("500").And.Contains("200"));
    }

    [Test]
    public async Task Not_HaveStatusAsync_ShouldPass_WhenStatusDiffers()
    {
        var response = await GotoAsync("/about.html");

        await response.Should().Not.HaveStatusAsync(500);
    }

    // ───────────────────────── HaveHttpVersionAsync ─────────────────────────

    [Test]
    public async Task HaveHttpVersionAsync_ShouldPass_ForTheReportedVersion()
    {
        var response = await GotoAsync("/about.html");

        // Read it back rather than hard-coding: the local server's protocol is not this
        // assertion's contract, the comparison is.
        var actual = await response.HttpVersionAsync();
        Assert.That(actual, Is.Not.Empty);

        await response.Should().HaveHttpVersionAsync(actual);
    }

    [Test]
    public async Task HaveHttpVersionAsync_ShouldBeCaseInsensitive()
    {
        var response = await GotoAsync("/about.html");

        var actual = await response.HttpVersionAsync();

        await response.Should().HaveHttpVersionAsync(actual.ToUpperInvariant());
        await response.Should().HaveHttpVersionAsync(actual.ToLowerInvariant());
    }

    [Test]
    public async Task HaveHttpVersionAsync_ShouldThrow_WhenVersionDiffers()
    {
        var response = await GotoAsync("/about.html");

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await response.Should().HaveHttpVersionAsync("HTTP/9.9");
        });

        Assert.That(ex!.Message, Does.Contain("HTTP/9.9"));
    }

    [Test]
    public async Task Not_HaveHttpVersionAsync_ShouldPass_WhenVersionDiffers()
    {
        var response = await GotoAsync("/about.html");

        await response.Should().Not.HaveHttpVersionAsync("HTTP/9.9");
    }

    // ───────────────────────── Because ─────────────────────────

    [Test]
    public async Task Because_ShouldIncludeMessageOnFailure()
    {
        var response = await GotoAsync("/api/not-found");

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await response.Should().BeOKAsync(because: "the about page should always resolve");
        });

        Assert.That(ex!.Message, Does.Contain("the about page should always resolve"));
    }

    [Test]
    public async Task Because_WithFormat_ShouldIncludeFormattedMessage()
    {
        var response = await GotoAsync("/about.html");

        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await response.Should().HaveStatusAsync(
                500,
                because: "the {0} endpoint was expected to fail",
                becauseArgs: ["about"]);
        });

        Assert.That(ex!.Message, Does.Contain("the about endpoint was expected to fail"));
    }

    // ───────────────────────── Chaining ─────────────────────────

    [Test]
    public async Task Chaining_ShouldPass_StatusVersionAndOk()
    {
        var response = await GotoAsync("/about.html");
        var version = await response.HttpVersionAsync();

        await response.Should()
            .BeOKAsync()
            .HaveStatusAsync(200)
            .HaveHttpVersionAsync(version);
    }

    [Test]
    public async Task Chaining_ShouldThrow_WhenSecondAssertionFails()
    {
        var response = await GotoAsync("/about.html");

        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            await response.Should()
                .BeOKAsync()
                .HaveStatusAsync(404);
        });
    }

    [Test]
    public async Task Chaining_WithNot_ShouldPass()
    {
        var response = await GotoAsync("/about.html");

        await response.Should()
            .BeOKAsync()
            .Not.HaveStatusAsync(500)
            .Not.HaveHttpVersionAsync("HTTP/9.9");
    }
}
