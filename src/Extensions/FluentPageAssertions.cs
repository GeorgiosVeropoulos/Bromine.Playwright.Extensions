using System.Text.RegularExpressions;
using Bromine.Playwright.Extensions.Configuration;
using Bromine.Playwright.Extensions.Reason;
using Microsoft.Playwright;

namespace Bromine.Playwright.Extensions.Extensions;

public class FluentPageAssertions : FluentBase<FluentPageAssertions>
{
    private readonly IPage _page;
    
    public FluentPageAssertions(IPage pagem, bool negateNext = false)
    {
        _page = pagem;
        NegateNext = negateNext;
    }
    
    
    
    /// <summary>
    /// Asserts that the page has the given title.
    /// See <see cref="IPageAssertions.ToHaveTitleAsync(string, PageAssertionsToHaveTitleOptions?)"/>.
    /// </summary>
    public FluentPageAssertions HaveTitleAsync(string titleOrRegExp, PageAssertionsToHaveTitleOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page).Not.ToHaveTitleAsync(titleOrRegExp, options)
            : Expect(_page).ToHaveTitleAsync(titleOrRegExp, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the page has a title matching the given regex.
    /// See <see cref="IPageAssertions.ToHaveTitleAsync(Regex, PageAssertionsToHaveTitleOptions?)"/>.
    /// </summary>
    public FluentPageAssertions HaveTitleAsync(Regex titleOrRegExp, PageAssertionsToHaveTitleOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page).Not.ToHaveTitleAsync(titleOrRegExp, options)
            : Expect(_page).ToHaveTitleAsync(titleOrRegExp, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the page has the given URL.
    /// See <see cref="IPageAssertions.ToHaveURLAsync(string, PageAssertionsToHaveURLOptions?)"/>.
    /// </summary>
    public FluentPageAssertions HaveURLAsync(string expectedUrl, PageAssertionsToHaveURLOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page).Not.ToHaveURLAsync(expectedUrl, options)
            : Expect(_page).ToHaveURLAsync(expectedUrl, options),
            new Because(because, becauseArgs));
        return this;
    }
    
    /// <summary>
    /// Asserts that the page has a URL matching the given regex.
    /// See <see cref="IPageAssertions.ToHaveURLAsync(Regex, PageAssertionsToHaveURLOptions?)"/>.
    /// </summary>
    public FluentPageAssertions HaveURLAsync(Regex urlOrRegExp, PageAssertionsToHaveURLOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page).Not.ToHaveURLAsync(urlOrRegExp, options)
            : Expect(_page).ToHaveURLAsync(urlOrRegExp, options),
            new Because(because, becauseArgs));
        return this;
    }

    /// <summary>
    /// Asserts that the page's accessibility tree matches the expected aria snapshot.
    /// <para>
    /// Snapshots the whole document, which is what <c>Page.AriaSnapshotAsync()</c> is defined as,
    /// and matches it with Playwright's own subset-matching rules — so the expected snapshot only
    /// has to describe the parts you care about. Requires Playwright 1.59 or newer.
    /// </para>
    /// See <see cref="ILocatorAssertions.ToMatchAriaSnapshotAsync"/>.
    /// </summary>
    public FluentPageAssertions MatchAriaSnapshotAsync(string expected, LocatorAssertionsToMatchAriaSnapshotOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page.Locator("body")).Not.ToMatchAriaSnapshotAsync(expected, options)
            : Expect(_page.Locator("body")).ToMatchAriaSnapshotAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }

    /// <summary>
    /// Asserts that the page logged no <c>error</c>-level console messages.
    /// <para>
    /// Checked once rather than retried: console history only grows, so waiting could never turn
    /// a failure into a pass. Pair with <see cref="PageExtensions.ClearConsoleAsync"/> to scope the
    /// assertion to one action, or pass <paramref name="sinceNavigationOnly"/> to ignore anything
    /// logged before the last navigation. Requires Playwright 1.59 or newer.
    /// </para>
    /// </summary>
    public FluentPageAssertions HaveNoConsoleErrorsAsync(bool sinceNavigationOnly = false, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(async () =>
        {
            var errors = await _page.GetConsoleErrorsAsync(sinceNavigationOnly);
            var hasErrors = errors.Count > 0;

            if (negate ? !hasErrors : hasErrors)
            {
                if (negate)
                {
                    throw new PlaywrightException(
                        $"Expected page to log at least one console error, but none were recorded. URL: {_page.Url}");
                }

                var detail = string.Join("\n  ", errors.Select(e => e.Text));
                throw new PlaywrightException(
                    $"Expected no console errors, but found {errors.Count}:\n  {detail}\nURL: {_page.Url}");
            }
        }, new Because(because, becauseArgs));
        return this;
    }

    /// <summary>
    /// Asserts that no uncaught exception reached the page.
    /// <para>
    /// Checked once, for the same reason as <see cref="HaveNoConsoleErrorsAsync"/>.
    /// Requires Playwright 1.59 or newer.
    /// </para>
    /// </summary>
    public FluentPageAssertions HaveNoPageErrorsAsync(string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(async () =>
        {
            var errors = await _page.PageErrorsAsync();
            var hasErrors = errors.Count > 0;

            if (negate ? !hasErrors : hasErrors)
            {
                if (negate)
                {
                    throw new PlaywrightException(
                        $"Expected page to raise at least one uncaught error, but none were recorded. URL: {_page.Url}");
                }

                var detail = string.Join("\n  ", errors);
                throw new PlaywrightException(
                    $"Expected no page errors, but found {errors.Count}:\n  {detail}\nURL: {_page.Url}");
            }
        }, new Because(because, becauseArgs));
        return this;
    }

    /// <summary>
    /// Asserts that some console message contains <paramref name="expectedSubstring"/>.
    /// <para>
    /// Retried until <see cref="PlaywrightDefaults.AssertionTimeout"/> elapses, because the
    /// message being waited for is usually still in flight when the assertion runs.
    /// Requires Playwright 1.59 or newer.
    /// </para>
    /// </summary>
    public FluentPageAssertions HaveConsoleMessageAsync(string expectedSubstring, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(async () =>
        {
            var lastSeen = new List<string>();

            await PollUntilAsync(
                async () =>
                {
                    var messages = await _page.GetConsoleMessagesAsync();
                    lastSeen = messages.Select(m => m.Text).ToList();
                    var found = lastSeen.Any(t => t.Contains(expectedSubstring, StringComparison.Ordinal));
                    return negate ? !found : found;
                },
                () =>
                {
                    if (negate)
                    {
                        return $"Expected no console message containing '{expectedSubstring}', but one was logged. URL: {_page.Url}";
                    }

                    var detail = lastSeen.Count == 0
                        ? "no console messages were recorded"
                        : $"recorded messages were:\n  {string.Join("\n  ", lastSeen)}";
                    return $"Expected a console message containing '{expectedSubstring}', but {detail}\nURL: {_page.Url}";
                });
        }, new Because(because, becauseArgs));
        return this;
    }

    /// <summary>
    /// Polls <paramref name="condition"/> until it holds or the assertion timeout elapses,
    /// mirroring how Playwright's own web-first assertions retry.
    /// </summary>
    private static async Task PollUntilAsync(Func<Task<bool>> condition, Func<string> failureMessage)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(PlaywrightDefaults.AssertionTimeout);

        while (true)
        {
            if (await condition().ConfigureAwait(false))
                return;

            if (DateTime.UtcNow >= deadline)
                throw new PlaywrightException(failureMessage());

            await Task.Delay(PlaywrightDefaults.PollIntervalMs).ConfigureAwait(false);
        }
    }
}