using Bromine.Playwright.Extensions.Builders;
using Bromine.Playwright.Extensions.Configuration;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests;

#nullable  disable
public class TestBase
{
    
    protected IPage Page { get; private set; }
    protected IBrowser Browser;
    protected IBrowserContext Context { get; private set; }

    [SetUp]
    public async Task SetUp()
    {
        PlaywrightDefaults.Reset();

        var builder = PlaywrightBrowserBuilder.Create()
            .WithChromium();
        
        // Run headed locally, headless in CI
        if (Environment.GetEnvironmentVariable("CI") == null)
            builder.Headless();
        
        var result = await builder.BuildAsync();
        

        Browser = result.Browser;
        Context = await BrowserContextBuilder.For(result.Browser).WithBaseUrl("https://example.com").BuildAsync();
        Page = await Context.NewPageAsync();
        
        await Page.GotoAsync("/");
    }
        
        
}