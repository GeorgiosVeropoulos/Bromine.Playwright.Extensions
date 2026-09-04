using Bromine.Playwright.Extensions.Reason;
using NUnit.Framework;

namespace Bromine.Playwright.Extensions.Tests.Reason;

/// <summary>
/// Pure unit tests for the <see cref="Because"/> record. No browser required.
/// </summary>
[Parallelizable(ParallelScope.All)]
public class BecauseTests
{
    [Test]
    public void Constructor_SetsMessageAndArgs()
    {
        var because = new Because("the {0} must be {1}", "button", "visible");

        Assert.Multiple(() =>
        {
            Assert.That(because.Message, Is.EqualTo("the {0} must be {1}"));
            Assert.That(because.Args, Is.EqualTo(new object[] { "button", "visible" }));
        });
    }

    [Test]
    public void Constructor_WithoutArgs_YieldsEmptyArgs()
    {
        var because = new Because("no formatting needed");

        Assert.Multiple(() =>
        {
            Assert.That(because.Message, Is.EqualTo("no formatting needed"));
            Assert.That(because.Args, Is.Empty);
        });
    }

    [Test]
    public void ImplementsIBecause()
    {
        IBecause because = new Because("as an interface", 1);

        Assert.Multiple(() =>
        {
            Assert.That(because.Message, Is.EqualTo("as an interface"));
            Assert.That(because.Args, Is.EqualTo(new object[] { 1 }));
        });
    }

    [Test]
    public void Deconstruct_ReturnsMessageAndArgs()
    {
        var (message, args) = new Because("deconstructed {0}", 42);

        Assert.Multiple(() =>
        {
            Assert.That(message, Is.EqualTo("deconstructed {0}"));
            Assert.That(args, Is.EqualTo(new object[] { 42 }));
        });
    }

    [Test]
    public void Properties_AreMutable()
    {
        var because = new Because("initial")
        {
            Message = "updated {0}",
            Args = ["value"]
        };

        Assert.Multiple(() =>
        {
            Assert.That(because.Message, Is.EqualTo("updated {0}"));
            Assert.That(because.Args, Is.EqualTo(new object[] { "value" }));
        });
    }
}

