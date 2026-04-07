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

    /// <inheritdoc cref="ILocatorAssertions.ToBeAttachedAsync"/>
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
    
    /// <inheritdoc cref="ILocatorAssertions.ToBeCheckedAsync"/>
    public FluentLocatorAssertions ToBeCheckedAsync(LocatorAssertionsToBeCheckedOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeCheckedAsync(options)
                : Expect(_locator).ToBeCheckedAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToBeDisabledAsync"/>
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
    
    /// <inheritdoc cref="ILocatorAssertions.ToBeEditableAsync"/>
    public FluentLocatorAssertions BeEditableAsync(LocatorAssertionsToBeEditableOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeEditableAsync(options)
                : Expect(_locator).ToBeEditableAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToBeEmptyAsync"/>
    public FluentLocatorAssertions BeEmptyAsync(LocatorAssertionsToBeEmptyOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeEmptyAsync(options)
                : Expect(_locator).ToBeEmptyAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToBeEnabledAsync"/>
    public FluentLocatorAssertions BeEnabledAsync(LocatorAssertionsToBeEnabledOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeEnabledAsync(options)
                : Expect(_locator).ToBeEnabledAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToBeFocusedAsync"/>
    public FluentLocatorAssertions BeFocusedAsync(LocatorAssertionsToBeFocusedOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeFocusedAsync(options)
                : Expect(_locator).ToBeFocusedAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToBeHiddenAsync"/>
    public FluentLocatorAssertions BeHiddenAsync(LocatorAssertionsToBeHiddenOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
       AddStep(() => negate 
           ? Expect(_locator).Not.ToBeHiddenAsync() 
           : Expect(_locator).ToBeHiddenAsync(),
           new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToBeInViewportAsync"/>
    public FluentLocatorAssertions BeInViewportAsync(LocatorAssertionsToBeInViewportOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeInViewportAsync(options)
                : Expect(_locator).ToBeInViewportAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    
    /// <inheritdoc cref="ILocatorAssertions.ToBeVisibleAsync"/>
    public FluentLocatorAssertions BeVisibleAsync(LocatorAssertionsToBeVisibleOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeVisibleAsync(options)
                : Expect(_locator).ToBeVisibleAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToContainClassAsync(string, LocatorAssertionsToContainClassOptions?)"/>
    public FluentLocatorAssertions ContainClassAsync(string expected, LocatorAssertionsToContainClassOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToContainClassAsync(expected, options)
                : Expect(_locator).ToContainClassAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveAccessibleDescriptionAsync(string, LocatorAssertionsToHaveAccessibleDescriptionOptions?)"/>
    public FluentLocatorAssertions HaveAccessibleDescriptionAsync(string expected, LocatorAssertionsToHaveAccessibleDescriptionOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAccessibleDescriptionAsync(expected, options)
                : Expect(_locator).ToHaveAccessibleDescriptionAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveAccessibleDescriptionAsync(Regex, LocatorAssertionsToHaveAccessibleDescriptionOptions?)"/>
    public FluentLocatorAssertions HaveAccessibleDescriptionAsync(Regex expected, LocatorAssertionsToHaveAccessibleDescriptionOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAccessibleDescriptionAsync(expected, options)
                : Expect(_locator).ToHaveAccessibleDescriptionAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveAccessibleErrorMessageAsync(string, LocatorAssertionsToHaveAccessibleErrorMessageOptions?)"/>
    public FluentLocatorAssertions HaveAccessibleErrorMessageAsync(string expected, LocatorAssertionsToHaveAccessibleErrorMessageOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAccessibleErrorMessageAsync(expected, options)
                : Expect(_locator).ToHaveAccessibleErrorMessageAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveAccessibleErrorMessageAsync(Regex, LocatorAssertionsToHaveAccessibleErrorMessageOptions?)"/>
    public FluentLocatorAssertions HaveAccessibleErrorMessageAsync(Regex expected, LocatorAssertionsToHaveAccessibleErrorMessageOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAccessibleErrorMessageAsync(expected, options)
                : Expect(_locator).ToHaveAccessibleErrorMessageAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveAccessibleNameAsync(string, LocatorAssertionsToHaveAccessibleNameOptions?)"/>
    public FluentLocatorAssertions HaveAccessibleNameAsync(string expected, LocatorAssertionsToHaveAccessibleNameOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAccessibleNameAsync(expected, options)
                : Expect(_locator).ToHaveAccessibleNameAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveAccessibleNameAsync(Regex, LocatorAssertionsToHaveAccessibleNameOptions?)"/>
    public FluentLocatorAssertions HaveAccessibleNameAsync(Regex expected, LocatorAssertionsToHaveAccessibleNameOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAccessibleNameAsync(expected, options)
                : Expect(_locator).ToHaveAccessibleNameAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveAttributeAsync(string, string, LocatorAssertionsToHaveAttributeOptions?)"/>
    public FluentLocatorAssertions HaveAttributeAsync(string name, string expected, LocatorAssertionsToHaveAttributeOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAttributeAsync(name, expected, options)
                : Expect(_locator).ToHaveAttributeAsync(name, expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveAttributeAsync(string, Regex, LocatorAssertionsToHaveAttributeOptions?)"/>
    public FluentLocatorAssertions HaveAttributeAsync(string name, Regex expected, LocatorAssertionsToHaveAttributeOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveAttributeAsync(name, expected, options)
                : Expect(_locator).ToHaveAttributeAsync(name, expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveClassAsync(string, LocatorAssertionsToHaveClassOptions?)"/>
    public FluentLocatorAssertions HaveClassAsync(string expected, LocatorAssertionsToHaveClassOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveClassAsync(expected, options)
                : Expect(_locator).ToHaveClassAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveClassAsync(Regex, LocatorAssertionsToHaveClassOptions?)"/>
    public FluentLocatorAssertions HaveClassAsync(Regex expected, LocatorAssertionsToHaveClassOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveClassAsync(expected, options)
                : Expect(_locator).ToHaveClassAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveClassAsync(IEnumerable{string}, LocatorAssertionsToHaveClassOptions?)"/>
    public FluentLocatorAssertions HaveClassAsync(IEnumerable<string> expected, LocatorAssertionsToHaveClassOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveClassAsync(expected, options)
                : Expect(_locator).ToHaveClassAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveClassAsync(IEnumerable{Regex}, LocatorAssertionsToHaveClassOptions?)"/>
    public FluentLocatorAssertions HaveClassAsync(IEnumerable<Regex> expected, LocatorAssertionsToHaveClassOptions? options = null, string because = "", params object[] becauseArgs)
    { 
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveClassAsync(expected, options)
                : Expect(_locator).ToHaveClassAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
     
    /// <inheritdoc cref="ILocatorAssertions.ToHaveCountAsync"/>
    public FluentLocatorAssertions HaveCountAsync(int expected, LocatorAssertionsToHaveCountOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveCountAsync(expected, options)
                : Expect(_locator).ToHaveCountAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
     
     /// <inheritdoc cref="ILocatorAssertions.ToHaveCSSAsync(string, string, LocatorAssertionsToHaveCSSOptions?)"/>
     public FluentLocatorAssertions HaveCSSAsync(string name, string expected, LocatorAssertionsToHaveCSSOptions? options = null, string because = "", params object[] becauseArgs)
     {
         var negate = NegateNext;
         AddStep(() => negate
                 ? Expect(_locator).Not.ToHaveCSSAsync(name, expected, options)
                 : Expect(_locator).ToHaveCSSAsync(name, expected, options),
             new Because(because, becauseArgs));
         return this;
     }
     
     /// <inheritdoc cref="ILocatorAssertions.ToHaveCSSAsync(string, Regex, LocatorAssertionsToHaveCSSOptions?)"/>
     public FluentLocatorAssertions HaveCSSAsync(string name, Regex expected, LocatorAssertionsToHaveCSSOptions? options = null, string because = "", params object[] becauseArgs)
     {
         var negate = NegateNext;
         AddStep(() => negate
                 ? Expect(_locator).Not.ToHaveCSSAsync(name, expected, options)
                 : Expect(_locator).ToHaveCSSAsync(name, expected, options),
             new Because(because, becauseArgs));
         return this;
     }
     
    /// <inheritdoc cref="ILocatorAssertions.ToHaveIdAsync(string, LocatorAssertionsToHaveIdOptions?)"/>
    public FluentLocatorAssertions HaveIdAsync(string id, LocatorAssertionsToHaveIdOptions? options = null, string because = "", params object[] becauseArgs)
    { 
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveIdAsync(id, options)
                : Expect(_locator).ToHaveIdAsync(id, options),
            new Because(because, becauseArgs));
        return this;
    }
     
    /// <inheritdoc cref="ILocatorAssertions.ToHaveIdAsync(Regex, LocatorAssertionsToHaveIdOptions?)"/>
    public FluentLocatorAssertions HaveIdAsync(Regex id, LocatorAssertionsToHaveIdOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveIdAsync(id, options)
                : Expect(_locator).ToHaveIdAsync(id, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveJSPropertyAsync"/>
    public FluentLocatorAssertions HaveJSPropertyAsync(string name, object value, LocatorAssertionsToHaveJSPropertyOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveJSPropertyAsync(name, value, options)
                : Expect(_locator).ToHaveJSPropertyAsync(name, value, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveRoleAsync"/>
    public FluentLocatorAssertions HaveRoleAsync(AriaRole role, LocatorAssertionsToHaveRoleOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveRoleAsync(role, options)
                : Expect(_locator).ToHaveRoleAsync(role, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveTextAsync(string, LocatorAssertionsToHaveTextOptions?)"/>
    public FluentLocatorAssertions HaveTextAsync(string expected, LocatorAssertionsToHaveTextOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveTextAsync(expected, options)
                : Expect(_locator).ToHaveTextAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveTextAsync(Regex, LocatorAssertionsToHaveTextOptions?)"/>
    public FluentLocatorAssertions HaveTextAsync(Regex expected, LocatorAssertionsToHaveTextOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveTextAsync(expected, options)
                : Expect(_locator).ToHaveTextAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveTextAsync(IEnumerable{string}, LocatorAssertionsToHaveTextOptions?)"/>
    public FluentLocatorAssertions HaveTextAsync(IEnumerable<string> expected, LocatorAssertionsToHaveTextOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveTextAsync(expected, options)
                : Expect(_locator).ToHaveTextAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveTextAsync(IEnumerable{Regex}, LocatorAssertionsToHaveTextOptions?)"/>
    public FluentLocatorAssertions HaveTextAsync(IEnumerable<Regex> expected, LocatorAssertionsToHaveTextOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveTextAsync(expected, options)
                : Expect(_locator).ToHaveTextAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveValueAsync(string, LocatorAssertionsToHaveValueOptions?)"/>
    public FluentLocatorAssertions HaveValueAsync(string value, LocatorAssertionsToHaveValueOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveValueAsync(value, options)
                : Expect(_locator).ToHaveValueAsync(value, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveValueAsync(Regex, LocatorAssertionsToHaveValueOptions?)"/>
    public FluentLocatorAssertions HaveValueAsync(Regex value, LocatorAssertionsToHaveValueOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveValueAsync(value, options)
                : Expect(_locator).ToHaveValueAsync(value, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveValuesAsync(IEnumerable{string}, LocatorAssertionsToHaveValuesOptions?)"/>
    public FluentLocatorAssertions HaveValuesAsync(IEnumerable<string> values, LocatorAssertionsToHaveValuesOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveValuesAsync(values, options)
                : Expect(_locator).ToHaveValuesAsync(values, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToHaveValuesAsync(IEnumerable{Regex}, LocatorAssertionsToHaveValuesOptions?)"/>
    public FluentLocatorAssertions HaveValuesAsync(IEnumerable<Regex> values, LocatorAssertionsToHaveValuesOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToHaveValuesAsync(values, options)
                : Expect(_locator).ToHaveValuesAsync(values, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="ILocatorAssertions.ToMatchAriaSnapshotAsync"/>
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