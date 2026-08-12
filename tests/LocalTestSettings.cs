using System.Text.Json;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests;

/// <summary>
/// Per-developer knobs for a local test run.
/// <para>
/// Headless is the default everywhere, including locally. A full run is the fixture count
/// times the number of engines, and every test gets its own browser, so running headed by
/// default opens hundreds of windows that fight for focus — which stalls screenshot capture
/// and download interception. Opt into headed explicitly when you actually want to watch.
/// </para>
/// <para>
/// Precedence, highest first: environment variable, then <c>testsettings.local.json</c>,
/// then the defaults below. The JSON file is gitignored — copy
/// <c>testsettings.local.example.json</c> next to it to create your own.
/// </para>
/// </summary>
internal sealed class LocalTestSettings
{
    private const string FileName = "testsettings.local.json";

    /// <summary>Show the browser window. Env override: <c>PW_HEADED=1</c>.</summary>
    public bool Headed { get; init; }

    /// <summary>Slow every Playwright operation down by this many ms. Env override: <c>PW_SLOWMO=250</c>.</summary>
    public float SlowMoMs { get; init; }

    /// <summary>
    /// Engines to actually run; the rest are skipped. Null or empty means all of them.
    /// Env override: <c>PW_ENGINES=Chromium,Firefox</c>.
    /// </summary>
    public string[]? Engines { get; init; }

    private static readonly Lazy<LocalTestSettings> Lazy = new(Load);

    public static LocalTestSettings Current => Lazy.Value;

    /// <summary>
    /// Whether the given engine should run under the current settings.
    /// </summary>
    public bool ShouldRun(BrowserType browser) =>
        Engines is null or { Length: 0 } ||
        Engines.Any(e => string.Equals(e.Trim(), browser.ToString(), StringComparison.OrdinalIgnoreCase));

    public string EnabledEngines =>
        Engines is null or { Length: 0 } ? "all" : string.Join(", ", Engines);

    private static LocalTestSettings Load()
    {
        var file = ReadFile() ?? new LocalTestSettings();

        var settings = new LocalTestSettings
        {
            Headed = EnvBool("PW_HEADED") ?? file.Headed,
            SlowMoMs = EnvFloat("PW_SLOWMO") ?? file.SlowMoMs,
            Engines = EnvList("PW_ENGINES") ?? file.Engines
        };

        TestContext.Progress.WriteLine(
            $"Test run: headed={settings.Headed}, slowMo={settings.SlowMoMs}ms, engines={settings.EnabledEngines}");

        return settings;
    }

    /// <summary>
    /// Walks up from the test assembly towards the repo root looking for the settings file,
    /// so a gitignored local file works without being copied into bin/.
    /// </summary>
    private static LocalTestSettings? ReadFile()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        for (var i = 0; i < 8 && dir != null; i++, dir = dir.Parent)
        {
            var path = Path.Combine(dir.FullName, FileName);
            if (!File.Exists(path))
                continue;

            try
            {
                return JsonSerializer.Deserialize<LocalTestSettings>(
                    File.ReadAllText(path),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true
                    });
            }
            catch (JsonException ex)
            {
                // Never fail the whole suite over a malformed local file — fall back to defaults.
                TestContext.Progress.WriteLine($"Ignoring invalid {path}: {ex.Message}");
                return null;
            }
        }

        return null;
    }

    private static bool? EnvBool(string name) => Environment.GetEnvironmentVariable(name) switch
    {
        null or "" => null,
        var v when v.Equals("0", StringComparison.Ordinal) => false,
        var v when v.Equals("false", StringComparison.OrdinalIgnoreCase) => false,
        _ => true
    };

    private static float? EnvFloat(string name) =>
        float.TryParse(Environment.GetEnvironmentVariable(name), out var value) ? value : null;

    /// <summary>
    /// Accepts comma-, space- or semicolon-separated engine names, so the same value can be
    /// passed straight to <c>playwright.ps1 install</c> (which wants spaces) and to this
    /// setting — letting CI declare the engine list exactly once.
    /// </summary>
    private static string[]? EnvList(string name) => Environment.GetEnvironmentVariable(name) switch
    {
        null or "" => null,
        var v => v.Split([',', ' ', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    };
}
