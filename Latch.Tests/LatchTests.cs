using System;
using FakeItEasy;
using NUnit.Framework;

namespace Denxorz.Latch.Tests
{
    [TestFixture]
    public class LatchTests
    {
        [Test]
        public void ctor_IsNotLatched()
        {
            // Arrange
            var classUnderTest = new Latch();

            // Act

            // Assert
            Assert.IsFalse(classUnderTest.IsLatched);
        }

        [Test]
        public void RunLatched_GiveBar_BarIsCalled()
        {
            // Arrange
            var classUnderTest = new Latch();
            var foo = A.Fake<IFoo>();

            // Act
            classUnderTest.RunLatched(foo.Bar);

            // Assert
            A.CallTo(() => foo.Bar()).MustHaveHappened();
        }

        [Test]
        public void RunLatched_CheckWhileLatching_IsLatchedIsTrue()
        {
            // Arrange
            var classUnderTest = new Latch();

            // Act
            bool isLatched = false;
            classUnderTest.RunLatched(() => isLatched = classUnderTest.IsLatched);

            // Assert
            Assert.IsTrue(isLatched);
        }

        [Test]
        public void RunLatched_CallBarAfterLatched_BarIsCalled()
        {
            // Arrange
            var classUnderTest = new Latch();
            var foo = A.Fake<IFoo>();

            // Act
            classUnderTest.RunLatched(() => { });
            classUnderTest.RunLatched(foo.Bar);

            // Assert
            A.CallTo(() => foo.Bar()).MustHaveHappened();
        }

        [Test]
        public void RunLatched_CallInLatch_BarIsNotCalled()
        {
            // Arrange
            var classUnderTest = new Latch();
            var foo = A.Fake<IFoo>();

            // Act
            classUnderTest.RunLatched(() => classUnderTest.RunLatched(foo.Bar));

            // Assert
            A.CallTo(() => foo.Bar()).MustNotHaveHappened();
        }

        [Test]
        public void RunLatched_BarThrows_IsLatchedIsFalseAfterBarAndExceptionWasNotSwallowed()
        {
            // Arrange
            var classUnderTest = new Latch();
            var foo = A.Fake<IFoo>();

            A.CallTo(() => foo.Bar()).Throws(new DivideByZeroException());

            // Act
            Assert.Throws(typeof(DivideByZeroException), () => classUnderTest.RunLatched(foo.Bar));

            // Assert
            Assert.IsFalse(classUnderTest.IsLatched);
        }
    }
}