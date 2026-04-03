namespace Bromine.Playwright.Extensions.Reason;

public record Because(string Message, params object[] Args) : IBecause
{
    public string Message { get; set; } = Message;
    public object[] Args { get; set; } = Args;
}