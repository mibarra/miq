using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miq.Tests.ProjectEuler
{
    public partial class Solutions
    {
        public uint TriangularNumer(uint n)
        {
            return n * (n + 1) / 2;
        }

        public double TriangularRoot(uint n)
        {
            return (System.Math.Sqrt(8 * n + 1) - 1) / 2;
        }

        // 2n^2 - 2n - O = 0

        // n = (2 +- sqrt(4 + 8O)) / 4
    }

    [TestClass]
    public class P61
    {
        [TestMethod]
        public void TestMethod1()
        {
            var sut = new Solutions();
            Assert.AreEqual(15u, sut.TriangularNumer(5));
            // What is the first n for which polygonalNumber(3, n) is four digits long?
            var x2 = sut.TriangularRoot(1000);
            var x3 = sut.TriangularRoot(9999);

            // Triangular numbers with 4 digits... 45 .. 140
        }
    }
}
