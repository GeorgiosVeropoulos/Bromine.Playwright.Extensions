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
    
    
    
    /// <inheritdoc cref="IPageAssertions.ToHaveTitleAsync(string titleOrRegExp, PageAssertionsToHaveTitleOptions? options = null)"/>
    public FluentPageAssertions HaveTitleAsync(string titleOrRegExp, PageAssertionsToHaveTitleOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page).Not.ToHaveTitleAsync(titleOrRegExp,options)
            : Expect(_page).ToHaveTitleAsync(titleOrRegExp),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <inheritdoc cref="IPageAssertions.ToHaveURLAsync(string urlOrRegExp, PageAssertionsToHaveURLOptions? options = null)"/>
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