using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using Miq;

namespace Miq.Tests.Math
{
    [TestClass]
    public class RomanNumerals
    {
        [TestMethod]
        public void ToRoman_0To9_ReturnsCorrectly()
        {
            string[] expected = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
            for (uint i = 1; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i-1], i.ToRoman());
            }
        }

        [TestMethod]
        public void ToRoman_10To90_ReturnsCorrectly()
        {
            string[] expected = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
            for (uint i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], (i * 10).ToRoman(), "Roman for " + (i * 10));
            }
        }

        [TestMethod]
        public void ToRoman_MiscTests()
        {
            Dictionary<uint, string> expected = new Dictionary<uint,string>()
            {
                {31, "XXXI"}, {24, "XXIV"}, {369, "CCCLXIX"},
                {448, "CDXLVIII"}, {1998, "MCMXCVIII"}, {2751, "MMDCCLI"}
            };

            foreach (var item in expected)
            {
                Assert.AreEqual(item.Value, item.Key.ToRoman(), "Roman for " + item.Key);
            }
        }

        [TestMethod]
        public void ToRoman_BigNumbers()
        {
            Assert.AreEqual("M̄M̄M̄D̄C̄C̄L̄X̄X̄MV̄CMXCII", 3774992u.ToRoman(), "Roman for 3774992");
            Assert.AreEqual("M̄M̄M̄C̄M̄X̄C̄MX̄CMXCIX", 3999999u.ToRoman(), "Roman for 3999999");
        }

        [TestMethod]
        public void ToRoman_4000()
        {
            Assert.AreEqual("", "");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void ToRoman_ToBigOfANumber()
        {
            4000000u.ToRoman();
        }

        [TestMethod]
        public void RomanToArabic_()
        {
            
        }
    }
}
