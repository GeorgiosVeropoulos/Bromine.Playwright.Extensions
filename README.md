# Bromine.Playwright.Extensions

[![NuGet](https://img.shields.io/nuget/v/Bromine.Playwright.Extensions?logo=nuget&label=NuGet)](https://www.nuget.org/packages/Bromine.Playwright.Extensions)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Bromine.Playwright.Extensions?logo=nuget&label=Downloads)](https://www.nuget.org/packages/Bromine.Playwright.Extensions)
[![CI](https://github.com/GeorgiosVeropoulos/Bromine.Playwright.Extensions/actions/workflows/ci.yml/badge.svg)](https://github.com/GeorgiosVeropoulos/Bromine.Playwright.Extensions/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/GeorgiosVeropoulos/Bromine.Playwright.Extensions/graph/badge.svg)](https://codecov.io/gh/GeorgiosVeropoulos/Bromine.Playwright.Extensions)
[![GitHub Release](https://img.shields.io/github/v/release/GeorgiosVeropoulos/Bromine.Playwright.Extensions?logo=github&label=Release)](https://github.com/GeorgiosVeropoulos/Bromine.Playwright.Extensions/releases/latest)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A Playwright extensions library for .NET providing **fluent assertions** (`.Should()`), **browser/context builder patterns**, and **enhanced Page/Locator extension methods** for E2E test automation.

## Installation

```bash
dotnet add package Bromine.Playwright.Extensions
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Bromine.Playwright.Extensions" Version="1.0.0" />
```

---

## Features

### 1. Fluent Assertions — `.Should()`

Chain-able, readable assertions for `ILocator`, `IPage`, and `IAPIResponse`.

```csharp
using Bromine.Playwright.Extensions.Assertions;

// Locator assertions
await page.Locator("#submit-btn").Should().BeVisibleAsync();
await page.Locator("#submit-btn").Should().BeInteractableAsync();   // visible + enabled
await page.Locator(".error").Should().HaveTextAsync("Invalid input");
await page.Locator(".error").Should().ContainTextAsync("Invalid");
await page.Locator("input[name='email']").Should().HaveValueAsync("test@test.com");
await page.Locator(".items").Should().HaveCountAsync(5);
await page.Locator("#checkbox").Should().BeCheckedAsync();
await page.Locator("#field").Should().HaveAttributeAsync("disabled", "true");
await page.Locator(".alert").Should().BeVisibleWithTextAsync("Success");

// Negated assertions
await page.Locator(".spinner").Should().Not.BeVisibleAsync();
await page.Locator("#old-element").Should().Not.HaveTextAsync("deprecated");

// Page assertions
await page.Should().HaveTitleAsync("Dashboard");
await page.Should().HaveUrlContainingAsync("/dashboard");
await page.Should().HaveUrlMatchingAsync(@"\/users\/\d+");

// Response assertions
var response = await request.GetAsync("/api/users");
await response.Should().BeOkAsync();
response.Should().HaveStatus(200);
response.Should().HaveHeader("Content-Type", "application/json");
await response.Should().BodyContainsAsync("\"success\":true");

// Custom timeout per assertion chain
await page.Locator("#slow-loader").Should().WithTimeout(30_000).BeVisibleAsync();
```

### 2. Browser Builder Pattern

Create Playwright browser instances with a fluent API — no more remembering option class hierarchies.

```csharp
using Bromine.Playwright.Extensions.Builders;

// Simple headless Chromium
var (playwright, browser) = await PlaywrightBrowserBuilder.Create()
    .WithChromium()
    .Headless()
    .BuildAsync();

// Headed Firefox with slow motion for debugging
var result = await PlaywrightBrowserBuilder.Create()
    .WithFirefox()
    .Headed()
    .WithSlowMotion(100)
    .WithTimeout(15_000)
    .BuildAsync();

// Chrome channel with proxy
var result = await PlaywrightBrowserBuilder.Create()
    .WithChromium()
    .WithChannel("chrome")
    .Headless()
    .WithProxy("http://proxy.company.com:8080")
    .BuildAsync();

// Clean disposal
await using var browserSession = await PlaywrightBrowserBuilder.Create()
    .WithChromium()
    .Headless()
    .BuildAsync();
// browserSession.Browser and browserSession.Playwright are available
// both get cleaned up on DisposeAsync
```

### 3. Browser Context Builder

Build `IBrowserContext` instances with all options via fluent API.

```csharp
using Bromine.Playwright.Extensions.Builders;

// Desktop context with full options
var context = await BrowserContextBuilder.For(browser)
    .WithViewport(1920, 1080)
    .BypassCSP()
    .WithPermissions("clipboard-read", "clipboard-write")
    .WithHttpCredentials("admin", "password123")
    .WithLocale("en-US")
    .WithTimezone("Europe/Athens")
    .AcceptDownloads()
    .WithDefaultTimeout(30_000)
    .WithDefaultNavigationTimeout(30_000)
    .BuildAsync();

// Mobile emulation
var mobileContext = await BrowserContextBuilder.For(browser)
    .WithDevice(playwright.Devices["iPhone 13"])
    .AsMobile()
    .WithPermissions("geolocation")
    .WithGeolocation(37.9838f, 23.7275f)  // Athens
    .BuildAsync();

// With HAR + Video recording + Tracing
var debugContext = await BrowserContextBuilder.For(browser)
    .WithViewport(1920, 1080)
    .WithHarRecording("./traces/network.har", omitContent: true)
    .WithVideoRecording("./traces/videos", width: 1920, height: 1080)
    .WithTracing(screenshots: true, snapshots: true, sources: true)
    .BuildAsync();

// With offline mode & storage state
var offlineContext = await BrowserContextBuilder.For(browser)
    .Offline()
    .WithStorageState("./auth-state.json")
    .BuildAsync();
```

### 4. Page Extensions

Enhanced `IPage` extension methods for common operations.

```csharp
using Bromine.Playwright.Extensions.Extensions;

// Navigation
await page.NavigateAndWaitAsync("https://example.com");
await page.NavigateAndWaitForDomAsync("https://example.com");
await page.ReloadAndWaitAsync();
await page.WaitForUrlContainingAsync("/dashboard");
await page.WaitForStableStateAsync();

// Cookies
var cookie = await page.GetCookieByNameAsync("session_id");
await page.SetCookieAsync("my_cookie", "value123", domain: ".example.com");
await page.ClearCookiesAsync();

// Screenshots
var bytes = await page.FullPageScreenshotAsync();
await page.FullPageScreenshotAsync("/path/to/screenshot.png");
var base64 = await page.ScreenshotToBase64Async();

// Safe interactions
bool clicked = await page.TryClickAsync(".optional-banner-dismiss");
string? text = await page.GetVisibleTextAsync(".user-greeting");

// Scrolling
await page.ScrollToBottomAsync();
await page.ScrollToTopAsync();

// Downloads
string filePath = await page.ClickAndDownloadAsync("#export-btn", "./downloads");
```

### 5. Locator Extensions

Enhanced `ILocator` extension methods with built-in waits and retries.

```csharp
using Bromine.Playwright.Extensions.Extensions;

// Click helpers
await page.Locator("#btn").ClickWhenReadyAsync();           // waits for visible + enabled
await page.Locator("#btn").ClickWithRetryAsync();            // auto-retry on failure (3x)
await page.Locator("#btn").HoverAndClickAsync();             // hover first
await page.Locator("#btn").DoubleClickWhenReadyAsync();
await page.Locator("#btn").ClickAndWaitUntilHiddenAsync();   // click + wait for element to disappear
await page.Locator(".items li").ClickNthWhenReadyAsync(2);   // click 3rd item

// Fill helpers
await page.Locator("#email").FillWhenReadyAsync("test@test.com");
await page.Locator("#email").FillAndVerifyAsync("test@test.com");  // fill + assert value
await page.Locator("#search").TypeSlowlyAsync("playwright", delayMs: 50);

// Scroll & visibility
await page.Locator("#footer").ScrollIntoViewAsync();
bool isVisible = await page.Locator(".popup").IsVisibleWithinAsync(timeoutMs: 3000);
await page.Locator(".loader").WaitUntilHiddenAsync();
await page.Locator("#content").WaitUntilAttachedAsync();

// Dropdown selection
await page.Locator("select#country").SelectByTextAsync("Greece");
await page.Locator("select#country").SelectByValueAsync("GR");

// Get data
string text = await page.Locator(".title").GetTextWhenReadyAsync();
string? href = await page.Locator("a.link").GetAttributeWhenReadyAsync("href");

// Drag & drop
await page.Locator("#source").DragToAsync(page.Locator("#target"));

// Random selection from list
string? selectedText = await page.Locator(".dropdown-item").ClickRandomOptionAsync();
```

### 6. Global Configuration

Set default timeouts once at test startup.

```csharp
using Bromine.Playwright.Extensions.Configuration;

// In your [BeforeTestRun] or setup
PlaywrightDefaults.AssertionTimeout = 10_000;    // 10s for assertions
PlaywrightDefaults.NavigationTimeout = 60_000;   // 60s for navigation
PlaywrightDefaults.ActionTimeout = 15_000;       // 15s for actions
PlaywrightDefaults.DefaultRetryCount = 5;        // 5 retry attempts
PlaywrightDefaults.RetryDelayMs = 1_000;         // 1s between retries

// Reset to defaults
PlaywrightDefaults.Reset();
```

---

## Static Analysis — Unawaited Assertion Detection

This package ships with a **built-in Roslyn analyzer** (`BROE101`) that produces a **compile error** if you forget to `await` a fluent assertion chain.

```csharp
// ❌ BROE101: This fluent assertion chain is not awaited and will never execute.
locator.Should().BeVisibleAsync();

// ✅ Correct
await locator.Should().BeVisibleAsync();
```

No configuration needed — the analyzer is bundled in the NuGet package and activates automatically.

---

## Running Tests

```bash
# Build
dotnet build -c Release

# Install Playwright browsers
pwsh tests/bin/Release/net8.0/playwright.ps1 install chromium --with-deps

# Run tests
dotnet test tests/Bromine.Playwright.Extensions.Tests.csproj -c Release --no-build
```

> Tests run **headed** locally and **headless** in CI (detected via the `CI` environment variable).

---

## CI / CD

### Pipelines

| Workflow | Trigger | Description |
|---|---|---|
| **CI** | Push / PR to `main` | Build → Install browsers → Run tests |
| **Publish** | Tag push (`v*`) | Run tests → Pack → Push to nuget.org → GitHub Release |
| **Update Playwright** | Weekly (Monday 8 AM UTC) | Check for new Playwright version → Update → Test → Open PR |

### Publishing a New Version

```bash
git tag v1.0.1
git push origin v1.0.1
```

The tag version (`1.0.1`) becomes the NuGet package version automatically. Tests must pass before the package is published.

### Setup

Add `NUGET_API_KEY` as a repository secret (Settings → Secrets → Actions). Get your key from [nuget.org/account/apikeys](https://www.nuget.org/account/apikeys).

---

## Building & Packing Locally

### Build

```bash
dotnet build -c Release
```

### Pack

```bash
dotnet pack src/Bromine.Playwright.Extensions.csproj -c Release -o ./nupkgs
```

The `.nupkg` includes the library DLL, XML docs, and the bundled Roslyn analyzer.

### Install Locally in Another Project

```bash
# Add the local folder as a NuGet source (once)
dotnet nuget add source /path/to/nupkgs --name LocalBromine

# Install
dotnet add package Bromine.Playwright.Extensions --version 1.0.0
```

---

## Requirements

- .NET 8.0+
- Microsoft.Playwright 1.56.0+

## License

MIT

