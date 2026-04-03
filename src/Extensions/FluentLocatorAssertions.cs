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

    public FluentLocatorAssertions ToBeAttachedAsync(LocatorAssertionsToBeAttachedOptions? options = null,
        string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToBeAttachedAsync(options)
            : Expect(_locator).ToBeAttachedAsync(options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions ToBeCheckedAsync(LocatorAssertionsToBeCheckedOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToBeCheckedAsync(options)
            : Expect(_locator).ToBeCheckedAsync(options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions BeDisabledAsync(LocatorAssertionsToBeDisabledOptions? options = null,
        string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToBeDisabledAsync(options)
            : Expect(_locator).ToBeDisabledAsync(options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions BeEditableAsync(LocatorAssertionsToBeEditableOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToBeEditableAsync(options)
            : Expect(_locator).ToBeEditableAsync(options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions BeEmptyAsync(LocatorAssertionsToBeEmptyOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToBeEmptyAsync(options)
            : Expect(_locator).ToBeEmptyAsync(options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions BeEnabledAsync(LocatorAssertionsToBeEnabledOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeEnabledAsync(options)
                : Expect(_locator).ToBeEnabledAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions BeFocusedAsync(LocatorAssertionsToBeFocusedOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToBeFocusedAsync(options)
            : Expect(_locator).ToBeFocusedAsync(options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions BeHiddenAsync(LocatorAssertionsToBeHiddenOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToBeHiddenAsync(options)
            : Expect(_locator).ToBeHiddenAsync(options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions BeInViewportAsync(LocatorAssertionsToBeInViewportOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToBeInViewportAsync(options)
            : Expect(_locator).ToBeInViewportAsync(options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    
    public FluentLocatorAssertions BeVisibleAsync(LocatorAssertionsToBeVisibleOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
                ? Expect(_locator).Not.ToBeVisibleAsync(options)
                : Expect(_locator).ToBeVisibleAsync(options),
            new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions ContainClassAsync(string expected, LocatorAssertionsToContainClassOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToContainClassAsync(expected, options)
            : Expect(_locator).ToContainClassAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveAccessibleDescriptionAsync(string expected, LocatorAssertionsToHaveAccessibleDescriptionOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveAccessibleDescriptionAsync(expected, options)
            : Expect(_locator).ToHaveAccessibleDescriptionAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveAccessibleDescriptionAsync(Regex expected, LocatorAssertionsToHaveAccessibleDescriptionOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveAccessibleDescriptionAsync(expected, options)
            : Expect(_locator).ToHaveAccessibleDescriptionAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveAccessibleErrorMessageAsync(string expected, LocatorAssertionsToHaveAccessibleErrorMessageOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveAccessibleErrorMessageAsync(expected, options)
            : Expect(_locator).ToHaveAccessibleErrorMessageAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    
    public FluentLocatorAssertions HaveAccessibleErrorMessageAsync(Regex expected, LocatorAssertionsToHaveAccessibleErrorMessageOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveAccessibleErrorMessageAsync(expected, options)
            : Expect(_locator).ToHaveAccessibleErrorMessageAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveAccessibleNameAsync(string expected, LocatorAssertionsToHaveAccessibleNameOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveAccessibleNameAsync(expected, options)
            : Expect(_locator).ToHaveAccessibleNameAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveAccessibleNameAsync(Regex expected, LocatorAssertionsToHaveAccessibleNameOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveAccessibleNameAsync(expected, options)
            : Expect(_locator).ToHaveAccessibleNameAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveAttributeAsync(string name, string expected, LocatorAssertionsToHaveAttributeOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveAttributeAsync(name, expected, options)
            : Expect(_locator).ToHaveAttributeAsync(name, expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveAttributeAsync(string name, Regex expected, LocatorAssertionsToHaveAttributeOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveAttributeAsync(name, expected, options)
            : Expect(_locator).ToHaveAttributeAsync(name, expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveClassAsync(string expected, LocatorAssertionsToHaveClassOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveClassAsync(expected, options)
            : Expect(_locator).ToHaveClassAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveClassAsync(Regex expected, LocatorAssertionsToHaveClassOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveClassAsync(expected, options)
            : Expect(_locator).ToHaveClassAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveClassAsync(IEnumerable<string> expected, LocatorAssertionsToHaveClassOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveClassAsync(expected, options)
            : Expect(_locator).ToHaveClassAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveClassAsync(IEnumerable<Regex> expected, LocatorAssertionsToHaveClassOptions? options = null, string because = "", params object[] becauseArgs)
    { 
        var task = NegateNext 
            ? Expect(_locator).Not.ToHaveClassAsync(expected, options)
            : Expect(_locator).ToHaveClassAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
     
    public FluentLocatorAssertions HaveCountAsync(int expected, LocatorAssertionsToHaveCountOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveCountAsync(expected, options)
            : Expect(_locator).ToHaveCountAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
     
     public FluentLocatorAssertions HaveCSSAsync(string name, string expected, LocatorAssertionsToHaveCSSOptions? options = null, string because = "", params object[] becauseArgs)
     {
         var task = NegateNext
             ? Expect(_locator).Not.ToHaveCSSAsync(name, expected, options)
             : Expect(_locator).ToHaveCSSAsync(name, expected, options);
         AddStep(() => task, new Because(because, becauseArgs));
         return this;
     }
     
     public FluentLocatorAssertions HaveCSSAsync(string name, Regex expected, LocatorAssertionsToHaveCSSOptions? options = null, string because = "", params object[] becauseArgs)
     {
         var task = NegateNext
             ? Expect(_locator).Not.ToHaveCSSAsync(name, expected, options)
             : Expect(_locator).ToHaveCSSAsync(name, expected, options);
         AddStep(() => task, new Because(because, becauseArgs));
         return this;
     }
     
    public FluentLocatorAssertions HaveIdAsync(string id, LocatorAssertionsToHaveIdOptions? options = null, string because = "", params object[] becauseArgs)
    { 
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveIdAsync(id, options)
            : Expect(_locator).ToHaveIdAsync(id, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
     
    public FluentLocatorAssertions HaveIdAsync(Regex id, LocatorAssertionsToHaveIdOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveIdAsync(id, options)
            : Expect(_locator).ToHaveIdAsync(id, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveJSPropertyAsync(string name, object value, LocatorAssertionsToHaveJSPropertyOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveJSPropertyAsync(name, value, options)
            : Expect(_locator).ToHaveJSPropertyAsync(name, value, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveRoleAsync(AriaRole role, LocatorAssertionsToHaveRoleOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveRoleAsync(role, options)
            : Expect(_locator).ToHaveRoleAsync(role, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveTextAsync(string expected, LocatorAssertionsToHaveTextOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveTextAsync(expected, options)
            : Expect(_locator).ToHaveTextAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveTextAsync(Regex expected, LocatorAssertionsToHaveTextOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveTextAsync(expected, options)
            : Expect(_locator).ToHaveTextAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveTextAsync(IEnumerable<string> expected, LocatorAssertionsToHaveTextOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveTextAsync(expected, options)
            : Expect(_locator).ToHaveTextAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveTextAsync(IEnumerable<Regex> expected, LocatorAssertionsToHaveTextOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveTextAsync(expected, options)
            : Expect(_locator).ToHaveTextAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveValueAsync(string value, LocatorAssertionsToHaveValueOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveValueAsync(value, options)
            : Expect(_locator).ToHaveValueAsync(value, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveValueAsync(Regex value, LocatorAssertionsToHaveValueOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveValueAsync(value, options)
            : Expect(_locator).ToHaveValueAsync(value, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveValuesAsync(IEnumerable<string> values, LocatorAssertionsToHaveValuesOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveValuesAsync(values, options)
            : Expect(_locator).ToHaveValuesAsync(values, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions HaveValuesAsync(IEnumerable<Regex> values, LocatorAssertionsToHaveValuesOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToHaveValuesAsync(values, options)
            : Expect(_locator).ToHaveValuesAsync(values, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentLocatorAssertions MatchAriaSnapshotAsync(string expected, LocatorAssertionsToMatchAriaSnapshotOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_locator).Not.ToMatchAriaSnapshotAsync(expected, options)
            : Expect(_locator).ToMatchAriaSnapshotAsync(expected, options);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
}