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
<PackageReference Include="Bromine.Playwright.Extensions" Version="1.5.0" />
```

---

## Features

### 1. Fluent Assertions — `.Should()`

Chain-able, readable assertions for `ILocator`, `IPage`, `IAPIResponse`, and `IResponse`.

```csharp
using Bromine.Playwright.Extensions.Assertions;

// Locator assertions
await page.Locator("#submit-btn").Should()
    .BeVisibleAsync()
    .HaveCountAsync(1)
    .HaveTextAsync("Submit");

// Negated assertions
await page.Locator(".spinner").Should().Not.BeVisibleAsync();
await page.Locator("#old-element").Should().Not.HaveTextAsync("deprecated");

// Chaining multiple assertions
await page.Locator("#submit-btn").Should()
    .BeVisibleAsync()
    .Not.BeEnabledAsync();

// "because" messages for better failure diagnostics
await page.Locator("#submit-btn").Should()
    .BeVisibleAsync(because: "the form should be loaded")
    .BeEnabledAsync(because: "the user has filled all required fields");

// Page assertions
await page.Should().HaveTitleAsync("Dashboard");
await page.Should().HaveURLAsync("https://example.com/dashboard");

// Response assertions — fully chainable, same pattern as locator assertions
var response = await request.GetAsync("/api/users");
await response.Should()
    .BeOKAsync()
    .HaveStatusAsync(200)
    .HaveHeaderValueAsync("Content-Type", "application/json")
    .BodyContainsAsync("Alice");

// Negated
await response.Should().Not.BeOKAsync();

// With because messages
await response.Should()
    .BeOKAsync(because: "the users endpoint should be healthy")
    .BodyContainsAsync("Alice", because: "Alice should be in the user list");
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

// Console and page errors (Playwright 1.59+)
var messages = await page.GetConsoleMessagesAsync();
var errors = await page.GetConsoleErrorsAsync(sinceNavigationOnly: true);
await page.ClearConsoleAsync();

// Aria snapshots (Playwright 1.59+)
string snapshot = await page.GetAriaSnapshotAsync();
string forAi = await page.GetAriaSnapshotForAiAsync(depth: 2);

// Screencast (Playwright 1.59+)
string video = await page.RecordScreencastAsync("./videos/checkout.webm", async () =>
{
    await page.Locator("#checkout").ClickAsync();
});
```

### 5. Global Configuration

Set default timeouts once at test startup.

```csharp
using Bromine.Playwright.Extensions.Configuration;

// In your [BeforeTestRun] or setup
PlaywrightDefaults.AssertionTimeout = 10_000;    // 10s for assertions  (default: 5s)
PlaywrightDefaults.NavigationTimeout = 60_000;   // 60s for navigation (default: 30s)
PlaywrightDefaults.ActionTimeout = 15_000;       // 15s for actions    (default: 15s)
PlaywrightDefaults.DefaultRetryCount = 3;        // 3 retry attempts   (default: 3)
PlaywrightDefaults.RetryDelayMs = 500;           // 500ms between retries (default: 500ms)

// Reset to defaults
PlaywrightDefaults.Reset();
```

### 6. Playwright 1.59 APIs

New assertions and builder options wrapping what 1.59 added.

```csharp
// Console / page-error assertions. The clean-page case — by far the common one — is the
// negation, and it reports immediately rather than waiting out the assertion timeout.
await page.Should().Not.HaveConsoleErrorsAsync();
await page.Should().Not.HavePageErrorsAsync();
await page.Should().Not.HaveConsoleErrorsAsync(sinceNavigationOnly: true);

// Asserting an error *did* happen retries, since it is usually still in flight.
await page.Should().HaveConsoleErrorsAsync();
await page.Should().HavePageErrorsAsync();

// Retried until the assertion timeout, since the message is usually still in flight.
await page.Should().HaveConsoleMessageAsync("checkout complete");

// Whole-page aria snapshot, matched with Playwright's own subset rules
await page.Should().MatchAriaSnapshotAsync("""
                                           - heading "Dashboard" [level=1]
                                           """);

