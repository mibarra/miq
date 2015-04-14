using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miq.PlanetaryPositions;

namespace Miq.Tests.Nursery
{
	[TestClass]
	public class PlanetaryPositionsTests
	{
		[TestMethod]
		[TestCategory("PlanetaryPositions")]
		public void SunPositionTest()
		{
			var date = new DayNumber(1990, 4, 19);

			var sun = new SunPosition();
			var scandinaviaAtMidnightApril19th1990 = new EarthLocation(15.0, 60.0, date, 0.0);
			sun.ObserverLocationOnEarth = scandinaviaAtMidnightApril19th1990;

			Assert.AreEqual(105.9134, sun.TrueAnomaly, 0.0001);
			Assert.AreEqual(104.9904, sun.EccentricAnomaly, 0.001);
			Assert.AreEqual(104.0653, sun.MeanAnomaly, 0.0001);

			Assert.AreEqual(1.0, sun.MeanDistance, 0.0001);
			Assert.AreEqual(0.016713, sun.Eccentricity, 0.0001);
			Assert.AreEqual(282.7735, sun.AscendingPerihelionAngle, 0.0001);
			Assert.AreEqual(26.8388, sun.MeanLongitude, 0.0001);
			Assert.AreEqual(23.4406, sun.ObliquityOfTheEcliptic, 0.0001);
			Assert.AreEqual(1.004323, sun.Distance, 0.0001);
			Assert.AreEqual(28.6869, sun.Longitude, 0.0001);

			Coordinates ecliptic = sun.EclipticCoordinates;
			Assert.AreEqual(0.881048, ecliptic.X, 0.0001);
			Assert.AreEqual(0.482098, ecliptic.Y, 0.0001);
			Assert.AreEqual(0.0, ecliptic.Z, 0.0001);

			Coordinates equatorial = sun.EquatorialCoordinates;
			Assert.AreEqual(0.881048, equatorial.X, 0.0001);
			Assert.AreEqual(0.442312, equatorial.Y, 0.0001);
			Assert.AreEqual(0.191778, equatorial.Z, 0.0001);

			Assert.AreEqual(1.004323, equatorial.Distance, 0.0001, "Equatorial Distance");
			Assert.AreEqual(26.658000, equatorial.RightAscension, 0.0001);
			Assert.AreEqual(11.008400, equatorial.Declination, 0.0001);


			Assert.AreEqual(13.78925/*hours*/, sun.GMSTAt0Hrs, 0.0001);
			Assert.AreEqual(14.78925 /*hours: 14h 47m 21.3s*/, sun.SiderealTime, 0.0001);
			Assert.AreEqual(13.01205/*hours; */, sun.HourAngle, 0.0001);

			Assert.AreEqual(15.68/*Degrees*/, sun.SkyPosition.Azimuth, 0.01);
			Assert.AreEqual(-17.96/*degrees*/, sun.SkyPosition.Altitude, 0.01);
		}

		[TestMethod]
		[TestCategory("PlanetaryPositions")]
		public void MercuryPositionTest()
		{
			var testDate = new DayNumber(1990, 4, 19);
			var scandinaviaAtMidnightApril19th1990 = new EarthLocation(15.0, 60.0, testDate, 0.0);
			var mercury = new MercuryPosition();
			mercury.ObserverLocationOnEarth = scandinaviaAtMidnightApril19th1990;

			Assert.AreEqual(48.2163, mercury.AscendingNodeLongitude, 0.0001);
			Assert.AreEqual(7.0045, mercury.Inclination, 0.0001);
			Assert.AreEqual(29.0882, mercury.ArgumentOfPerihelion, 0.0001);

			var c = mercury.EclipticCoordinates;
			Assert.AreEqual(170.5709, c.Longitude, 0.0001);
			Assert.AreEqual(5.9255, c.Latitude, 0.0001);
			Assert.AreEqual(0.374862, c.Distance, 0.000001);
		}

