namespace Bromine.Playwright.Extensions;

public interface IBecause
{
    public string Message { get; set; }
    public object[] Args { get; set; }
}