using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests;

#nullable disable

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Children)]
public class ApiTestBase
{
    private IPlaywright _playwright;

    protected IAPIRequestContext Request { get; private set; }

    protected string Url(string path) => $"{TestServerFixture.BaseUrl}{path}";

    [SetUp]
    public async Task SetUp()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        Request = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = TestServerFixture.BaseUrl
        });
    }

    [TearDown]
    public async Task TearDown()
    {
        if (Request != null)
            await Request.DisposeAsync();

        _playwright?.Dispose();
    }
}

