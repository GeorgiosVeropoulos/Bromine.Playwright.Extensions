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
    /// See <see cref="IPageAssertions.ToHaveTitleAsync(string, PageAssertionsToHaveTitleOptions)"/>.
    /// </summary>
    public FluentPageAssertions HaveTitleAsync(string titleOrRegExp, PageAssertionsToHaveTitleOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page).Not.ToHaveTitleAsync(titleOrRegExp,options)
            : Expect(_page).ToHaveTitleAsync(titleOrRegExp),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the page has the given URL.
    /// See <see cref="IPageAssertions.ToHaveURLAsync(string, PageAssertionsToHaveURLOptions)"/>.
    /// </summary>
    public FluentPageAssertions HaveURLAsync(string expectedUrl, PageAssertionsToHaveURLOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page).Not.ToHaveURLAsync(expectedUrl)
            : Expect(_page).ToHaveURLAsync(expectedUrl),
            new Because(because, becauseArgs));
        return this;
    }
}