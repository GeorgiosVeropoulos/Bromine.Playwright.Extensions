using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests;

[SetUpFixture]
public class TestServerFixture
{
    private WebApplication? _app;

    /// <summary>
    /// Base URL of the local test server (e.g. http://localhost:5123).
    /// Available after <see cref="OneTimeSetUp"/> completes.
    /// </summary>
    public static string BaseUrl { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var wwwroot = Path.Combine(AppContext.BaseDirectory, "wwwroot");

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0"); 

        _app = builder.Build();

        _app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(wwwroot)
        });

        // Serve index.html for the root URL "/"
        _app.UseDefaultFiles(new DefaultFilesOptions
        {
            FileProvider = new PhysicalFileProvider(wwwroot)
        });

        // Re-register static files after default files so the pipeline works
        _app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(wwwroot)
        });

        await _app.StartAsync();

        BaseUrl = _app.Urls.First();
        TestContext.Progress.WriteLine($"Test server started at {BaseUrl}");
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (_app != null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }
}