// Navigation / network responses
var response = await page.GotoAsync("/dashboard");
await response.Should()
    .BeOKAsync()
    .HaveStatusAsync(200)
    .HaveHttpVersionAsync("HTTP/1.1");

// Artifacts that survive browser close
await using var result = await PlaywrightBrowserBuilder.Create()
    .WithArtifactsDir("./artifacts")
    .BuildAsync();

// Trace written live instead of zipped on stop
var context = await BrowserContextBuilder.For(result.Browser)
    .WithLiveTracing()
    .BuildAsync();

// Swap identity without discarding the context
await context.SwitchStorageStateAsync("./state/admin.json");

// Resilient selector for a brittle CSS one
string selector = await page.Locator("#main-heading").NormalizedSelectorAsync();

// Response already received? No await.
if (request.HasResponse()) { /* ... */ }
var maybe = await request.GetResponseAsync(timeoutMs: 500);   // null on timeout
```

#### Caveats found while testing 1.59

Verified against Microsoft.Playwright 1.59.0 on Chromium, Firefox and WebKit:

| API | Status |
|---|---|
| `AriaSnapshotOptions.Depth` | Only honoured for **page-level `Mode.Ai`** snapshots. Ignored in default mode and for locator snapshots. Use `GetAriaSnapshotForAiAsync(depth:)`. |
| `Screencast.ShowChapterAsync` | Broken in the .NET binding — the driver rejects `Page.overlayChapter`. Not wrapped. |
| `Screencast.ShowOverlayAsync` / `ShowOverlaysAsync` / `HideOverlaysAsync` | Broken in the .NET binding — driver rejects `Page.overlayShow` / `Page.overlaySetVisible`. Not wrapped. |
| `Screencast.ShowActionsAsync` / `HideActionsAsync` | Work. Wrapped as `ShowScreencastActionsAsync` / `HideScreencastActionsAsync`. |

#### Intentionally not wrapped

These 1.59 additions are interactive or dev-tooling APIs, outside what a test assertion library
should expose — they need a human, a headed browser, or an open port, and cannot be asserted in
CI. Use them directly off the Playwright objects if you need them.

- `IBrowserContext.Debugger` / `IDebugger` — attaches an interactive debugger
- `IPage.PickLocatorAsync` / `CancelPickLocatorAsync` — interactive locator picker
- `IBrowser.BindAsync` / `UnbindAsync` — exposes the browser to `playwright-cli` over a port
- `ICDPSession.Close` event — Chromium-only, low level

### 7. Playwright 1.60 APIs

```csharp
using Bromine.Playwright.Extensions.Extensions;

// Drag-and-drop onto a drop zone. Unlike SetInputFilesAsync this fires real drag events,
// so it works against dropzones with no <input type="file"> behind them.
await page.Locator("#dropzone").DropFilesAsync(["./fixtures/invoice.pdf"]);
await page.Locator("#dropzone").DropFilesAsync([
    new FilePayload { Name = "generated.csv", MimeType = "text/csv", Buffer = bytes }
]);
await page.Locator("#dropzone").DropTextAsync("pasted text");
await page.Locator("#dropzone").DropDataAsync([
    new KeyValuePair<string, string>("text/uri-list", "https://example.com/")
]);

// Outline an element for the duration of an action, so a screencast shows what it acted on
await page.Locator("#total").HighlightWhileAsync(
    () => page.Locator("#checkout").ClickAsync(),
    style: "outline: 3px solid magenta");

// HAR for just one step, rather than the whole session as WithHarRecording gives you
string har = await context.RecordHarAsync("./har/checkout.har", async () =>
{
    await page.Locator("#checkout").ClickAsync();
});

