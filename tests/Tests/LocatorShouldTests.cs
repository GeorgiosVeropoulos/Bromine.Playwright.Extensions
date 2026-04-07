using System.Text.RegularExpressions;
using Bromine.Playwright.Extensions.Assertions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests;

[TestFixture]
public class LocatorShouldTests : TestBase
{
    // ───────────────────────── Visibility / Attachment ─────────────────────────

    [Test]
    public async Task BeVisibleAsync_ShouldPass_WhenElementIsVisible()
    {
        await Page.Locator("[data-testid='visible-element']").Should().BeVisibleAsync();
    }

    [Test]
    public async Task BeVisibleAsync_Not_ShouldPass_WhenElementIsHidden()
    {
        await Page.Locator("[data-testid='hidden-element']").Should().Not.BeVisibleAsync();
    }

    [Test]
    public async Task BeHiddenAsync_ShouldPass_WhenElementIsHidden()
    {
        await Page.Locator("[data-testid='hidden-element']").Should().BeHiddenAsync();
    }

    [Test]
    public async Task BeHiddenAsync_Not_ShouldPass_WhenElementIsVisible()
    {
        await Page.Locator("[data-testid='visible-element']").Should().Not.BeHiddenAsync();
    }

    [Test]
    public async Task ToBeAttachedAsync_ShouldPass_WhenElementExists()
    {
        await Page.Locator("[data-testid='heading']").Should().ToBeAttachedAsync();
    }

    [Test]
    public async Task ToBeAttachedAsync_Not_ShouldPass_WhenElementDoesNotExist()
    {
        await Page.Locator("[data-testid='nonexistent']").Should().Not.ToBeAttachedAsync();
    }

    [Test]
    public async Task BeInViewportAsync_ShouldPass_WhenElementIsInViewport()
    {
        await Page.Locator("[data-testid='in-viewport']").Should().BeInViewportAsync();
    }

    [Test]
    public async Task BeInViewportAsync_Not_ShouldPass_WhenElementIsOffScreen()
    {
        await Page.Locator("[data-testid='off-screen']").Should().Not.BeInViewportAsync();
    }

    // ───────────────────────── Enabled / Disabled / Editable ─────────────────────────

    [Test]
    public async Task BeEnabledAsync_ShouldPass_WhenInputIsEnabled()
    {
        await Page.Locator("[data-testid='enabled-input']").Should().BeEnabledAsync();
    }

    [Test]
    public async Task BeEnabledAsync_Not_ShouldPass_WhenInputIsDisabled()
    {
        await Page.Locator("[data-testid='disabled-input']").Should().Not.BeEnabledAsync();
    }

    [Test]
    public async Task BeDisabledAsync_ShouldPass_WhenInputIsDisabled()
    {
        await Page.Locator("[data-testid='disabled-input']").Should().BeDisabledAsync();
    }

    [Test]
    public async Task BeDisabledAsync_Not_ShouldPass_WhenInputIsEnabled()
    {
        await Page.Locator("[data-testid='enabled-input']").Should().Not.BeDisabledAsync();
    }

    [Test]
    public async Task BeEditableAsync_ShouldPass_WhenInputIsEditable()
    {
        await Page.Locator("[data-testid='enabled-input']").Should().BeEditableAsync();
    }

    [Test]
    public async Task BeEditableAsync_Not_ShouldPass_WhenInputIsDisabled()
    {
        await Page.Locator("[data-testid='disabled-input']").Should().Not.BeEditableAsync();
    }

    // ───────────────────────── Empty ─────────────────────────

    [Test]
    public async Task BeEmptyAsync_ShouldPass_WhenInputIsEmpty()
    {
        await Page.Locator("[data-testid='empty-input']").Should().BeEmptyAsync();
    }

    [Test]
    public async Task BeEmptyAsync_Not_ShouldPass_WhenInputHasValue()
    {
        await Page.Locator("[data-testid='enabled-input']").Should().Not.BeEmptyAsync();
    }

    // ───────────────────────── Checked ─────────────────────────

    [Test]
    public async Task ToBeCheckedAsync_ShouldPass_WhenCheckboxIsChecked()
    {
        await Page.Locator("[data-testid='checked-checkbox']").Should().ToBeCheckedAsync();
    }

    [Test]
    public async Task ToBeCheckedAsync_Not_ShouldPass_WhenCheckboxIsUnchecked()
    {
        await Page.Locator("[data-testid='unchecked-checkbox']").Should().Not.ToBeCheckedAsync();
    }

    // ───────────────────────── Focused ─────────────────────────

    [Test]
    public async Task BeFocusedAsync_ShouldPass_WhenElementHasAutofocus()
    {
        await Page.Locator("[data-testid='autofocus-input']").Should().BeFocusedAsync();
    }

