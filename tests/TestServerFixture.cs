using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

        // Browsers request /favicon.ico for any page without an icon link. Left to 404 it lands
        // in the console as an *error*, and console history survives navigation — so the 404 from
        // one page leaks into console assertions made on the next one. 204 keeps every page clean.
        _app.MapGet("/favicon.ico", () => Results.NoContent());

        // ── API endpoints for ResponseAssertionBuilder tests ──
        _app.MapGet("/api/ok", () => Results.Ok(new { success = true, message = "all good" }));

        _app.MapGet("/api/not-found", () => Results.NotFound(new { error = "resource not found" }));

        _app.MapGet("/api/users", () => Results.Json(
            new[] { new { id = 1, name = "Alice" }, new { id = 2, name = "Bob" } },
            statusCode: 200,
            contentType: "application/json"));

        _app.MapGet("/api/error", () => Results.StatusCode(500));

        _app.MapGet("/api/custom-header", (HttpContext ctx) =>
        {
            ctx.Response.Headers["X-Custom-Header"] = "custom-value";
            return Results.Ok(new { header = "present" });
        });

        // Stays pending long enough for a request to be observed before its response exists,
        // which is what IRequest.ExistingResponse is for.
        _app.MapGet("/api/slow", async () =>
        {
            await Task.Delay(2_000);
            return Results.Ok(new { slow = true });
        });

        _app.MapGet("/api/download", () =>
        {
            var content = "This is a test download file."u8.ToArray();
            return Results.File(content, "text/plain", "test-download.txt");
        });

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

