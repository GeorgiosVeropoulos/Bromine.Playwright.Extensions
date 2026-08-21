using Bromine.Playwright.Extensions.Reason;
using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Extensions;

/// <summary>
/// Fluent assertions for a navigation or network <see cref="IResponse"/> — the type returned by
/// <c>Page.GotoAsync</c> and <c>Page.WaitForResponseAsync</c>.
/// <para>
/// Distinct from <see cref="FluentAPIResponseAssertions"/>, which covers
/// <see cref="IAPIResponse"/> from the API request context. Playwright ships no built-in
/// <c>Expect(IResponse)</c>, so every assertion here is evaluated directly.
/// </para>
/// </summary>
public class FluentResponseAssertions : FluentBase<FluentResponseAssertions>
{
    private readonly IResponse _response;

    public FluentResponseAssertions(IResponse response, bool negateNext = false)
    {
        _response = response;
        NegateNext = negateNext;
    }

    /// <summary>
    /// Asserts that the response was served over the expected HTTP version, e.g. <c>"HTTP/1.1"</c>
    /// or <c>"h2"</c>. Compared case-insensitively. Requires Playwright 1.59 or newer.
    /// </summary>
    public FluentResponseAssertions HaveHttpVersionAsync(string expectedVersion, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(async () =>
        {
            var actual = await _response.HttpVersionAsync();
            var matches = string.Equals(actual, expectedVersion, StringComparison.OrdinalIgnoreCase);

            if (negate ? matches : !matches)
            {
                var expectation = negate ? "not to be served over" : "to be served over";
                throw new PlaywrightException(
                    $"Expected response {expectation} '{expectedVersion}', but got '{actual}'. URL: {_response.Url}");
            }
        }, new Because(because, becauseArgs));
        return this;
    }

    /// <summary>
    /// Asserts that the response has the expected status code.
    /// </summary>
    public FluentResponseAssertions HaveStatusAsync(int expectedStatus, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() =>
        {
            var matches = _response.Status == expectedStatus;
            if (negate ? matches : !matches)
            {
                var expectation = negate ? "not to have" : "to have";
                throw new PlaywrightException(
                    $"Expected response {expectation} status {expectedStatus}, but got {_response.Status}. URL: {_response.Url}");
            }
            return Task.CompletedTask;
        }, new Because(because, becauseArgs));
        return this;
    }

    /// <summary>
    /// Asserts that the response status is in the 2xx range.
    /// </summary>
    public FluentResponseAssertions BeOKAsync(string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() =>
        {
            var ok = _response.Ok;
            if (negate ? ok : !ok)
            {
                var expectation = negate ? "not to be OK" : "to be OK";
                throw new PlaywrightException(
                    $"Expected response {expectation}, but status was {_response.Status} ({_response.StatusText}). URL: {_response.Url}");
            }
            return Task.CompletedTask;
        }, new Because(because, becauseArgs));
        return this;
    }
}