    [Test]
    public async Task BeFocusedAsync_Not_ShouldPass_WhenElementIsNotFocused()
    {
        await Page.Locator("[data-testid='enabled-input']").Should().Not.BeFocusedAsync();
    }

    // ───────────────────────── Text ─────────────────────────

    [Test]
    public async Task HaveTextAsync_String_ShouldPass_WhenTextMatches()
    {
        await Page.Locator("[data-testid='paragraph']").Should().HaveTextAsync("Hello World");
    }

    [Test]
    public async Task HaveTextAsync_String_Not_ShouldPass_WhenTextDoesNotMatch()
    {
        await Page.Locator("[data-testid='paragraph']").Should().Not.HaveTextAsync("Wrong text");
    }

    [Test]
    public async Task HaveTextAsync_Regex_ShouldPass_WhenTextMatchesPattern()
    {
        await Page.Locator("[data-testid='paragraph']").Should().HaveTextAsync(new Regex("Hello.*"));
    }

    [Test]
    public async Task HaveTextAsync_Enumerable_ShouldPass_ForMultipleElements()
    {
        await Page.Locator("[data-testid='list-item']").Should()
            .HaveTextAsync(new[] { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5" });
    }

    // ───────────────────────── Value ─────────────────────────

    [Test]
    public async Task HaveValueAsync_String_ShouldPass_WhenValueMatches()
    {
        await Page.Locator("[data-testid='enabled-input']").Should().HaveValueAsync("some value");
    }

    [Test]
    public async Task HaveValueAsync_Regex_ShouldPass_WhenValueMatchesPattern()
    {
        await Page.Locator("[data-testid='enabled-input']").Should().HaveValueAsync(new Regex("some.*"));
    }

    [Test]
    public async Task HaveValueAsync_Not_ShouldPass_WhenValueDoesNotMatch()
    {
        await Page.Locator("[data-testid='enabled-input']").Should().Not.HaveValueAsync("wrong");
    }

    // ───────────────────────── Values (multi-select) ─────────────────────────

    [Test]
    public async Task HaveValuesAsync_ShouldPass_WhenSelectedValuesMatch()
    {
        await Page.Locator("[data-testid='multi-select']").Should()
            .HaveValuesAsync(new[] { "apple", "banana" });
    }

    [Test]
    public async Task HaveValuesAsync_Regex_ShouldPass_WhenSelectedValuesMatchPattern()
    {
        await Page.Locator("[data-testid='multi-select']").Should()
            .HaveValuesAsync(new[] { new Regex("app.*"), new Regex("ban.*") });
    }

    // ───────────────────────── Id ─────────────────────────

    [Test]
    public async Task HaveIdAsync_String_ShouldPass_WhenIdMatches()
    {
        await Page.Locator("[data-testid='heading']").Should().HaveIdAsync("main-heading");
    }

    [Test]
    public async Task HaveIdAsync_Regex_ShouldPass_WhenIdMatchesPattern()
    {
        await Page.Locator("[data-testid='heading']").Should().HaveIdAsync(new Regex("main-.*"));
    }

    [Test]
    public async Task HaveIdAsync_Not_ShouldPass_WhenIdDoesNotMatch()
    {
        await Page.Locator("[data-testid='heading']").Should().Not.HaveIdAsync("wrong-id");
    }

    // ───────────────────────── Class ─────────────────────────

    [Test]
    public async Task HaveClassAsync_String_ShouldPass_WhenClassMatches()
    {
        await Page.Locator("[data-testid='heading']").Should().HaveClassAsync("highlight");
    }

    [Test]
    public async Task HaveClassAsync_Regex_ShouldPass_WhenClassMatchesPattern()
    {
        await Page.Locator("[data-testid='heading']").Should().HaveClassAsync(new Regex("high.*"));
    }

    [Test]
    public async Task ContainClassAsync_ShouldPass_WhenElementContainsClass()
    {
        await Page.Locator("#description").Should().ContainClassAsync("primary");
    }

    // ───────────────────────── Count ─────────────────────────

    [Test]
    public async Task HaveCountAsync_ShouldPass_WhenCountMatches()
    {
        await Page.Locator("[data-testid='list-item']").Should().HaveCountAsync(5);
    }

    [Test]
    public async Task HaveCountAsync_Not_ShouldPass_WhenCountDoesNotMatch()
    {
        await Page.Locator("[data-testid='list-item']").Should().Not.HaveCountAsync(3);
    }

    // ───────────────────────── Attribute ─────────────────────────

    [Test]
    public async Task HaveAttributeAsync_String_ShouldPass_WhenAttributeMatches()
    {
        await Page.Locator("[data-testid='enabled-input']").Should()
            .HaveAttributeAsync("type", "text");
    }

    [Test]
    public async Task HaveAttributeAsync_Regex_ShouldPass_WhenAttributeMatchesPattern()
    {
        await Page.Locator("[data-testid='enabled-input']").Should()
            .HaveAttributeAsync("type", new Regex("te.*"));
    }

    [Test]
    public async Task HaveAttributeAsync_Not_ShouldPass_WhenAttributeDoesNotMatch()
    {
        await Page.Locator("[data-testid='enabled-input']").Should()
            .Not.HaveAttributeAsync("type", "password");
    }

    // ───────────────────────── CSS ─────────────────────────

    [Test]
    public async Task HaveCSSAsync_String_ShouldPass_WhenCSSMatches()
    {
        await Page.Locator("[data-testid='styled-text']").Should()
            .HaveCSSAsync("color", "rgb(255, 0, 0)");
    }

    [Test]
    public async Task HaveCSSAsync_Regex_ShouldPass_WhenCSSMatchesPattern()
    {
        await Page.Locator("[data-testid='styled-text']").Should()
            .HaveCSSAsync("color", new Regex("rgb\\(255.*"));
    }

    // ───────────────────────── Role ─────────────────────────

    [Test]
    public async Task HaveRoleAsync_ShouldPass_WhenRoleMatches()
    {
        await Page.Locator("[data-testid='submit-btn']").Should()
            .HaveRoleAsync(AriaRole.Button);
    }

    [Test]
    public async Task HaveRoleAsync_Not_ShouldPass_WhenRoleDoesNotMatch()
    {
        await Page.Locator("[data-testid='submit-btn']").Should()
            .Not.HaveRoleAsync(AriaRole.Link);
    }

    // ───────────────────────── Accessible Name ─────────────────────────

    [Test]
    public async Task HaveAccessibleNameAsync_String_ShouldPass_WhenNameMatches()
    {
        await Page.Locator("[data-testid='submit-btn']").Should()
            .HaveAccessibleNameAsync("Submit form");
    }

    [Test]
    public async Task HaveAccessibleNameAsync_Regex_ShouldPass_WhenNameMatchesPattern()
    {
        await Page.Locator("[data-testid='submit-btn']").Should()
            .HaveAccessibleNameAsync(new Regex("Submit.*"));
    }

    // ───────────────────────── Accessible Description ─────────────────────────

    [Test]
    public async Task HaveAccessibleDescriptionAsync_String_ShouldPass_WhenDescriptionMatches()
    {
        await Page.Locator("[data-testid='submit-btn']").Should()
            .HaveAccessibleDescriptionAsync("Click to submit the form");
    }

    [Test]
    public async Task HaveAccessibleDescriptionAsync_Regex_ShouldPass_WhenDescriptionMatchesPattern()
    {
        await Page.Locator("[data-testid='submit-btn']").Should()
            .HaveAccessibleDescriptionAsync(new Regex("Click to.*"));
    }

    // ───────────────────────── Accessible Error Message ─────────────────────────

    [Test]
    public async Task HaveAccessibleErrorMessageAsync_String_ShouldPass_WhenErrorMessageMatches()
    {
        await Page.Locator("[data-testid='error-input']").Should()
            .HaveAccessibleErrorMessageAsync("Please enter a valid email address");
    }

    [Test]
    public async Task HaveAccessibleErrorMessageAsync_Regex_ShouldPass_WhenErrorMessageMatchesPattern()
    {
        await Page.Locator("[data-testid='error-input']").Should()
            .HaveAccessibleErrorMessageAsync(new Regex("Please enter.*"));
    }

    // ───────────────────────── JS Property ─────────────────────────

    [Test]
    public async Task HaveJSPropertyAsync_ShouldPass_WhenPropertyMatches()
    {
        await Page.Locator("[data-testid='enabled-input']").Should()
            .HaveJSPropertyAsync("type", "text");
    }

    [Test]
    public async Task HaveJSPropertyAsync_Not_ShouldPass_WhenPropertyDoesNotMatch()
    {
        await Page.Locator("[data-testid='enabled-input']").Should()
            .Not.HaveJSPropertyAsync("type", "password");
    }

    // ───────────────────────── Chaining ─────────────────────────

    [Test]
    public async Task Chaining_ShouldPass_WhenMultipleAssertionsAreTrue()
    {
        await Page.Locator("[data-testid='enabled-input']").Should()
            .BeVisibleAsync()
            .BeEnabledAsync()
            .BeEditableAsync()
            .Not.BeEmptyAsync()
            .HaveValueAsync("some value")
            .HaveAttributeAsync("type", "text")
            .HaveIdAsync("enabled-input");
    }

    [Test]
    public async Task Chaining_WithNot_ShouldPass_ForHiddenAndDisabledChecks()
    {
        await Page.Locator("[data-testid='hidden-element']").Should()
            .BeHiddenAsync()
            .Not.BeVisibleAsync();
    }

    // ───────────────────────── Error Messages ─────────────────────────

    [Test]
    public void BeVisibleAsync_ShouldThrow_WhenElementIsHidden()
    {
        var locator = Page.Locator("[data-testid='hidden-element']");

        var ex = Assert.ThrowsAsync<PlaywrightException>(
            async () => await locator.Should().BeVisibleAsync());

        Assert.That(ex!.Message, Does.Contain("visible"));
    }

    [Test]
    public void BeHiddenAsync_ShouldThrow_WhenElementIsVisible()
    {
        var locator = Page.Locator("[data-testid='visible-element']");

        var ex = Assert.ThrowsAsync<PlaywrightException>(
            async () => await locator.Should().BeHiddenAsync());

        Assert.That(ex!.Message, Does.Contain("hidden"));
    }

    [Test]
    public void BeEnabledAsync_ShouldThrow_WhenElementIsDisabled()
    {
        var locator = Page.Locator("[data-testid='disabled-input']");

        var ex = Assert.ThrowsAsync<PlaywrightException>(
            async () => await locator.Should().BeEnabledAsync());

        Assert.That(ex!.Message, Does.Contain("enabled"));
    }

    [Test]
    public void HaveTextAsync_ShouldThrow_WhenTextDoesNotMatch()
    {
        var locator = Page.Locator("[data-testid='paragraph']");

        var ex = Assert.ThrowsAsync<PlaywrightException>(
            async () => await locator.Should().HaveTextAsync("Wrong text"));

        Assert.That(ex!.Message, Does.Contain("expected"));
    }

    [Test]
    public void HaveValueAsync_ShouldThrow_WhenValueDoesNotMatch()
    {
        var locator = Page.Locator("[data-testid='enabled-input']");

        var ex = Assert.ThrowsAsync<PlaywrightException>(
            async () => await locator.Should().HaveValueAsync("wrong value"));

        Assert.That(ex!.Message, Does.Contain("expected"));
    }

    // ───────────────────────── Because Message Formatting ─────────────────────────

    [Test]
    public void Because_ShouldAppearInExceptionMessage_WhenAssertionFails()
    {
        var locator = Page.Locator("[data-testid='hidden-element']");

        var ex = Assert.ThrowsAsync<PlaywrightException>(
            async () => await locator.Should()
                .BeVisibleAsync(because: "the element should be shown after login"));

        Assert.That(ex!.Message, Does.Contain("the element should be shown after login"));
    }

    [Test]
    public void Because_WithArgs_ShouldFormatMessageInException()
    {
        var locator = Page.Locator("[data-testid='hidden-element']");

        var ex = Assert.ThrowsAsync<PlaywrightException>(
            async () => await locator.Should()
                .BeVisibleAsync(because: "element '{0}' should be visible on page '{1}'",
                    becauseArgs: ["hidden-element", "index"]));

        Assert.That(ex!.Message, Does.Contain("element 'hidden-element' should be visible on page 'index'"));
    }

    [Test]
    public void Because_InChain_ShouldShowMessageForFailingStep()
    {
        var locator = Page.Locator("[data-testid='disabled-input']");

        var ex = Assert.ThrowsAsync<PlaywrightException>(
            async () => await locator.Should()
                .BeVisibleAsync(because: "it should be visible")
                .BeEnabledAsync(because: "it should be enabled for interaction"));

        // The first step passes (disabled-input is visible), the second fails
        Assert.That(ex!.Message, Does.Contain("it should be enabled for interaction"));
    }

    [Test]
    public void Chaining_ShouldThrowOnFirstFailure_WhenMiddleStepFails()
    {
        var locator = Page.Locator("[data-testid='hidden-element']");

        var ex = Assert.ThrowsAsync<PlaywrightException>(
            async () => await locator.Should()
                .BeHiddenAsync()        // passes
                .BeVisibleAsync()       // fails — should stop here
                .BeEnabledAsync());     // should never run

        Assert.That(ex!.Message, Does.Contain("visible"));
    }

    [Test]
    public void NotFoundLocator_ShouldThrow_WhenAssertingVisibility()
    {
        var locator = Page.Locator("[data-testid='does-not-exist']");

        var ex = Assert.ThrowsAsync<PlaywrightException>(
            async () => await locator.Should().BeVisibleAsync());

        Assert.That(ex!.Message, Does.Contain("expected"));
    }
}

