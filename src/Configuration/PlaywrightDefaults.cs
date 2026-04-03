namespace Bromine.Playwright.Extensions.Configuration;

/// <summary>
/// Global configuration defaults for Bromine Playwright Extensions.
/// Set these once at test startup to apply across all assertions and helpers.
/// </summary>
public static class PlaywrightDefaults
{
    /// <summary>
    /// Default timeout in milliseconds for assertion operations.
    /// Default: 5000ms (5 seconds).
    /// </summary>
    public static float AssertionTimeout { get; set; } = 5_000;

    /// <summary>
    /// Default timeout in milliseconds for navigation operations.
    /// Default: 30000ms (30 seconds).
    /// </summary>
    public static float NavigationTimeout { get; set; } = 30_000;

    /// <summary>
    /// Default timeout in milliseconds for action operations (click, fill, etc.).
    /// Default: 15000ms (15 seconds).
    /// </summary>
    public static float ActionTimeout { get; set; } = 15_000;

    /// <summary>
    /// Default number of retries for retry-based operations.
    /// Default: 3.
    /// </summary>
    public static int DefaultRetryCount { get; set; } = 3;

    /// <summary>
    /// Default delay between retries in milliseconds.
    /// Default: 500ms.
    /// </summary>
    public static int RetryDelayMs { get; set; } = 500;

    /// <summary>
    /// Default polling interval for polling-based waits in milliseconds.
    /// Default: 200ms.
    /// </summary>
    public static int PollIntervalMs { get; set; } = 200;

    /// <summary>
    /// Resets all defaults to their original values.
    /// </summary>
    public static void Reset()
    {
        AssertionTimeout = 5_000;
        NavigationTimeout = 30_000;
        ActionTimeout = 15_000;
        DefaultRetryCount = 3;
        RetryDelayMs = 500;
        PollIntervalMs = 200;
    }
}

