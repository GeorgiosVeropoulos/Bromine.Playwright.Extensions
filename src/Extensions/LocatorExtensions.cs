using Bromine.Playwright.Extensions.Configuration;
using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Extensions;

/// <summary>
/// Extension methods for <see cref="ILocator"/> providing aria snapshot and
/// locator-normalisation helpers.
/// </summary>
public static class LocatorExtensions
{
    /// <summary>
    /// Capture the aria snapshot of the matched element.
    /// <para>
    /// <paramref name="depth"/> is passed through, but note that Playwright 1.59 only honours it
    /// for page-level <see cref="AriaSnapshotMode.Ai"/> snapshots — it had no observable effect
    /// on locator snapshots in either mode. Requires Playwright 1.59 or newer.
    /// </para>
    /// </summary>
    public static async Task<string> GetAriaSnapshotAsync(
        this ILocator locator,
        int? depth = null,
        AriaSnapshotMode? mode = null,
        float? timeoutMs = null)
    {
        return await locator.AriaSnapshotAsync(new LocatorAriaSnapshotOptions
        {
            Depth = depth,
            Mode = mode,
            Timeout = timeoutMs ?? PlaywrightDefaults.ActionTimeout
        });
    }

    /// <summary>
    /// Capture the aria snapshot of the matched element in the AI-optimised shape
    /// (<see cref="AriaSnapshotMode.Ai"/>), for handing element structure to a model.
    /// </summary>
    public static Task<string> GetAriaSnapshotForAiAsync(
        this ILocator locator,
        int? depth = null,
        float? timeoutMs = null)
        => locator.GetAriaSnapshotAsync(depth, AriaSnapshotMode.Ai, timeoutMs);

    /// <summary>
    /// Rewrite this locator to follow Playwright's recommended practices — test ids and aria
    /// roles ahead of CSS — and return the selector it resolves to.
    /// <para>
    /// Handy for logging or for migrating a brittle CSS selector: point it at an element, print
    /// what comes back, and paste that into the test. Requires Playwright 1.59 or newer.
    /// </para>
    /// </summary>
    public static async Task<string> NormalizedSelectorAsync(this ILocator locator)
    {
        var normalized = await locator.NormalizeAsync();

        // ToString() is annotated nullable on object; Playwright's locators always render a
        // selector, so an empty string is a safer contract here than handing back null.
        return normalized.ToString() ?? string.Empty;
    }
}
