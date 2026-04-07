using Bromine.Playwright.Extensions.Assertions;
using Bromine.Playwright.Extensions.Extensions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Exceptions;

[TestFixture]
public class ExceptionTests : TestBase
{
    
    
    [Test]
    public void VerifyNotFoundLocatorThrowsCorrectMessage()
    {
        var invalidLocator = Page.Locator("[data-tes='null']");

        var message = Assert
            .ThrowsAsync<PlaywrightException>(async () => await invalidLocator.Should().BeVisibleAsync())?.Message;
        
        Assert.That(message, Does.Contain("Locator expected to be visible"));
    }

    [Test]
    public async Task VerifyNotFoundLocatorIsNotVisible()
    {
        var invalidLocator = Page.Locator("[data-qa='null']");
        await invalidLocator.Should().BeHiddenAsync()
            .Not.BeVisibleAsync();

        await Page.NavigateAndWaitAsync("/about.html");
        Console.WriteLine();
    }
}