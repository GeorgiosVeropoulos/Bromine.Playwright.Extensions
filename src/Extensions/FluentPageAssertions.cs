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
    
    
    
    public FluentPageAssertions HaveTitleAsync(string expectedTitle, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_page).Not.ToHaveTitleAsync(expectedTitle)
            : Expect(_page).ToHaveTitleAsync(expectedTitle);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
    
    public FluentPageAssertions HaveURLAsync(string expectedUrl, string because = "", params object[] becauseArgs)
    {
        var task = NegateNext
            ? Expect(_page).Not.ToHaveURLAsync(expectedUrl)
            : Expect(_page).ToHaveURLAsync(expectedUrl);
        AddStep(() => task, new Because(because, becauseArgs));
        return this;
    }
}