using Bromine.Playwright.Extensions.Assertions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests;

[TestFixture]
public class ResponseAssertionBuilderTests : ApiTestBase
{
    // ───────────────────────── BeOKAsync ─────────────────────────

    [Test]
    public async Task BeOKAsync_ShouldPass_WhenResponseIs200()
    {
        var response = await Request.GetAsync("/api/ok");

        await response.Should().BeOKAsync();
    }

    [Test]
    public async Task BeOKAsync_ShouldPass_ForStaticFile()
    {
        var response = await Request.GetAsync("/index.html");

        await response.Should().BeOKAsync();
    }

    [Test]
    public void BeOKAsync_ShouldThrow_WhenResponseIs404()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/not-found");
            await response.Should().BeOKAsync();
        });
    }

    [Test]
    public void BeOKAsync_ShouldThrow_WhenResponseIs500()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/error");
            await response.Should().BeOKAsync();
        });
    }

    // ───────────────────────── Not.BeOKAsync ─────────────────────────

    [Test]
    public async Task Not_BeOKAsync_ShouldPass_WhenResponseIs404()
    {
        var response = await Request.GetAsync("/api/not-found");

        await response.Should().Not.BeOKAsync();
    }

    [Test]
    public async Task Not_BeOKAsync_ShouldPass_WhenResponseIs500()
    {
        var response = await Request.GetAsync("/api/error");

        await response.Should().Not.BeOKAsync();
    }

    [Test]
    public void Not_BeOKAsync_ShouldThrow_WhenResponseIs200()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().Not.BeOKAsync();
        });
    }

    // ───────────────────────── HaveStatusAsync ─────────────────────────

    [Test]
    public async Task HaveStatusAsync_ShouldPass_WhenStatusMatches200()
    {
        var response = await Request.GetAsync("/api/ok");

        await response.Should().HaveStatusAsync(200);
    }

    [Test]
    public async Task HaveStatusAsync_ShouldPass_WhenStatusMatches404()
    {
        var response = await Request.GetAsync("/api/not-found");
        
        await response.Should()
            .Not.HaveStatusAsync(200)
            .HaveStatusAsync(404);
    }

    [Test]
    public async Task HaveStatusAsync_ShouldPass_WhenStatusMatches500()
    {
        var response = await Request.GetAsync("/api/error");

        await response.Should().HaveStatusAsync(500);
    }

    [Test]
    public void HaveStatusAsync_ShouldThrow_WhenStatusDoesNotMatch()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().HaveStatusAsync(404);
        });
    }

    [Test]
    public void HaveStatusAsync_ShouldIncludeExpectedAndActualInMessage()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().HaveStatusAsync(500);
        });

        Assert.That(ex!.Message, Does.Contain("500"));
        Assert.That(ex.Message, Does.Contain("200"));
    }

    // ───────────────────────── HaveHeaderAsync (existence) ─────────────────────────

    [Test]
    public async Task HaveHeaderAsync_ShouldPass_WhenHeaderExists()
    {
        var response = await Request.GetAsync("/api/ok");

        await response.Should().HaveHeaderAsync("content-type");
    }

    [Test]
    public async Task HaveHeaderAsync_ShouldPass_CaseInsensitive()
    {
        var response = await Request.GetAsync("/api/ok");

        await response.Should().HaveHeaderAsync("Content-Type");
    }

    [Test]
    public async Task HaveHeaderAsync_ShouldPass_WhenCustomHeaderExists()
    {
        var response = await Request.GetAsync("/api/custom-header");

        await response.Should().HaveHeaderAsync("X-Custom-Header");
    }

    [Test]
    public void HaveHeaderAsync_ShouldThrow_WhenHeaderDoesNotExist()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().HaveHeaderAsync("X-Non-Existent");
        });

        Assert.That(ex!.Message, Does.Contain("X-Non-Existent"));
    }

    // ───────────────────────── HaveHeaderAsync (with value) ─────────────────────────

    [Test]
    public async Task HaveHeaderValueAsync_ShouldPass_WhenHeaderValueMatches()
    {
        var response = await Request.GetAsync("/api/custom-header");

        await response.Should().HaveHeaderValueAsync("X-Custom-Header", "custom-value");
    }

    [Test]
    public void HaveHeaderValueAsync_ShouldThrow_WhenHeaderValueDoesNotMatch()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/custom-header");
            await response.Should().HaveHeaderValueAsync("X-Custom-Header", "wrong-value");
        });

        Assert.That(ex!.Message, Does.Contain("wrong-value"));
    }

    [Test]
    public void HaveHeaderValueAsync_ShouldThrow_WhenHeaderDoesNotExist()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().HaveHeaderValueAsync("X-Non-Existent", "any-value");
        });
    }

    // ───────────────────────── BodyContainsAsync ─────────────────────────

    [Test]
    public async Task BodyContainsAsync_ShouldPass_WhenBodyContainsSubstring()
    {
        var response = await Request.GetAsync("/api/ok");

        await response.Should().BodyContainsAsync("success");
    }

    [Test]
    public async Task BodyContainsAsync_ShouldPass_WhenBodyContainsJsonValue()
    {
        var response = await Request.GetAsync("/api/ok");

        await response.Should().BodyContainsAsync("all good");
    }

    [Test]
    public async Task BodyContainsAsync_ShouldPass_WhenBodyContainsUserData()
    {
        var response = await Request.GetAsync("/api/users");

        await response.Should().BodyContainsAsync("Alice");
    }

    [Test]
    public void BodyContainsAsync_ShouldThrow_WhenBodyDoesNotContainSubstring()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().BodyContainsAsync("this text is not in the response");
        });

        Assert.That(ex!.Message, Does.Contain("this text is not in the response"));
    }

    // ───────────────────────── Chaining ─────────────────────────

    [Test]
    public async Task Chaining_ShouldPass_WhenMultipleAssertionsAreTrue()
    {
        var response = await Request.GetAsync("/api/ok");

        await response.Should()
            .HaveStatusAsync(200)
            .HaveHeaderAsync("content-type")
            .BeOKAsync();
    }

    [Test]
    public async Task Chaining_ShouldPass_BeOKThenBodyContains()
    {
        var response = await Request.GetAsync("/api/users");

        await response.Should()
            .BeOKAsync()
            .HaveStatusAsync(200)
            .BodyContainsAsync("Bob");
    }

    [Test]
    public async Task Chaining_ShouldPass_FullChain()
    {
        var response = await Request.GetAsync("/api/custom-header");

        await response.Should()
            .BeOKAsync()
            .HaveStatusAsync(200)
            .HaveHeaderValueAsync("X-Custom-Header", "custom-value")
            .BodyContainsAsync("header");
    }

    // ───────────────────────── Not (negation) ─────────────────────────

    [Test]
    public async Task Not_BeOKAsync_InChain_ShouldPass()
    {
        var response = await Request.GetAsync("/api/not-found");

        await response.Should()
            .Not.BeOKAsync()
            .HaveStatusAsync(404);
    }

    [Test]
    public async Task Not_HaveStatusAsync_ShouldPass_WhenStatusDoesNotMatch()
    {
        var response = await Request.GetAsync("/api/ok");

        await response.Should().Not.HaveStatusAsync(404);
    }

    [Test]
    public void Not_HaveStatusAsync_ShouldThrow_WhenStatusMatches()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().Not.HaveStatusAsync(200);
        });
    }

    [Test]
    public async Task Not_HaveHeaderAsync_ShouldPass_WhenHeaderDoesNotExist()
    {
        var response = await Request.GetAsync("/api/ok");

        await response.Should().Not.HaveHeaderAsync("X-Non-Existent");
    }

    [Test]
    public void Not_HaveHeaderAsync_ShouldThrow_WhenHeaderExists()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().Not.HaveHeaderAsync("content-type");
        });
    }

    [Test]
    public async Task Not_HaveHeaderValueAsync_ShouldPass_WhenValueDoesNotMatch()
    {
        var response = await Request.GetAsync("/api/custom-header");

        await response.Should().Not.HaveHeaderValueAsync("X-Custom-Header", "wrong-value");
    }

    [Test]
    public void Not_HaveHeaderValueAsync_ShouldThrow_WhenValueMatches()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/custom-header");
            await response.Should().Not.HaveHeaderValueAsync("X-Custom-Header", "custom-value");
        });
    }

    [Test]
    public async Task Not_BodyContainsAsync_ShouldPass_WhenBodyDoesNotContain()
    {
        var response = await Request.GetAsync("/api/ok");

        await response.Should().Not.BodyContainsAsync("this text is not present");
    }

    [Test]
    public void Not_BodyContainsAsync_ShouldThrow_WhenBodyContains()
    {
        Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().Not.BodyContainsAsync("success");
        });
    }

    // ───────────────────────── Because ─────────────────────────

    [Test]
    public void Because_ShouldIncludeMessageOnFailure()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/not-found");
            await response.Should().BeOKAsync(because: "the API should return success");
        });

        Assert.That(ex!.Message, Does.Contain("the API should return success"));
    }

    [Test]
    public void Because_WithFormat_ShouldIncludeFormattedMessage()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().HaveStatusAsync(404, because: "endpoint {0} should return {1}", becauseArgs: ["/api/ok", 404]);
        });

        Assert.That(ex!.Message, Does.Contain("endpoint /api/ok should return 404"));
    }

    // ───────────────────────── Error Messages ─────────────────────────

    [Test]
    public void HaveStatusAsync_ErrorMessage_ShouldContainUrl()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().HaveStatusAsync(999);
        });

        Assert.That(ex!.Message, Does.Contain("/api/ok"));
    }

    [Test]
    public void HaveHeaderAsync_ErrorMessage_ShouldContainUrl()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().HaveHeaderAsync("X-Missing");
        });

        Assert.That(ex!.Message, Does.Contain("/api/ok"));
    }

    [Test]
    public void BodyContainsAsync_ErrorMessage_ShouldContainUrl()
    {
        var ex = Assert.ThrowsAsync<PlaywrightException>(async () =>
        {
            var response = await Request.GetAsync("/api/ok");
            await response.Should().BodyContainsAsync("nope");
        });

        Assert.That(ex!.Message, Does.Contain("/api/ok"));
    }
}

