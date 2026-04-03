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
    /// await page.Locator("#submit").Should().BeInteractableAsync();
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
    /// await page.Should().HaveUrlContainingAsync("/dashboard");
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
    /// await response.Should().BeOkAsync();
    /// response.Should().HaveStatus(200);
    /// </code>
    /// </example>
    /// </summary>
    // public static ResponseAssertionBuilder Should(this IAPIResponse response)
    // => new(response);

    public static ResponseAssertionBuilder Should(this IAPIResponse response) => new(response);
}

