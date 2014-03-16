using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miq.Tests.Nursery
{
    [TestClass]
    public class ReservoirSampling
    {
        [TestMethod]
        public void TestMethod1()
        {
            Random random = new Random();
            var numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int pickedNumber = 0;
            int i = 0;
            foreach (var item in numbers)
            {
                i++;
                double probabilityOfPickingThisItem = 1.0d / (double)i;
                double aRandomNumber = random.NextDouble();
                if (aRandomNumber < probabilityOfPickingThisItem)
                {
                    pickedNumber = item;
                }
            }
            Assert.IsTrue(numbers.Contains(pickedNumber));
        }
    }
}
