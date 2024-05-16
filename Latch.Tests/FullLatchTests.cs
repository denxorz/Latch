using FakeItEasy;
using NUnit.Framework;

namespace Denxorz.Latch.Tests;

[TestFixture]
public class FullLatchTests
{
    [Test]
    public void Ctor_IsNotLatched()
    {
        // Arrange
        var classUnderTest = new FullLatch();

        // Act

        // Assert
        Assert.That(classUnderTest.IsLatched, Is.False);
    }

    [Test]
    public void LatchAndRun_GiveBar_BarIsCalled()
    {
        // Arrange
        var classUnderTest = new FullLatch();
        var foo = A.Fake<IFoo>();

        // Act
        classUnderTest.LatchAndRun(foo.Bar);

        // Assert
        A.CallTo(() => foo.Bar()).MustHaveHappened();
    }

    [Test]
    public void RunIfNotLatched_GiveBar_BarIsCalled()
    {
        // Arrange
        var classUnderTest = new FullLatch();
        var foo = A.Fake<IFoo>();

        // Act
        classUnderTest.RunIfNotLatched(foo.Bar);

        // Assert
        A.CallTo(() => foo.Bar()).MustHaveHappened();
    }

    [Test]
    public void LatchAndRun_CheckWhileLatching_IsLatchedIsTrue()
    {
        // Arrange
        var classUnderTest = new FullLatch();

        // Act
        bool isLatched = false;
        classUnderTest.LatchAndRun(() => isLatched = classUnderTest.IsLatched);

        // Assert
        Assert.That(isLatched, Is.True);
    }

    [Test]
    public void RunIfNotLatched_CallBarWhileLatched_BarIsNotCalled()
    {
        // Arrange
        var classUnderTest = new FullLatch();
        var foo = A.Fake<IFoo>();

        // Act
        classUnderTest.LatchAndRun(() => classUnderTest.RunIfNotLatched(foo.Bar));

        // Assert
        A.CallTo(() => foo.Bar()).MustNotHaveHappened();
    }

    [Test]
    public void RunIfNotLatched_CallBarAfterLatched_BarIsCalled()
    {
        // Arrange
        var classUnderTest = new FullLatch();
        var foo = A.Fake<IFoo>();

        // Act
        classUnderTest.LatchAndRun(() => { });
        classUnderTest.RunIfNotLatched(foo.Bar);

        // Assert
        A.CallTo(() => foo.Bar()).MustHaveHappened();
    }

    [Test]
    public void LatchAndRun_BarThrows_IsLatchedIsFalseAfterBarAndExceptionWasNotSwallowed()
    {
        // Arrange
        var classUnderTest = new FullLatch();
        var foo = A.Fake<IFoo>();

        A.CallTo(() => foo.Bar()).Throws(new DivideByZeroException());

        // Act
        Assert.Throws(typeof(DivideByZeroException), () => classUnderTest.LatchAndRun(foo.Bar));

        // Assert
        Assert.That(classUnderTest.IsLatched, Is.False);

    }
}
