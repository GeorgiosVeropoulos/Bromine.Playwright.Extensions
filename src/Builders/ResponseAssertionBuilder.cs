using Microsoft.Playwright;
using Bromine.Playwright.Extensions.Configuration;
using PlaywrightAssertions = Microsoft.Playwright.Assertions;

namespace Bromine.Playwright.Extensions.Assertions;

/// <summary>
/// Fluent assertion wrapper for <see cref="IAPIResponse"/>.
/// Obtained via <c>response.Should()</c>.
/// </summary>
public class ResponseAssertionBuilder
{
    private readonly IAPIResponse _response;
    private float? _timeout;

    internal ResponseAssertionBuilder(IAPIResponse response)
    {
        _response = response;
    }

    /// <summary>
    /// Override the default assertion timeout for this assertion chain.
    /// </summary>
    public ResponseAssertionBuilder WithTimeout(float timeoutMs)
    {
        _timeout = timeoutMs;
        return this;
    }

    private float Timeout => _timeout ?? PlaywrightDefaults.AssertionTimeout;

    /// <summary>
    /// Assert that the response status is OK (2xx).
    /// </summary>
    public async Task<ResponseAssertionBuilder> BeOkAsync()
    {
        await PlaywrightAssertions.Expect(_response)
            .ToBeOKAsync();
        return this;
    }

    /// <summary>
    /// Assert that the response is NOT OK (non-2xx).
    /// </summary>
    public async Task<ResponseAssertionBuilder> NotBeOkAsync()
    {
        await PlaywrightAssertions.Expect(_response).Not
            .ToBeOKAsync();
        return this;
    }

    /// <summary>
    /// Assert that the response has the expected status code.
    /// </summary>
    public ResponseAssertionBuilder HaveStatus(int expectedStatus)
    {
        if (_response.Status != expectedStatus)
        {
            throw new PlaywrightException(
                $"Expected response status {expectedStatus}, but got {_response.Status}. URL: {_response.Url}");
        }
        return this;
    }

    /// <summary>
    /// Assert that the response has a specific header.
    /// </summary>
    public ResponseAssertionBuilder HaveHeader(string headerName)
    {
        var headers = _response.Headers;
        if (!headers.ContainsKey(headerName.ToLowerInvariant()))
        {
            throw new PlaywrightException(
                $"Expected response to have header '{headerName}', but it was not found. URL: {_response.Url}");
        }
        return this;
    }

    /// <summary>
    /// Assert that the response has a specific header with the expected value.
    /// </summary>
    public ResponseAssertionBuilder HaveHeader(string headerName, string expectedValue)
    {
        HaveHeader(headerName);
        var actual = _response.Headers[headerName.ToLowerInvariant()];
        if (!string.Equals(actual, expectedValue, StringComparison.OrdinalIgnoreCase))
        {
            throw new PlaywrightException(
                $"Expected header '{headerName}' to have value '{expectedValue}', but got '{actual}'. URL: {_response.Url}");
        }
        return this;
    }

    /// <summary>
    /// Assert that the response body contains the expected substring.
    /// </summary>
    public async Task<ResponseAssertionBuilder> BodyContainsAsync(string expected)
    {
        var body = await _response.TextAsync();
        if (!body.Contains(expected, StringComparison.Ordinal))
        {
            throw new PlaywrightException(
                $"Expected response body to contain '{expected}', but it did not. URL: {_response.Url}");
        }
        return this;
    }
}

