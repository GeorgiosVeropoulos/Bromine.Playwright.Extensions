using Bromine.Playwright.Extensions.Reason;
using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Extensions;

public class FluentAPIResponseAssertions : FluentBase<FluentAPIResponseAssertions>
{
    
    private readonly IAPIResponse _response;
    
    
    public FluentAPIResponseAssertions(IAPIResponse response, bool negateNext = false)
    {
        _response = response;
        NegateNext = negateNext;
    }
    
    /// <inheritdoc cref="IAPIResponseAssertions.ToBeOKAsync"/>
    public FluentAPIResponseAssertions BeOKAsync(string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_response).Not.ToBeOKAsync()
            : Expect(_response).ToBeOKAsync(), 
            new Because(because, becauseArgs));
        return this;
    }
    
}