// Or start/stop explicitly, optionally filtered
await context.StartHarRecordingAsync("./har/api.har", urlFilter: "**/api/**");
await page.Locator("#refresh").ClickAsync();
await context.StopHarRecordingAsync();
```

`page.Should().MatchAriaSnapshotAsync(...)` now runs on Playwright's native page-level assertion
instead of snapshotting the `body` locator.

> **Deprecated, not broken (1.4.0):** the `options` parameter should now be
> `PageAssertionsToMatchAriaSnapshotOptions`, matching what the method actually calls. The old
> `LocatorAssertionsToMatchAriaSnapshotOptions` overload still works and still compiles — it is
> marked `[Obsolete]` and forwards to the new one, so you get a warning rather than an error:
>
> ```csharp
> // warns
> .MatchAriaSnapshotAsync(snap, new LocatorAssertionsToMatchAriaSnapshotOptions { Timeout = 5_000 })
> // preferred
> .MatchAriaSnapshotAsync(snap, new PageAssertionsToMatchAriaSnapshotOptions { Timeout = 5_000 })
> ```
>
> `MatchAriaSnapshotAsync(snap)` and `MatchAriaSnapshotAsync(snap, because: "…")` are untouched and
> warning-free — the obsolete overload takes its options parameter as required precisely so those
> calls don't resolve to it.

#### Caveats found while testing 1.60

Verified against Microsoft.Playwright 1.60.0 on Chromium, Firefox and WebKit:

| API | Status |
|---|---|
| `LocatorAssertionsToHaveCSSOptions.Pseudo` | **Silently ignored** — the assertion always reads the element's own computed style. Worse than inert: `Pseudo = Before` *passes* against the element's value and *fails* against the real `::before` value, so a test using it passes for the wrong reason. Avoid until fixed. |
| `AriaSnapshotOptions.Boxes` | Inert. Output is byte-identical with `Boxes` on and off, in both modes, page and locator. Not exposed. |
| `Locator.HighlightAsync` / `Tracing.StartHarAsync` | Work, but the returned `IAsyncDisposable` is a no-op stub, so `await using` silently does nothing. Wrapped with explicit scopes (`HighlightWhileAsync`, `RecordHarAsync`) that actually clean up. |
| `Locator.DropAsync` | Works fully — file paths, in-memory `FilePayload`, raw MIME data, and `Position`. |

#### Breaking changes in Playwright 1.60

This library uses none of these, so upgrading is safe for it — but your own test code might:

- `ExposeBindingOptions.Handle` removed from both `IPage` and `IBrowserContext` (this is the change
  announced for 1.59 that only actually landed in 1.60)
- `IPage.ExposeBindingAsync(string, Action, options)` and the `IBrowserContext` equivalents removed
- `IBrowserContext.ExposeBindingAsync<T>(string, Func<BindingSource, IJSHandle, T>)` removed
- `ILocator.HighlightAsync()` parameterless overload removed — replaced by the options overload

#### Intentionally not wrapped from 1.60

- `IBrowserContext.Download` / `FrameAttached` / `FrameDetached` / `FrameNavigated` / `PageClose` /
  `PageLoad`, and `IBrowser.Context` — events; the library wraps state and assertions, not event
  plumbing, and these are awkward to assert on in CI
- `GetByRoleOptions.Description` — a locator-factory option; this library does not wrap locator
  construction
- `Assertions.Expect(x, message)` — Playwright's new native failure message. Our `because` already
  covers this, and uniformly: it decorates custom assertions like `HaveStatusAsync` too, which the
  native parameter cannot reach
- `Program.RunWithResult`, `Helpers.RegexOptionsExtensions`, `ConnectOverCDPOptions.NoDefaults`,
  `IWebSocketRoute.Protocols`, `WebErrorLocation` — CLI, infrastructure, or areas the library does
  not cover

### 8. Playwright 1.61 APIs

```csharp
using Bromine.Playwright.Extensions.Assertions;
using Bromine.Playwright.Extensions.Extensions;

// Web storage assertions on page.LocalStorage / page.SessionStorage
await page.Should().HaveLocalStorageItemAsync("session-id");             // present, any value
await page.Should().HaveLocalStorageItemAsync("feature-flag", "on");     // exact value
await page.Should().Not.HaveLocalStorageItemAsync("legacy-token");       // absent
await page.Should().HaveSessionStorageItemAsync("wizard-step", "3");

