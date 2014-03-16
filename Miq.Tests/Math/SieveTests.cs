using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miq;

namespace Miq.Tests.Math
{
    [TestClass]
    public class SieveTests
    {
        [TestMethod]
        public void SieveTill30Works()
        {
            MoreMath.Sieve sieve = new MoreMath.Sieve(30);

            int[] primes = sieve.Primes();

            CollectionAssert.AreEqual(new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 }, primes);
        }

        [TestMethod]
        [TestCategory("Slow")]
        public void SieveTill100MillionDoesntBreak()
        {
            MoreMath.Sieve sieve = new MoreMath.Sieve(100000000);
            
            int[] primes = sieve.Primes();
            int n = primes.Length;

            Assert.AreEqual(5761455, n);
            Assert.AreEqual(99999989, primes[n - 1]);
        }
    }
}
