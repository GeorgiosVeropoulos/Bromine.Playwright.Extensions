using System.Text.RegularExpressions;
using Bromine.Playwright.Extensions.Reason;
using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Extensions;

public class FluentPageAssertions : FluentBase<FluentPageAssertions>
{
    private readonly IPage _page;
    
    public FluentPageAssertions(IPage pagem, bool negateNext = false)
    {
        _page = pagem;
        NegateNext = negateNext;
    }
    
    
    
    /// <summary>
    /// Asserts that the page has the given title.
    /// See <see cref="IPageAssertions.ToHaveTitleAsync(string, PageAssertionsToHaveTitleOptions?)"/>.
    /// </summary>
    public FluentPageAssertions HaveTitleAsync(string titleOrRegExp, PageAssertionsToHaveTitleOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page).Not.ToHaveTitleAsync(titleOrRegExp, options)
            : Expect(_page).ToHaveTitleAsync(titleOrRegExp, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the page has a title matching the given regex.
    /// See <see cref="IPageAssertions.ToHaveTitleAsync(Regex, PageAssertionsToHaveTitleOptions?)"/>.
    /// </summary>
    public FluentPageAssertions HaveTitleAsync(Regex titleOrRegExp, PageAssertionsToHaveTitleOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page).Not.ToHaveTitleAsync(titleOrRegExp, options)
            : Expect(_page).ToHaveTitleAsync(titleOrRegExp, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the page has the given URL.
    /// See <see cref="IPageAssertions.ToHaveURLAsync(string, PageAssertionsToHaveURLOptions?)"/>.
    /// </summary>
    public FluentPageAssertions HaveURLAsync(string expectedUrl, PageAssertionsToHaveURLOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page).Not.ToHaveURLAsync(expectedUrl, options)
            : Expect(_page).ToHaveURLAsync(expectedUrl, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the page has a URL matching the given regex.
    /// See <see cref="IPageAssertions.ToHaveURLAsync(Regex, PageAssertionsToHaveURLOptions?)"/>.
    /// </summary>
    public FluentPageAssertions HaveURLAsync(Regex urlOrRegExp, PageAssertionsToHaveURLOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page).Not.ToHaveURLAsync(urlOrRegExp, options)
            : Expect(_page).ToHaveURLAsync(urlOrRegExp, options),
            new Because(because, becauseArgs));
        return this;
    }
}