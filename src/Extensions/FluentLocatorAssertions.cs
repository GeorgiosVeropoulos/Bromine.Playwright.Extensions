using System.Text.RegularExpressions;
using Bromine.Playwright.Extensions.Reason;
using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Extensions;

public class FluentLocatorAssertions : FluentBase<FluentLocatorAssertions>
{
    
    private readonly ILocator _locator;
    
    public FluentLocatorAssertions(ILocator locator, bool negateNext = false)
    {
        _locator = locator;
        NegateNext = negateNext;
    }

    /// <summary>
    /// Asserts that the locator points to an attached DOM node.
    /// See <see cref="ILocatorAssertions.ToBeAttachedAsync"/>.
    /// </summary>
    public FluentLocatorAssertions ToBeAttachedAsync(LocatorAssertionsToBeAttachedOptions? options = null,
        string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeAttachedAsync(options)
                : Expect(_locator).ToBeAttachedAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the checkbox locator is checked.
    /// See <see cref="ILocatorAssertions.ToBeCheckedAsync"/>.
    /// </summary>
    public FluentLocatorAssertions ToBeCheckedAsync(LocatorAssertionsToBeCheckedOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeCheckedAsync(options)
                : Expect(_locator).ToBeCheckedAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to a disabled element.
    /// See <see cref="ILocatorAssertions.ToBeDisabledAsync"/>.
    /// </summary>
    public FluentLocatorAssertions BeDisabledAsync(LocatorAssertionsToBeDisabledOptions? options = null,
        string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeDisabledAsync(options)
                : Expect(_locator).ToBeDisabledAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an editable element.
    /// See <see cref="ILocatorAssertions.ToBeEditableAsync"/>.
    /// </summary>
    public FluentLocatorAssertions BeEditableAsync(LocatorAssertionsToBeEditableOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeEditableAsync(options)
                : Expect(_locator).ToBeEditableAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an empty editable element or to a DOM node that has no text.
    /// See <see cref="ILocatorAssertions.ToBeEmptyAsync"/>.
    /// </summary>
    public FluentLocatorAssertions BeEmptyAsync(LocatorAssertionsToBeEmptyOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeEmptyAsync(options)
                : Expect(_locator).ToBeEmptyAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an enabled element.
    /// See <see cref="ILocatorAssertions.ToBeEnabledAsync"/>.
    /// </summary>
    public FluentLocatorAssertions BeEnabledAsync(LocatorAssertionsToBeEnabledOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeEnabledAsync(options)
                : Expect(_locator).ToBeEnabledAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to a focused DOM node.
    /// See <see cref="ILocatorAssertions.ToBeFocusedAsync"/>.
    /// </summary>
    public FluentLocatorAssertions BeFocusedAsync(LocatorAssertionsToBeFocusedOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeFocusedAsync(options)
                : Expect(_locator).ToBeFocusedAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to a hidden element (not visible).
    /// See <see cref="ILocatorAssertions.ToBeHiddenAsync"/>.
    /// </summary>
    public FluentLocatorAssertions BeHiddenAsync(LocatorAssertionsToBeHiddenOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
       AddStep(() => negate 
           ? Expect(_locator).Not.ToBeHiddenAsync() 
           : Expect(_locator).ToBeHiddenAsync(),
           new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element that intersects the viewport.
    /// See <see cref="ILocatorAssertions.ToBeInViewportAsync"/>.
    /// </summary>
    public FluentLocatorAssertions BeInViewportAsync(LocatorAssertionsToBeInViewportOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeInViewportAsync(options)
                : Expect(_locator).ToBeInViewportAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to a visible element.
    /// See <see cref="ILocatorAssertions.ToBeVisibleAsync"/>.
    /// </summary>
    public FluentLocatorAssertions BeVisibleAsync(LocatorAssertionsToBeVisibleOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeVisibleAsync(options)
                : Expect(_locator).ToBeVisibleAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element that contains the given CSS class.
    /// See <see cref="ILocatorAssertions.ToContainClassAsync(string, LocatorAssertionsToContainClassOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions ContainClassAsync(string expected, LocatorAssertionsToContainClassOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToContainClassAsync(expected, options)
                : Expect(_locator).ToContainClassAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with the given accessible description.
    /// See <see cref="ILocatorAssertions.ToHaveAccessibleDescriptionAsync(string, LocatorAssertionsToHaveAccessibleDescriptionOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveAccessibleDescriptionAsync(string expected, LocatorAssertionsToHaveAccessibleDescriptionOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAccessibleDescriptionAsync(expected, options)
                : Expect(_locator).ToHaveAccessibleDescriptionAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with an accessible description matching the given regex.
    /// See <see cref="ILocatorAssertions.ToHaveAccessibleDescriptionAsync(Regex, LocatorAssertionsToHaveAccessibleDescriptionOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveAccessibleDescriptionAsync(Regex expected, LocatorAssertionsToHaveAccessibleDescriptionOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAccessibleDescriptionAsync(expected, options)
                : Expect(_locator).ToHaveAccessibleDescriptionAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with the given accessible error message.
    /// See <see cref="ILocatorAssertions.ToHaveAccessibleErrorMessageAsync(string, LocatorAssertionsToHaveAccessibleErrorMessageOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveAccessibleErrorMessageAsync(string expected, LocatorAssertionsToHaveAccessibleErrorMessageOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAccessibleErrorMessageAsync(expected, options)
                : Expect(_locator).ToHaveAccessibleErrorMessageAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    
    /// <summary>
    /// Asserts that the locator points to an element with an accessible error message matching the given regex.
    /// See <see cref="ILocatorAssertions.ToHaveAccessibleErrorMessageAsync(Regex, LocatorAssertionsToHaveAccessibleErrorMessageOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveAccessibleErrorMessageAsync(Regex expected, LocatorAssertionsToHaveAccessibleErrorMessageOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAccessibleErrorMessageAsync(expected, options)
                : Expect(_locator).ToHaveAccessibleErrorMessageAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with the given accessible name.
    /// See <see cref="ILocatorAssertions.ToHaveAccessibleNameAsync(string, LocatorAssertionsToHaveAccessibleNameOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveAccessibleNameAsync(string expected, LocatorAssertionsToHaveAccessibleNameOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAccessibleNameAsync(expected, options)
                : Expect(_locator).ToHaveAccessibleNameAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with an accessible name matching the given regex.
    /// See <see cref="ILocatorAssertions.ToHaveAccessibleNameAsync(Regex, LocatorAssertionsToHaveAccessibleNameOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveAccessibleNameAsync(Regex expected, LocatorAssertionsToHaveAccessibleNameOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAccessibleNameAsync(expected, options)
                : Expect(_locator).ToHaveAccessibleNameAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with the given attribute value.
    /// See <see cref="ILocatorAssertions.ToHaveAttributeAsync(string, string, LocatorAssertionsToHaveAttributeOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveAttributeAsync(string name, string expected, LocatorAssertionsToHaveAttributeOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAttributeAsync(name, expected, options)
                : Expect(_locator).ToHaveAttributeAsync(name, expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with an attribute value matching the given regex.
    /// See <see cref="ILocatorAssertions.ToHaveAttributeAsync(string, Regex, LocatorAssertionsToHaveAttributeOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveAttributeAsync(string name, Regex expected, LocatorAssertionsToHaveAttributeOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAttributeAsync(name, expected, options)
                : Expect(_locator).ToHaveAttributeAsync(name, expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with the given CSS class.
    /// See <see cref="ILocatorAssertions.ToHaveClassAsync(string, LocatorAssertionsToHaveClassOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveClassAsync(string expected, LocatorAssertionsToHaveClassOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveClassAsync(expected, options)
                : Expect(_locator).ToHaveClassAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with a CSS class matching the given regex.
    /// See <see cref="ILocatorAssertions.ToHaveClassAsync(Regex, LocatorAssertionsToHaveClassOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveClassAsync(Regex expected, LocatorAssertionsToHaveClassOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveClassAsync(expected, options)
                : Expect(_locator).ToHaveClassAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator resolves to an element list where each element's CSS class matches the corresponding string.
    /// See <see cref="ILocatorAssertions.ToHaveClassAsync(IEnumerable{string}, LocatorAssertionsToHaveClassOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveClassAsync(IEnumerable<string> expected, LocatorAssertionsToHaveClassOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveClassAsync(expected, options)
                : Expect(_locator).ToHaveClassAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator resolves to an element list where each element's CSS class matches the corresponding regex.
    /// See <see cref="ILocatorAssertions.ToHaveClassAsync(IEnumerable{Regex}, LocatorAssertionsToHaveClassOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveClassAsync(IEnumerable<Regex> expected, LocatorAssertionsToHaveClassOptions? options = null, string because = "", params object[] becauseArgs)
    { 
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveClassAsync(expected, options)
                : Expect(_locator).ToHaveClassAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
     
    /// <summary>
    /// Asserts that the locator resolves to the given number of DOM nodes.
    /// See <see cref="ILocatorAssertions.ToHaveCountAsync"/>.
    /// </summary>
    public FluentLocatorAssertions HaveCountAsync(int expected, LocatorAssertionsToHaveCountOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveCountAsync(expected, options)
                : Expect(_locator).ToHaveCountAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
     
     /// <summary>
     /// Asserts that the locator points to an element with the given computed CSS style.
     /// See <see cref="ILocatorAssertions.ToHaveCSSAsync(string, string, LocatorAssertionsToHaveCSSOptions?)"/>.
     /// </summary>
     public FluentLocatorAssertions HaveCSSAsync(string name, string expected, LocatorAssertionsToHaveCSSOptions? options = null, string because = "", params object[] becauseArgs)
     {
         var negate = NegateNext;
         AddStep(() => negate
                 ? Expect(_locator).Not.ToHaveCSSAsync(name, expected, options)
                 : Expect(_locator).ToHaveCSSAsync(name, expected, options),
             new Because(because, becauseArgs));
         return this;
     }
     
     /// <summary>
     /// Asserts that the locator points to an element with a computed CSS style matching the given regex.
     /// See <see cref="ILocatorAssertions.ToHaveCSSAsync(string, Regex, LocatorAssertionsToHaveCSSOptions?)"/>.
     /// </summary>
     public FluentLocatorAssertions HaveCSSAsync(string name, Regex expected, LocatorAssertionsToHaveCSSOptions? options = null, string because = "", params object[] becauseArgs)
     {
         var negate = NegateNext;
         AddStep(() => negate
                 ? Expect(_locator).Not.ToHaveCSSAsync(name, expected, options)
                 : Expect(_locator).ToHaveCSSAsync(name, expected, options),
             new Because(because, becauseArgs));
         return this;
     }
     
    /// <summary>
    /// Asserts that the locator points to an element with the given DOM node ID.
    /// See <see cref="ILocatorAssertions.ToHaveIdAsync(string, LocatorAssertionsToHaveIdOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveIdAsync(string id, LocatorAssertionsToHaveIdOptions? options = null, string because = "", params object[] becauseArgs)
    { 
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveIdAsync(id, options)
                : Expect(_locator).ToHaveIdAsync(id, options),
            new Because(because, becauseArgs));
        return this;
    }
     
    /// <summary>
    /// Asserts that the locator points to an element with a DOM node ID matching the given regex.
    /// See <see cref="ILocatorAssertions.ToHaveIdAsync(Regex, LocatorAssertionsToHaveIdOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveIdAsync(Regex id, LocatorAssertionsToHaveIdOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveIdAsync(id, options)
                : Expect(_locator).ToHaveIdAsync(id, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with the given JavaScript property.
    /// See <see cref="ILocatorAssertions.ToHaveJSPropertyAsync"/>.
    /// </summary>
    public FluentLocatorAssertions HaveJSPropertyAsync(string name, object value, LocatorAssertionsToHaveJSPropertyOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveJSPropertyAsync(name, value, options)
                : Expect(_locator).ToHaveJSPropertyAsync(name, value, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with the given ARIA role.
    /// See <see cref="ILocatorAssertions.ToHaveRoleAsync"/>.
    /// </summary>
    public FluentLocatorAssertions HaveRoleAsync(AriaRole role, LocatorAssertionsToHaveRoleOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveRoleAsync(role, options)
                : Expect(_locator).ToHaveRoleAsync(role, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with the given text.
    /// See <see cref="ILocatorAssertions.ToHaveTextAsync(string, LocatorAssertionsToHaveTextOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveTextAsync(string expected, LocatorAssertionsToHaveTextOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveTextAsync(expected, options)
                : Expect(_locator).ToHaveTextAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with text matching the given regex.
    /// See <see cref="ILocatorAssertions.ToHaveTextAsync(Regex, LocatorAssertionsToHaveTextOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveTextAsync(Regex expected, LocatorAssertionsToHaveTextOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveTextAsync(expected, options)
                : Expect(_locator).ToHaveTextAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator resolves to an element list where each element's text matches the corresponding string.
    /// See <see cref="ILocatorAssertions.ToHaveTextAsync(IEnumerable{string}, LocatorAssertionsToHaveTextOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveTextAsync(IEnumerable<string> expected, LocatorAssertionsToHaveTextOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveTextAsync(expected, options)
                : Expect(_locator).ToHaveTextAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator resolves to an element list where each element's text matches the corresponding regex.
    /// See <see cref="ILocatorAssertions.ToHaveTextAsync(IEnumerable{Regex}, LocatorAssertionsToHaveTextOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveTextAsync(IEnumerable<Regex> expected, LocatorAssertionsToHaveTextOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveTextAsync(expected, options)
                : Expect(_locator).ToHaveTextAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with the given input value.
    /// See <see cref="ILocatorAssertions.ToHaveValueAsync(string, LocatorAssertionsToHaveValueOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveValueAsync(string value, LocatorAssertionsToHaveValueOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveValueAsync(value, options)
                : Expect(_locator).ToHaveValueAsync(value, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element with an input value matching the given regex.
    /// See <see cref="ILocatorAssertions.ToHaveValueAsync(Regex, LocatorAssertionsToHaveValueOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveValueAsync(Regex value, LocatorAssertionsToHaveValueOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveValueAsync(value, options)
                : Expect(_locator).ToHaveValueAsync(value, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to multi-select/combobox and the selected values match the given strings.
    /// See <see cref="ILocatorAssertions.ToHaveValuesAsync(IEnumerable{string}, LocatorAssertionsToHaveValuesOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveValuesAsync(IEnumerable<string> values, LocatorAssertionsToHaveValuesOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveValuesAsync(values, options)
                : Expect(_locator).ToHaveValuesAsync(values, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to multi-select/combobox and the selected values match the given regexes.
    /// See <see cref="ILocatorAssertions.ToHaveValuesAsync(IEnumerable{Regex}, LocatorAssertionsToHaveValuesOptions?)"/>.
    /// </summary>
    public FluentLocatorAssertions HaveValuesAsync(IEnumerable<Regex> values, LocatorAssertionsToHaveValuesOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveValuesAsync(values, options)
                : Expect(_locator).ToHaveValuesAsync(values, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the locator points to an element that matches the given accessibility snapshot.
    /// See <see cref="ILocatorAssertions.ToMatchAriaSnapshotAsync"/>.
    /// </summary>
    public FluentLocatorAssertions MatchAriaSnapshotAsync(string expected, LocatorAssertionsToMatchAriaSnapshotOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToMatchAriaSnapshotAsync(expected, options)
                : Expect(_locator).ToMatchAriaSnapshotAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
}