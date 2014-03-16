using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Miq.Tests.MoreMathTests
{
    [TestClass]
    public class Number
    {
        [TestMethod]
        public void NumberOfDigits_Test()
        {
            Dictionary<uint, uint> expected = new Dictionary<uint, uint>()
            {
                {9, 0},
                {10, 1},
                {99, 1},
                {100, 2},
                {999, 2},
                {1000, 3},
                {9999, 3},
                {10000, 4},
            };

            foreach (var item in expected)
            {
                Assert.AreEqual(item.Value, item.Key.NumberOfDigits(), "Digits of " + item.Key);
            }
        }

        [TestMethod]
        public void DigitsOf_Works()
        {
            uint[] expected = {2, 4};
            
            uint[] actual = 42u.DigitsOf();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DigitsOf_WorksForZero()
        {
            uint[] expected = { 0 };

            uint[] actual = 0u.DigitsOf();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
