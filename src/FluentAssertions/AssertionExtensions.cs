using Bromine.Playwright.Extensions.Extensions;
using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Assertions;

/// <summary>
/// Entry-point extension methods that provide the <c>.Should()</c> fluent API
/// for Playwright types.
/// </summary>
public static class AssertionExtensions
{
    /// <summary>
    /// Start a fluent assertion chain on an <see cref="ILocator"/>.
    /// <example>
    /// <code>
    /// await page.Locator("#submit").Should().BeVisibleAsync();
    /// await page.Locator("#submit").Should().BeEnabledAsync();
    /// await page.Locator(".error").Should().HaveTextAsync("Invalid input");
    /// await page.Locator(".items").Should().HaveCountAsync(5);
    /// </code>
    /// </example>
    /// </summary>
    public static FluentLocatorAssertions Should(this ILocator locator) => new(locator);

    /// <summary>
    /// Start a fluent assertion chain on an <see cref="IPage"/>.
    /// <example>
    /// <code>
    /// await page.Should().HaveTitleAsync("Dashboard");
    /// await page.Should().HaveURLAsync("https://example.com/dashboard");
    /// </code>
    /// </example>
    /// </summary>
    public static FluentPageAssertions Should(this IPage page)
        => new(page);

    /// <summary>
    /// Start a fluent assertion chain on an <see cref="IAPIResponse"/>.
    /// <example>
    /// <code>
    /// var response = await request.GetAsync("/api/users");
    /// await response.Should().BeOKAsync();
    /// await response.Should().HaveStatusAsync(200);
    /// await response.Should()
    ///     .BeOKAsync()
    ///     .HaveHeaderValueAsync("Content-Type", "application/json")
    ///     .BodyContainsAsync("Alice");
    /// </code>
    /// </example>
    /// </summary>
    public static FluentAPIResponseAssertions Should(this IAPIResponse response) => new(response);

    /// <summary>
    /// Start a fluent assertion chain on an <see cref="IResponse"/> — the navigation or network
    /// response from <c>GotoAsync</c> / <c>WaitForResponseAsync</c>.
    /// <example>
    /// <code>
    /// var response = await page.GotoAsync("/dashboard");
    /// await response.Should().BeOKAsync();
    /// await response.Should()
    ///     .HaveStatusAsync(200)
    ///     .HaveHttpVersionAsync("HTTP/1.1");
    /// </code>
    /// </example>
    /// </summary>
    public static FluentResponseAssertions Should(this IResponse response) => new(response);
}

