using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miq.Tests.Nursery
{
	// See also PlanetaryPositionsTests.cs
    [TestClass]
    public class OrbitalParameters_Tests
    {

        [TestMethod]
        public void ShuttleOrbitalPeriod()
        {
            double shuttleOrbitalRadius = 6.5e6 + 380e3;
            double expectedShuttleOrbitalPeriod = 5.4e3;
            double EarthMass = 6e24;
            Assert.AreEqual(expectedShuttleOrbitalPeriod, OrbitalPeriod(EarthMass, shuttleOrbitalRadius), expectedShuttleOrbitalPeriod * 0.05);
        }

        [TestMethod]
        public void MoonOrbitalPeriod()
        {
            double moonOrbitalRadius = 385e6;
            double expectedMoonOrbitalPeriod = 2.4e6;
            double EarthMass = 6e24;
            Assert.AreEqual(expectedMoonOrbitalPeriod, OrbitalPeriod(EarthMass, moonOrbitalRadius), expectedMoonOrbitalPeriod * 0.05);
        }

        [TestMethod]
        public void EarthOrbitalPeriod()
        {
            double earthOrbitalRadius = 150e9;
            double expectedOrbitalPeriod = 365.25 * 24 * 60 * 60;
            double sunMass = 2e30;
            Assert.AreEqual(expectedOrbitalPeriod, OrbitalPeriod(sunMass, earthOrbitalRadius), expectedOrbitalPeriod * 0.05);
            var x = OrbitalPeriod(sunMass, earthOrbitalRadius);
        }


        private double OrbitalPeriod(double bigObjectMass, double smallObjectOrbitalRadius)
        {
            double G = 6.67384e-11;
            return 2.0 * System.Math.PI * System.Math.Pow(smallObjectOrbitalRadius, 3.0 / 2.0) / System.Math.Sqrt(G * bigObjectMass);
        }
    }
}
