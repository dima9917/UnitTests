using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests2
{
    class TestDoubles
    {
    }

    [TestFixture]
    public class MethodSamples
    {
        [Test]
        public void OrdinaryMethodCalls()
        {
            var mock = new Mock<IFoo>();

            mock.Setup(foo => foo.DoSomething("ping")).Returns(true);
            mock.Setup(foo => foo.DoSomething(It.IsIn("pong", "foo"))).Returns(false);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(mock.Object.DoSomething("ping"));
                Assert.IsFalse(mock.Object.DoSomething("pong"));
                Assert.IsFalse(mock.Object.DoSomething("foo"));
            });
        }

        [Test]
        public void ArgumentDependentMatching()
        {
            var mock = new Mock<IFoo>();

            mock.Setup(foo => foo.DoSomething(It.IsAny<string>()))
                .Returns(true);
            mock.Setup(foo => foo.Add(It.Is<int>(x => x % 2 == 0)))
                .Returns(true);
            mock.Setup(foo => foo.Add(It.IsInRange<int>(1, 10, Range.Inclusive)))
                .Returns(false);
            mock.Setup(foo => foo.DoSomething(It.IsRegex("[a-z]+")))
                .Returns(false);
        }
    }

    public interface IFoo
    {
        bool Add(int v);
        bool DoSomething(string v);
    }
}
