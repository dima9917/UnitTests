using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImpromptuInterface;

namespace UnitTests
{
    public interface Ilog
    {
        bool Write(string msg);
    }

    public class ConsoleLog : Ilog
    {
        public bool Write(string msg)
        {
            Console.WriteLine(msg);
            return true;
        }
    }
    public class NullLog : Ilog
    {//Static Fakes
        public bool Write(string msg)
        {
            return true;
        }
    }

    public class NullLogWithResult : Ilog
    {//Stubs
        bool expectedResult;
        public NullLogWithResult(bool expectedResult)
        {
            this.expectedResult = expectedResult;
        }
        public bool Write(string msg)
        {
            return expectedResult;
        }
    }

    public class LogMock : Ilog
    {//Mocks
        bool expectedResult;
        public Dictionary<string, int> MethodCallCount;
        public LogMock(bool expectedResult)
        {
            this.expectedResult = expectedResult;
            MethodCallCount = new Dictionary<string, int>();
        }

        private void AddOrIncrement(string methodName)
        {
            if (MethodCallCount.ContainsKey(methodName)) MethodCallCount[methodName]++;
            else MethodCallCount.Add(methodName, 1);
        }
        public bool Write(string msg)
        {
            AddOrIncrement(nameof(Write));
            return expectedResult;
        }
    }

    public class Null<T>:DynamicObject where T : class
    {//Dynamic Fakes with ImpromptuInterface
        public static T Instance
        {
            get
            {
                return new Null<T>().ActLike<T>();
            }
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, 
            object[] args, out object result)
        {
            result = Activator.CreateInstance(
                typeof(T).GetMethod(binder.Name).ReturnType
                );
            return true;
        }
    }

    class BankAccount
    {
        
        public int Balance { get; set; }
        private readonly Ilog log;
        public BankAccount(Ilog log)
        {
            this.log = log;
        }

        public void Deposit(int amount)
        {
            if (log.Write($"Depositing {amount}"))
                Balance += amount;
        }

    }

    [TestFixture]
    public class BankAccountTests
    {
        BankAccount ba;

        [Test]
        public void DepositIntegrationTest()
        {
            ba = new BankAccount(new ConsoleLog()) { Balance = 100 };
            ba.Deposit(100);
            Assert.That(ba.Balance, Is.EqualTo(200));
        }

        [Test]
        public void DepositIntegrationTestWithStub()
        {
            ba = new BankAccount(new NullLogWithResult(true)) { Balance = 100 };
            ba.Deposit(100);
            Assert.That(ba.Balance, Is.EqualTo(200));
        }

        [Test]
        [Ignore("fuck you")]
        public void DepositIntegrationTestWithDynamicFake()
        {
            var log = Null<Ilog>.Instance;
            ba = new BankAccount(log) { Balance = 100 };
            ba.Deposit(100);
            Assert.That(ba.Balance, Is.EqualTo(200));
        }

        [Test]
        public void DepositTestWithMock()
        {
            var log = new LogMock(true);
            ba = new BankAccount(log) { Balance = 100 };
            ba.Deposit(100);
            Assert.Multiple(() =>
            {
                Assert.That(ba.Balance, Is.EqualTo(200));
                Assert.That(
                    log.MethodCallCount[nameof(LogMock.Write)],
                    Is.EqualTo(1)
                    );
            });
        }
    }


}
