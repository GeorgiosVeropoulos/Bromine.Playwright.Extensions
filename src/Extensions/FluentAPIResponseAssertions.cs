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
    
    /// <summary>
    /// Asserts that the API response has an OK status (2xx).
    /// See <see cref="IAPIResponseAssertions.ToBeOKAsync"/>.
    /// </summary>
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