// Screencast with any ScreencastStartOptions — an explicit video size, quality, OnFrame —
// without a parameter per option
string video = await page.RecordScreencastAsync("./videos/run.webm",
    new ScreencastStartOptions { Size = new() { Width = 1280, Height = 720 } },
    () => page.Locator("#checkout").ClickAsync());

// Action annotations with the pointer cursor marker 1.61 added
await page.ShowScreencastActionsAsync(new ScreencastShowActionsOptions
{
    Cursor = ScreencastCursor.Pointer
});
```

The storage assertions retry in **both** directions until the assertion timeout: storage is
mutable state — apps write and remove items asynchronously — so a later look can change the
outcome either way, unlike the append-only console and page-error assertions.

The options-taking screencast overloads copy the options (the caller's instance is never mutated)
and always write to the `savePath` argument — `options.Path` is overridden — so the file produced
is the one the call names, and the target directory is created as with the simple overloads.

#### Caveats found while testing 1.61

Verified against Microsoft.Playwright 1.61.0 on Chromium, Firefox and WebKit:

| API | Status |
|---|---|
| `Page.LocalStorage` / `Page.SessionStorage` | Work fully on all three engines — set/get/items/remove/clear, missing keys read as `null`, state survives same-origin navigation. |
| `ScreencastStartOptions.Size` | Works on all three engines (verified via the WebM headers of the produced videos). |
| `ScreencastShowActionsOptions.Cursor` | Works on all three engines. |
| `IAPIResponse.ServerAddrAsync` | **Returns `null` for `localhost` URLs.** Target `127.0.0.1` or a real host to get an address. |
| `IAPIResponse.SecurityDetailsAsync` | Returns `null` over plain HTTP — only meaningful for HTTPS endpoints. |
| `ScreencastFrame.Timestamp` | Delivered on all engines, but the unit and origin differ per engine (Chromium: Unix epoch ms; Firefox and WebKit: different internal clocks). Don't compare across engines. |

#### Breaking changes in Playwright 1.61

None — 1.61 adds 35 members to the .NET surface and removes nothing.

#### Intentionally not wrapped from 1.61

- `IBrowserContext.Credentials` (`ICredentials` — WebAuthn passkeys) — virtual-authenticator state
  setup rather than assertions, and historically Chromium-only; revisit if suites start testing
  passkey flows
- `IAPIResponse.ServerAddrAsync` / `SecurityDetailsAsync` — plain one-call getters that Playwright
  already makes ergonomic; nothing for an assertion library to add beyond the caveats above
- `BrowserTypeConnectOverCDPOptions.ArtifactsDir` — the builder wraps launching browsers, not
  remote CDP connection
- `ScreencastStartOptions.OnFrame` / `ScreencastFrame.Timestamp` — event plumbing; pass `OnFrame`
  yourself through the options-taking `StartScreencastAsync` overload if you need it

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

Use an **annotated** tag (`-a`). Its message becomes the release notes:

```bash
git tag -a v1.1.0 -m "Updated Microsoft.Playwright to 1.57.0"
```

```bash
git push origin v1.1.0
```

The tag version (`1.1.0`) becomes the NuGet package version automatically, and the tag message
becomes both the GitHub Release body and the package's release notes on nuget.org. Tests must
pass before the package is published.

For a longer note, pass `-m` more than once — each becomes its own paragraph:

```bash
git tag -a v1.1.0 -m "Updated Microsoft.Playwright to 1.57.0" -m "Adds the ContainClassAsync(IEnumerable<string>) overload."
```

A plain `git tag v1.1.0` (lightweight) still works, but it carries no message — the release
notes silently fall back to the tagged commit's message instead.

Tags are only safe to replace **before** they are pushed:

```bash
git tag -f -a v1.1.0 -m "New message"
```

Once a tag is pushed the publish runs, and a NuGet version can never be reused — only delisted.

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
dotnet add package Bromine.Playwright.Extensions --version 1.5.0
```

---

## Requirements

- .NET 8.0+
- Microsoft.Playwright 1.61.0+

## License

MIT

