using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Miq.Test.xUnit
{
    public class StringCalculatorKata
    {
        public int Add(string numbers)
        {
            string numbersOnly = NumbersOnly(numbers);
            if (string.IsNullOrEmpty(numbersOnly))
            {
                return 0;
            }

            var numbersList = numbersOnly
                        .Split(DelimitersFor(numbers), StringSplitOptions.RemoveEmptyEntries)
                        .Select(str => int.Parse(str))
                        .Where(i => i <= 1000);

            var negatives = numbersList.Where(i => i < 0);
            if (negatives.Any())
            {
                throw new ArgumentException(
                    string.Format("negatives not allowed. Found: {0}", string.Join(", ", negatives)));
            }

            return numbersList.Sum();
        }

        private static string NumbersOnly(string numbers)
        {
            string numbersOnly = numbers;

            if (numbers.StartsWith("//"))
            {
                numbersOnly = numbers.Substring(numbers.IndexOf('\n') + 1);
            }
            return numbersOnly;
        }

        private static string[] DelimitersFor(string numbers)
        {
            List<string> delimiters = new List<string>() { ",", "\n" };
            
            if (!numbers.StartsWith("//"))
            {
                return delimiters.ToArray();
            }

            string delimitersPart = numbers.Substring(2, numbers.IndexOf('\n') - 2);
            if (delimitersPart.Length == 1)
            {
                delimiters.Add(delimitersPart);
            }
            else
            {
                var strippedDel = delimitersPart.Substring(1, delimitersPart.Length - 2);
                delimiters.AddRange(
                    strippedDel.Split(new string[] {"]["}, StringSplitOptions.RemoveEmptyEntries).ToArray());
            }

            return delimiters.ToArray();
        }
    }

    [TestClass]
    public class CalculatorKataTests
    {
        private StringCalculatorKata sut;

        [TestInitialize]
        public void Initialize()
        {
            sut = new StringCalculatorKata();
        }

        [TestMethod]
        public void AddEmptyReturnsCorrectResults()
        {
            var numbers = "";

            int result = sut.Add(numbers);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void AddWithOneNumberReturnsCorrectResults()
        {
            int result = sut.Add("42");

            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void AddWithTwoNumbersReturnsCorrectResults()
        {
            int result = sut.Add("42,666");

            Assert.AreEqual(708, result);
        }

        [TestMethod]
        public void AddWithUnknowNumberOfNumbersReturnsCorrectResults()
        {
            int result = sut.Add("42,666,5,6");

            Assert.AreEqual(719, result);
        }

        [TestMethod]
        public void AddWithNewLinesReturnsCorrectResults()
        {
            var numbers = "42,666\n5,6";

            int result = sut.Add(numbers);

            Assert.AreEqual(719, result);
        }

        [TestMethod]
        public void AddWithDifferentDelimiterReturnsCorrectResults()
        {
            var numbers = "//;\n1;2";

            int result = sut.Add(numbers);

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void AddWithNegativeNumbersThrowsException()
        {
            var numbers = "//;\n-1;-2";

            try
            {
                sut.Add(numbers);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("-1"));
                Assert.IsTrue(e.Message.Contains("-2"));
                return;
            }
        }

        [TestMethod]
        public void AddWithBigNumbersReturnsCorrectResult()
        {
            var numbers = "2,1001";

            int result = sut.Add(numbers);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void AddWithLongDelimitersReturnsCorrectResult()
        {
            var numbers = "//[***]\n1***2***3";

            int result = sut.Add(numbers);

            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void AddWithMultipleDelimitersReturnsCorrectResult()
        {
            var numbers = "//[*][%]\n1*2%3";
            int result = sut.Add(numbers);
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void AddWithMultipleLongDelimitersReturnsCorrectResult()
        {
            var numbers = "//[***][;;]\n1***2;;3";
            int result = sut.Add(numbers);
            Assert.AreEqual(6, result);
        }
    }
}
