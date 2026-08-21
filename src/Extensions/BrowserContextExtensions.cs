using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Extensions;

/// <summary>
/// Extension methods for <see cref="IBrowserContext"/> providing storage-state and
/// lifetime helpers.
/// </summary>
public static class BrowserContextExtensions
{
    /// <summary>
    /// True while the context is still usable — the inverse of
    /// <see cref="IBrowserContext.IsClosed"/>. Requires Playwright 1.59 or newer.
    /// </summary>
    public static bool IsOpen(this IBrowserContext context) => !context.IsClosed;

    /// <summary>
    /// Replace the context's cookies, localStorage and IndexedDB with the state in
    /// <paramref name="storageStatePath"/>, without building a new context.
    /// <para>
    /// The point is switching identity mid-test — sign in as one user, swap to another — which
    /// previously meant discarding the context and its pages. Existing state is cleared first.
    /// Requires Playwright 1.59 or newer.
    /// </para>
    /// </summary>
    /// <exception cref="InvalidOperationException">The context is closing or already closed.</exception>
    /// <exception cref="FileNotFoundException">No storage state file at the given path.</exception>
    public static async Task SwitchStorageStateAsync(
        this IBrowserContext context,
        string storageStatePath)
    {
        // Both guards front-run failures that Playwright would otherwise surface from deep
        // inside the driver, where the message says nothing about which call was at fault.
        if (context.IsClosed)
        {
            throw new InvalidOperationException(
                "Cannot switch storage state: the browser context is closing or already closed.");
        }

        if (!File.Exists(storageStatePath))
        {
            throw new FileNotFoundException(
                $"Storage state file not found: {storageStatePath}", storageStatePath);
        }

        await context.SetStorageStateAsync(storageStatePath);
    }
}
