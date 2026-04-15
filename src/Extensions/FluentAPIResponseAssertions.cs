using Bromine.Playwright.Extensions.Reason;
using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Extensions;

public class FluentAPIResponseAssertions : FluentBase<FluentAPIResponseAssertions>
{
    
    private readonly IAPIResponse _response;
    
    
    public FluentAPIResponseAssertions(IAPIResponse response, bool negateNext = false)
    {
        _response = response;
        NegateNext = negateNext;
    }
    
    /// <summary>
    /// Asserts that the API response has an OK status (2xx).
    /// See <see cref="IAPIResponseAssertions.ToBeOKAsync"/>.
    /// </summary>
    public FluentAPIResponseAssertions BeOKAsync(string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_response).Not.ToBeOKAsync()
            : Expect(_response).ToBeOKAsync(), 
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the API response has the expected status code.
    /// </summary>
    public FluentAPIResponseAssertions HaveStatusAsync(int expectedStatus, string because = "", params object[] becauseArgs)
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
    /// Asserts that the API response has a specific header present.
    /// </summary>
    public FluentAPIResponseAssertions HaveHeaderAsync(string headerName, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() =>
        {
            var exists = _response.Headers.ContainsKey(headerName.ToLowerInvariant());
            if (negate ? exists : !exists)
            {
                var expectation = negate ? "not to have" : "to have";
                throw new PlaywrightException(
                    $"Expected response {expectation} header '{headerName}'. URL: {_response.Url}");
            }
            return Task.CompletedTask;
        }, new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the API response has a specific header with the expected value.
    /// </summary>
    public FluentAPIResponseAssertions HaveHeaderValueAsync(string headerName, string expectedValue, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() =>
        {
            var headers = _response.Headers;
            var key = headerName.ToLowerInvariant();
            var exists = headers.ContainsKey(key);
            var matches = exists && string.Equals(headers[key], expectedValue, StringComparison.OrdinalIgnoreCase);
            
            if (negate ? matches : !matches)
            {
                if (negate)
                {
                    throw new PlaywrightException(
                        $"Expected header '{headerName}' not to have value '{expectedValue}', but it did. URL: {_response.Url}");
                }
                if (!exists)
                {
                    throw new PlaywrightException(
                        $"Expected response to have header '{headerName}', but it was not found. URL: {_response.Url}");
                }
                throw new PlaywrightException(
                    $"Expected header '{headerName}' to have value '{expectedValue}', but got '{headers[key]}'. URL: {_response.Url}");
            }
            return Task.CompletedTask;
        }, new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the API response body contains the expected substring.
    /// </summary>
    public FluentAPIResponseAssertions BodyContainsAsync(string expected, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(async () =>
        {
            var body = await _response.TextAsync();
            var contains = body.Contains(expected, StringComparison.Ordinal);
            if (negate ? contains : !contains)
            {
                var expectation = negate ? "not to contain" : "to contain";
                throw new PlaywrightException(
                    $"Expected response body {expectation} '{expected}', but it did{(negate ? "" : " not")}. URL: {_response.Url}");
            }
        }, new Because(because, becauseArgs));
        return this;
    }
    
}