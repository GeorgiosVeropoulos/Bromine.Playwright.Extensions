using System.Diagnostics.CodeAnalysis;
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
    /// Matched with Playwright's own subset-matching rules, so the expected snapshot only has to
    /// describe the parts you care about.
    /// </para>
    /// <para>
    /// Backed by the native page-level assertion from Playwright 1.60. Before that this had to
    /// snapshot the <c>body</c> locator instead, which is what <c>Page.AriaSnapshotAsync()</c> is
    /// defined as — the results are equivalent, but the native assertion reports against the page
    /// rather than a synthetic locator. Requires Playwright 1.60 or newer.
    /// </para>
    /// See <see cref="IPageAssertions.ToMatchAriaSnapshotAsync"/>.
    /// </summary>
    public FluentPageAssertions MatchAriaSnapshotAsync(string expected, PageAssertionsToMatchAriaSnapshotOptions? options = null, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => negate
            ? Expect(_page).Not.ToMatchAriaSnapshotAsync(expected, options)
            : Expect(_page).ToMatchAriaSnapshotAsync(expected, options),
            new Because(because, becauseArgs));
        return this;
    }

    /// <summary>
    /// Obsolete overload kept for callers written against 1.3.0, when this assertion snapshotted
    /// the <c>body</c> locator and locator options were the honest signature.
    /// <para>
    /// Deliberately takes <paramref name="options"/> as a required parameter. Were it optional,
    /// <c>MatchAriaSnapshotAsync(expected)</c> would match both overloads and fail to compile as
    /// ambiguous — and it would resolve here, warning callers who never passed options at all.
    /// Requiring it means only the calls that really do use the wrong type see the warning.
    /// </para>
    /// </summary>
    [Obsolete("Pass PageAssertionsToMatchAriaSnapshotOptions instead. This assertion now uses " +
              "Playwright's native page-level matcher, so the page option type is the correct one. " +
              "Both types carry only Timeout, so the change is mechanical.")]
    // Excluded from coverage rather than tested: it exists only to keep 1.3.0 callers compiling,
    // and it forwards to the overload above, which is covered. Adding tests for a method we are
    // trying to retire would mean maintaining them until it is removed.
    [ExcludeFromCodeCoverage]
    public FluentPageAssertions MatchAriaSnapshotAsync(string expected, LocatorAssertionsToMatchAriaSnapshotOptions options, string because = "", params object[] becauseArgs)
        => MatchAriaSnapshotAsync(
            expected,
            options is null ? null : new PageAssertionsToMatchAriaSnapshotOptions { Timeout = options.Timeout },
            because,
            becauseArgs);

    /// <summary>
    /// Asserts that the page logged at least one <c>error</c>-level console message.
    /// <para>
    /// Use <c>Not.HaveConsoleErrorsAsync()</c> to assert a clean page — that is the common case.
    /// Pair it with <see cref="PageExtensions.ClearConsoleAsync"/> to scope the assertion to one
    /// action, or pass <paramref name="sinceNavigationOnly"/> to ignore anything logged before the
    /// last navigation. Requires Playwright 1.59 or newer.
    /// </para>
    /// <para>
    /// Retries when asserting presence and checks once when negated; see
    /// <see cref="AssertLogHasEntriesAsync"/> for why.
    /// </para>
    /// </summary>
    public FluentPageAssertions HaveConsoleErrorsAsync(bool sinceNavigationOnly = false, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => AssertLogHasEntriesAsync(
                negate,
                async () => (await _page.GetConsoleErrorsAsync(sinceNavigationOnly))
                    .Select(e => e.Text)
                    .ToList(),
                found => $"Expected no console errors, but found {found.Count}:\n  {string.Join("\n  ", found)}\nURL: {_page.Url}",
                () => $"Expected the page to log a console error, but none were recorded. URL: {_page.Url}"),
            new Because(because, becauseArgs));
        return this;
    }

    /// <summary>
    /// Asserts that at least one uncaught exception reached the page.
    /// <para>
    /// Use <c>Not.HavePageErrorsAsync()</c> to assert that nothing threw. Retries when asserting
    /// presence and checks once when negated, as <see cref="HaveConsoleErrorsAsync"/> does.
    /// Requires Playwright 1.59 or newer.
    /// </para>
    /// </summary>
    public FluentPageAssertions HavePageErrorsAsync(string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;
        AddStep(() => AssertLogHasEntriesAsync(
                negate,
                async () => (await _page.PageErrorsAsync()).ToList(),
                found => $"Expected no page errors, but found {found.Count}:\n  {string.Join("\n  ", found)}\nURL: {_page.Url}",
                () => $"Expected the page to raise an uncaught error, but none were recorded. URL: {_page.Url}"),
            new Because(because, becauseArgs));
        return this;
    }

    /// <summary>
    /// Asserts presence or absence of entries in a log that only ever grows — console messages
    /// and page errors are never removed.
    /// <para>
    /// That monotonicity is why the two directions retry differently. Asserting presence retries,
    /// because the entry may not have reached the driver yet. Asserting absence is checked once:
    /// a later look can only ever find <em>more</em> entries, so retrying could never turn a
    /// failure into a pass — it would just spend the whole assertion timeout before reporting one.
    /// </para>
    /// </summary>
    private static async Task AssertLogHasEntriesAsync(
        bool negate,
        Func<Task<List<string>>> readEntries,
        Func<List<string>, string> unexpectedEntriesMessage,
        Func<string> noEntriesMessage)
    {
        if (negate)
        {
            var found = await readEntries().ConfigureAwait(false);
            if (found.Count > 0)
                throw new PlaywrightException(unexpectedEntriesMessage(found));
            return;
        }

        await PollUntilAsync(
            async () => (await readEntries().ConfigureAwait(false)).Count > 0,
            noEntriesMessage);
    }

    /// <summary>
    /// Asserts that some console message contains <paramref name="expectedSubstring"/>.
    /// <para>
    /// Retried until <see cref="PlaywrightDefaults.AssertionTimeout"/> elapses, because the
    /// message being waited for is usually still in flight when the assertion runs. Negated it is
    /// checked once, for the reason given on <see cref="AssertLogHasEntriesAsync"/>.
    /// Requires Playwright 1.59 or newer.
    /// </para>
    /// </summary>
    public FluentPageAssertions HaveConsoleMessageAsync(string expectedSubstring, string because = "", params object[] becauseArgs)
    {
        var negate = NegateNext;

        // Every message seen on the last read, kept so a failure can show what *was* logged
        // instead of only what was missing.
        var allSeen = new List<string>();

        AddStep(() => AssertLogHasEntriesAsync(
                negate,
                async () =>
                {
                    allSeen = (await _page.GetConsoleMessagesAsync()).Select(m => m.Text).ToList();
                    return allSeen
                        .Where(t => t.Contains(expectedSubstring, StringComparison.Ordinal))
                        .ToList();
                },
                _ => $"Expected no console message containing '{expectedSubstring}', but one was logged. URL: {_page.Url}",
                () =>
                {
                    var detail = allSeen.Count == 0
                        ? "no console messages were recorded"
                        : $"recorded messages were:\n  {string.Join("\n  ", allSeen)}";
                    return $"Expected a console message containing '{expectedSubstring}', but {detail}\nURL: {_page.Url}";
                }),
            new Because(because, becauseArgs));
        return this;
    }

    /// <summary>
    /// Asserts that the page's local storage has an item named <paramref name="name"/> — holding
    /// exactly <paramref name="expectedValue"/> when one is given, holding anything otherwise.
    /// <para>
    /// Retries in both directions until <see cref="PlaywrightDefaults.AssertionTimeout"/> elapses.
    /// Storage is mutable state — apps write and remove items asynchronously after the actions
    /// that trigger them — so a later look can change the outcome either way, unlike the
    /// append-only console and page-error logs. Requires Playwright 1.61 or newer.
    /// </para>
    /// </summary>
    public FluentPageAssertions HaveLocalStorageItemAsync(string name, string? expectedValue = null, string because = "", params object[] becauseArgs)
        => HaveStorageItemAsync(() => _page.LocalStorage, "local storage", name, expectedValue, because, becauseArgs);

    /// <summary>
    /// Asserts that the page's session storage has an item named <paramref name="name"/> — holding
    /// exactly <paramref name="expectedValue"/> when one is given, holding anything otherwise.
    /// Retries in both directions, as <see cref="HaveLocalStorageItemAsync"/> does.
    /// Requires Playwright 1.61 or newer.
    /// </summary>
    public FluentPageAssertions HaveSessionStorageItemAsync(string name, string? expectedValue = null, string because = "", params object[] becauseArgs)
        => HaveStorageItemAsync(() => _page.SessionStorage, "session storage", name, expectedValue, because, becauseArgs);

    private FluentPageAssertions HaveStorageItemAsync(
        Func<IWebStorage> storage,
        string storageName,
        string name,
        string? expectedValue,
        string because,
        object[] becauseArgs)
    {
        var negate = NegateNext;

        // Everything seen on the last poll, kept so a failure can show what *is* stored
        // instead of only what was missing.
        IReadOnlyList<WebStorageItem> lastItems = [];

        AddStep(() => PollUntilAsync(
                async () =>
                {
                    lastItems = await storage().ItemsAsync().ConfigureAwait(false);
                    var item = lastItems.FirstOrDefault(i => i.Name == name);
                    var matches = expectedValue is null
                        ? item is not null
                        : item is not null && item.Value == expectedValue;
                    return negate ? !matches : matches;
                },
                () => StorageFailureMessage(negate, storageName, name, expectedValue, lastItems)),
            new Because(because, becauseArgs));
        return this;
    }

    private string StorageFailureMessage(
        bool negate,
        string storageName,
        string name,
        string? expectedValue,
        IReadOnlyList<WebStorageItem> items)
    {
        var item = items.FirstOrDefault(i => i.Name == name);

        if (negate)
            return expectedValue is null
                ? $"Expected {storageName} to have no item named '{name}', but it has value '{item?.Value}'. URL: {_page.Url}"
                : $"Expected {storageName} item '{name}' not to have value '{expectedValue}', but it does. URL: {_page.Url}";

        if (item is not null)
            return $"Expected {storageName} item '{name}' to have value '{expectedValue}', but found '{item.Value}'. URL: {_page.Url}";

        var stored = items.Count == 0
            ? $"the {storageName} is empty"
            : $"stored items were:\n  {string.Join("\n  ", items.Select(i => $"{i.Name}={i.Value}"))}";
        return $"Expected a {storageName} item named '{name}', but {stored}\nURL: {_page.Url}";
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