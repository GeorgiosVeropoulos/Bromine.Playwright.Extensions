using Bromine.Playwright.Extensions.Assertions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Exceptions;

[TestFixture]
public class ExceptionTests : TestBase
{
    
    
    [Test]
    public void VerifyNotFoundLocatorThrowsCorrectMessage()
    {
        var invalidLocator = Page.Locator("[data-qa='null']");

        var message = Assert
            .ThrowsAsync<PlaywrightException>(async () => await invalidLocator.Should().BeVisibleAsync())?.Message;
        
        Assert.That(message, Does.Contain("Locator expected to be visible"));
    }

    [Test]
    public void VerifyNotFoundLocatorIsNotVisible()
    {
        var invalidLocator = Page.Locator("[data-qa='null']");
        invalidLocator.Should().BeHiddenAsync().BeVisibleAsync();

    }
}