		[TestMethod]
		[TestCategory("PlanetaryPositions")]
		public void MoonPositionTest()
		{
			var testDate = new DayNumber(1990, 4, 19);
			var scandinaviaAtMidnightApril19th1990 = new EarthLocation(15.0, 60.0, testDate, 0.0);

			var moon = new MoonPosition();
			moon.ObserverLocationOnEarth = scandinaviaAtMidnightApril19th1990;

			Assert.AreEqual(312.7381/*degrees*/, moon.AscendingNodeLongitude, 0.0001);
			Assert.AreEqual(5.1454/*degrees*/, moon.Inclination, 0.0001);
			Assert.AreEqual(95.7454/*deg*/, moon.ArgumentOfPerihelion, 0.0001);
			Assert.AreEqual(60.2666/*earth radii (equatorial)*/, moon.MeanDistance, 0.0001);
			Assert.AreEqual(0.054900, moon.Eccentricity, 0.000001);
			Assert.AreEqual(266.0954/*_deg*/, moon.MeanAnomaly, 0.0001);
			Assert.AreEqual(262.9735/*deg*/, moon.EccentricAnomaly, 0.0001);

			Assert.AreEqual(-10.68095, moon.PerihelionCoordinates.X, 0.0001);
			Assert.AreEqual(-59.72377, moon.PerihelionCoordinates.Y, 0.00001);

			Assert.AreEqual(60.67134/*earth radii*/, moon.Distance, 0.00001);
			Assert.AreEqual(259.8605/*degrees*/, moon.TrueAnomaly, 0.0001);

			var eclip = moon.BaseEclipticCoordinates;
			Assert.AreEqual(+37.65311, eclip.X, 0.00001);
			Assert.AreEqual(-47.57180, eclip.Y, 0.0001);
			Assert.AreEqual(-0.41687, eclip.Z, 0.0001);

			Assert.AreEqual(308.3616, eclip.Longitude, 0.00001);
			Assert.AreEqual(-0.3937, eclip.Latitude, 0.00001);
			Assert.AreEqual(60.6713, eclip.Distance, 0.0001);

			Assert.AreEqual(314.5789/*deg*/, moon.MeanLongitude, 0.0001);
			Assert.AreEqual(287.7401/*deg*/, moon.MeanElongation, 0.0001);
			Assert.AreEqual(1.8408/*deg*/, moon.ArgumentOfLatitude, 0.0001);

			Assert.AreEqual(-1.4132/*deg*/, moon.LongitudePerturbation, 0.0001);
			Assert.AreEqual(-0.1919/*deg*/, moon.LatitudePerturbation, 0.0001);
			Assert.AreEqual(0.0066/*earthradii*/, moon.DistancePerturbation, 0.0001);

			eclip = moon.EclipticCoordinates;
			Assert.AreEqual(306.9484, eclip.Longitude, 0.0001);
			Assert.AreEqual(-0.5856, eclip.Latitude, 0.0001);
			Assert.AreEqual(60.6779, eclip.Distance, 0.0001);

			Coordinates equat = moon.EquatorialCoordinates;
			Assert.AreEqual(309.5011/*deg*/, equat.RightAscension, 0.0001);
			Assert.AreEqual(-19.1032/*deg*/, equat.Declination, 0.0001);

			// Moon's topocentric position
			// needs a position on earth.
			// 15D E long  60 deg N lat
			Assert.AreEqual(272.3377/*deg*/, moon.HourAngle, 0.0001);
			Assert.AreEqual(59.83/*deg*/, moon.GeocentricLatitude, 0.01);
			Assert.AreEqual(0.9975, moon.DistanceFromEarthCenter/*rho*/, 0.0001);
			Assert.AreEqual(0.9443/*deg*/, moon.Parallax, 0.0001);
			Assert.AreEqual(88.642/*deg*/, moon.AuxiliaryAngle, 0.001);

			Coordinates t = moon.TopocentricCoordinates;
			Assert.AreEqual(310.0017/*deg*/, t.RightAscension, 0.0001);
			Assert.AreEqual(-19.8790/*deg*/, t.Declination, 0.0001);
		}
	}
}
