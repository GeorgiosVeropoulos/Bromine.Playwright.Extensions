using Bromine.Playwright.Extensions.Extensions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Extensions;

/// <summary>
/// Covers the helpers built on Playwright 1.59's non-blocking
/// <see cref="IRequest.ExistingResponse"/>.
/// </summary>
public class RequestExtensionsTests : TestBase
{
    public RequestExtensionsTests(BrowserType browser) : base(browser) { }

    // ───────────────────────── HasResponse ─────────────────────────

    [Test]
    public async Task HasResponse_ShouldBeTrue_ForCompletedNavigation()
    {
        var response = await Page.GotoAsync("/about.html");

        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Request.HasResponse(), Is.True);
    }

    // ───────────────────────── GetResponseAsync ─────────────────────────

    [Test]
    public async Task GetResponseAsync_ShouldReturnTheResponse_ForCompletedNavigation()
    {
        var response = await Page.GotoAsync("/about.html");

        var fromRequest = await response!.Request.GetResponseAsync();

        Assert.That(fromRequest, Is.Not.Null);
        Assert.That(fromRequest!.Url, Is.EqualTo(response.Url));
        Assert.That(fromRequest.Status, Is.EqualTo(200));
    }

    [Test]
    public async Task GetResponseAsync_ShouldReturnNull_WhenTheResponseHasNotArrivedInTime()
    {
        var pending = await StartSlowRequestAsync();

        // The endpoint sleeps for 2s, so a 200ms budget cannot be met.
        var response = await pending.GetResponseAsync(timeoutMs: 200);

        Assert.That(response, Is.Null);
    }

    [Test]
    public async Task HasResponse_ShouldBeFalse_WhileTheRequestIsStillInFlight()
    {
        var pending = await StartSlowRequestAsync();

        Assert.That(pending.HasResponse(), Is.False);
    }

    [Test]
    public async Task GetResponseAsync_ShouldReturnTheResponse_WhenGivenEnoughTime()
    {
        var pending = await StartSlowRequestAsync();

        var response = await pending.GetResponseAsync(timeoutMs: 10_000);

        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Status, Is.EqualTo(200));
    }

    /// <summary>
    /// Fire a request at the deliberately slow endpoint and return it as soon as the browser
    /// reports it, while its response is still outstanding.
    /// </summary>
    private async Task<IRequest> StartSlowRequestAsync()
    {
        var observed = new TaskCompletionSource<IRequest>();

        void OnRequest(object? sender, IRequest request)
        {
            if (request.Url.Contains("/api/slow", StringComparison.Ordinal))
                observed.TrySetResult(request);
        }

        Page.Request += OnRequest;
        try
        {
            // Not awaited: the fetch only settles after the endpoint's 2s delay.
            _ = Page.EvaluateAsync("fetch('/api/slow')");

            var finished = await Task.WhenAny(observed.Task, Task.Delay(10_000));
            Assert.That(finished, Is.SameAs(observed.Task), "the slow request was never observed");

            return await observed.Task;
        }
        finally
        {
            Page.Request -= OnRequest;
        }
    }
}
