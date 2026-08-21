namespace Bromine.Playwright.Extensions.Tests.Support;

/// <summary>
/// Polls an async getter until a condition holds.
/// <para>
/// Console messages and page errors reach the driver a moment after the browser produces them,
/// so reading them straight after a click is a race. On timeout this returns the last value read
/// rather than throwing, so the caller's own assertion reports the failure.
/// </para>
/// </summary>
public static class Eventually
{
    public static async Task<T> Async<T>(
        Func<Task<T>> get,
        Func<T, bool> until,
        int timeoutMs = 5_000,
        int pollMs = 50)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
        var value = await get();

        while (!until(value))
        {
            if (DateTime.UtcNow >= deadline)
                return value;

            await Task.Delay(pollMs);
            value = await get();
        }

        return value;
    }
}
