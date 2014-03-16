using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Miq.Tests.Nursery
{
    [TestClass]
    public class DiceSpecificationTests
    {
        class RollSpec
        {
            public ushort Rolls { get; private set; }
            public ushort Faces { get; private set; }

            public RollSpec(string spec)
            {
                Match match = SpecRegex.Match(spec);
                if (match.Success)
                {
                    Rolls = ushort.Parse(match.Groups[1].Value);
                    Faces = ushort.Parse(match.Groups[2].Value);
                }
            }

            private static Regex SpecRegex = new Regex("(\\d+)D(\\d+)", RegexOptions.IgnoreCase);
        }

        [TestMethod]
        public void ConstructorBuildsACorrectObject()
        {
            RollSpec sut = new RollSpec("2d6");

            Assert.AreEqual(2, sut.Rolls);
            Assert.AreEqual(6, sut.Faces);
        }
    }
}
