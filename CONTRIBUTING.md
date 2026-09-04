# Contributing to Bromine.Playwright.Extensions

Thanks for helping! Contributions of every size are welcome — covering new Playwright APIs,
fixing bugs, improving failure messages, and clarifying docs.

## Getting started

You need the **.NET 8 SDK** and **PowerShell** (`pwsh`, used by Playwright's install script).

```bash
git clone https://github.com/GeorgiosVeropoulos/Bromine.Playwright.Extensions.git
cd Bromine.Playwright.Extensions
dotnet build
pwsh tests/bin/Debug/net8.0/playwright.ps1 install chromium firefox webkit
```

## Running the tests

```bash
dotnet test tests/Bromine.Playwright.Extensions.Tests.csproj
```

Every fixture derives `TestBase` and runs once per engine — Chromium, Firefox and WebKit — with
engines that aren't installed skipped automatically. Filter to one engine with
`--filter "TestCategory=Chromium"` (or `Firefox` / `Webkit`).

Local knobs live in a gitignored `tests/testsettings.local.json` (copy
`testsettings.local.example.json`) or environment variables, which win over the file:

| Variable | Effect |
|---|---|
| `PW_HEADED=1` | show the browser window |
| `PW_SLOWMO=250` | slow every Playwright operation down (ms) |
| `PW_ENGINES=Chromium,Firefox` | run only these engines |

> **CI runs Chromium only, on Linux.** Firefox and WebKit are yours to verify locally before
> opening a PR — please run the full suite on all three engines, and headed
> (`PW_HEADED=1`) for anything console- or timing-sensitive.

## Design rules

These keep the library coherent; PRs that follow them merge much faster.

- **Every wrapper must earn its place.** Don't wrap a Playwright method just to rename it. A
  helper is justified when it combines calls, adds a precondition with a better error, provides a
  real scope (start/act/stop in a `finally`), or fixes an ergonomic trap in the raw API.
- **Fluent assertions** derive `FluentBase<TSelf>`, capture `NegateNext` into a local *before*
  `AddStep`, and end with `string because = "", params object[] becauseArgs`. Delegate to
  Playwright's own `Expect(...)` matchers whenever one exists. Failure messages must carry the
  expected value, the actual state, and the page URL — they are the whole debugging story on CI.
- **Public API changes are additive only.** Adding an optional parameter to an existing public
  method is binary-breaking for compiled consumers — add a sibling method or an overload instead.
- **Probe before you wrap.** Never expose an upstream API you haven't called on all three engines;
  several have shipped broken or inert (see the caveats tables in the README). A known upstream
  defect is recorded in the README, **not** pinned with a test — such a test only fails when
  Playwright fixes the bug, turning good news into a red build.
- **Timeouts come from `PlaywrightDefaults`**, never hard-coded.

## Tests

- Name them `Method_ShouldOutcome_WhenCondition`.
- For a new assertion, cover: pass, throw (with the message asserted), `Not.` pass, `Not.` throw,
  `because` propagation, and chaining — the existing `tests/Tests/Should/` fixtures are the
  template.
- Need a new server behaviour? Add a route or page to `TestServerFixture` / `tests/wwwroot`.
- Test the wrapper boundary (option translation, targeting, lifetimes, that the option reached
  Playwright at all) — not Playwright's own semantics.

## Versioning and releases

The package **minor tracks the Playwright minor**: Playwright 1.60 → 1.4.0, 1.61 → 1.5.0.
Releases are cut by the maintainer from an annotated tag (`vX.Y.Z`) on master; the tag message
becomes the GitHub Release body and the NuGet release notes, and the publish workflow pushes the
package. You don't need to touch versions in a PR unless it's the Playwright bump itself.

## Reporting issues

Before filing, check the README's *"Caveats found while testing"* tables — several surprising
behaviours are documented upstream Playwright defects, not bugs in this library (those belong at
[microsoft/playwright-dotnet](https://github.com/microsoft/playwright-dotnet/issues)).

For a bug here, please include:

- the package version and the `Microsoft.Playwright` version,
- the engine(s) it reproduces on,
- a minimal repro — ideally a failing test derived from `TestBase`.
