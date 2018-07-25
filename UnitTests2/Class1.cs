using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests2
{
    public interface Ilog
    {
        bool Write(string msg);
    }

    class BankAccount
    {

        public int Balance { get; set; }
        private Ilog log;
        public BankAccount(Ilog log)
        {
            this.log = log;
        }

        public void Deposit(int amount)
        {
            log.Write($"Depositing {amount}");

            Balance += amount;
        }

    }

    [TestFixture]
    public class BankAccountTests
    {
        BankAccount ba;

        [Test]
        public void DepositTest()
        {
            var log = new Mock<Ilog>();
            ba = new BankAccount(log.Object) { Balance = 100 };
            ba.Deposit(100);
            Assert.That(ba.Balance, Is.EqualTo(200));
        }
    }
}
