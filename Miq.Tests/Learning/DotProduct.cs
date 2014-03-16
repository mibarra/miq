using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media.Media3D;

namespace Miq.Tests.Learning
{
    [TestClass]
    public class DotProductTest
    {
        [TestMethod]
        public void ExcersizeDotProduct()
        {
            Vector3D a = new Vector3D(0, 1, 0);
            Vector3D b = new Vector3D(1, 0, 0);

            double x = Vector3D.DotProduct(a, b);
            Assert.AreEqual(x, 0d, 0.0001); 
        }
    }
}
