using FakeItEasy;
using NUnit.Framework;

namespace Denxorz.Latch.Tests;

[TestFixture]
public class LatchTests
{
    [Test]
    public void Ctor_IsNotLatched()
    {
        // Arrange
        var classUnderTest = new Latch();

        // Act

        // Assert
        Assert.That(classUnderTest.IsLatched, Is.False);
    }

    [Test]
    public void RunInsideLatchd_GiveBar_BarIsCalled()
    {
        // Arrange
        var classUnderTest = new Latch();
        var foo = A.Fake<IFoo>();

        // Act
        classUnderTest.RunInsideLatch(foo.Bar);

        // Assert
        A.CallTo(() => foo.Bar()).MustHaveHappened();
    }

    [Test]
    public void RunInsideLatch_CheckWhileLatching_IsLatchedIsTrue()
    {
        // Arrange
        var classUnderTest = new Latch();

        // Act
        bool isLatched = false;
        classUnderTest.RunInsideLatch(() => isLatched = classUnderTest.IsLatched);

        // Assert
        Assert.That(isLatched, Is.True);
    }

    [Test]
    public void RunInsideLatch_CallBarAfterLatched_BarIsCalled()
    {
        // Arrange
        var classUnderTest = new Latch();
        var foo = A.Fake<IFoo>();

        // Act
        classUnderTest.RunInsideLatch(() => { });
        classUnderTest.RunInsideLatch(foo.Bar);

        // Assert
        A.CallTo(() => foo.Bar()).MustHaveHappened();
    }

    [Test]
    public void RunInsideLatch_CallInLatch_BarIsNotCalled()
    {
        // Arrange
        var classUnderTest = new Latch();
        var foo = A.Fake<IFoo>();

        // Act
        classUnderTest.RunInsideLatch(() => classUnderTest.RunInsideLatch(foo.Bar));

        // Assert
        A.CallTo(() => foo.Bar()).MustNotHaveHappened();
    }

    [Test]
    public void RunInsideLatch_BarThrows_IsLatchedIsFalseAfterBarAndExceptionWasNotSwallowed()
    {
        // Arrange
        var classUnderTest = new Latch();
        var foo = A.Fake<IFoo>();

        A.CallTo(() => foo.Bar()).Throws(new DivideByZeroException());

        // Act
        Assert.Throws(typeof(DivideByZeroException), () => classUnderTest.RunInsideLatch(foo.Bar));

        // Assert
        Assert.That(classUnderTest.IsLatched, Is.False);
    }
}