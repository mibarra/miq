using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miq.Tests.Nursery
{
	[TestClass]
	public class PlanetaryPositionsTests
	{
		// See also, OrbitalParameters_Tests.cs

		[TestMethod]
		public void SunPositionTest()
		{
			// ZZZ abstract out the fact that an angle can be expressed in degrees or hours

			var sun = new SunPosition();
			// ZZZ mix date above in EarthLocation
			var scandinaviaAtMidnightApril19th1990 = new EarthLocation(15.0, 60.0, -3543, 0.0);
			sun.ObserverLocationOnEarth = scandinaviaAtMidnightApril19th1990;

			// ZZZ getting tired of typing 0.0001, abstract out
			Assert.AreEqual(1.0, sun.MeanDistance, 0.0001);
			Assert.AreEqual(0.016713, sun.Eccentricity, 0.0001);
			Assert.AreEqual(282.7735, sun.AscendingPerihelionAngle, 0.0001);
			Assert.AreEqual(104.0653, sun.MeanAnomaly, 0.0001);
			Assert.AreEqual(26.8388, sun.MeanLongitude, 0.0001);
			Assert.AreEqual(23.4406, sun.EclipticObliquity, 0.0001);
			Assert.AreEqual(104.9904, sun.EccentricAnomaly, 0.0001);
			Assert.AreEqual(1.004323, sun.Distance, 0.0001);
			Assert.AreEqual(105.9134, sun.TrueAnomaly, 0.0001);
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

			// dawn, sunrise, sunset, dusk, night
			// noche, alba, dia, ocaso
			// 18 degrees below horizon: night
			// 90.83 from zenith
			// We could calculate dawn/sunrise/sunset/dusk time tables! :)
		}
	}

	public class EarthLocation
	{
		public EarthLocation(double longitude, double latitude, double date, double time)
		{
			Longitude = longitude;
			Latitude = latitude;
			Date = date;
			Time = time;
		}

		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public double Date { get; set; } // ZZZ in jdateblah.
		public double Time { get; set; } // ZZZ in UT
	}

	public class SunPosition
	{
		public EarthLocation ObserverLocationOnEarth { get; set; }

		public SkyCoordinates SkyPosition
		{
			get
			{
				var c = RectangularCoordinates.FromHourAngleAndDeclination(
					HourAngle * 15.0 /* degrees conversion, again*/,
					EquatorialCoordinates.Declination);
				return c.RotateAraoundYForLatitude(ObserverLocationOnEarth.Latitude);
			}
		}

		public double MeanDistance
		{
			get
			{
				return 1.0;
			}
		}

		public double AscendingPerihelionAngle
		{
			get
			{
				return 282.9404 + 4.70935e-5 * ObserverLocationOnEarth.Date;
			}
		}

		public double Eccentricity
		{
			get
			{
				return 0.016709 - 1.151e-9 * ObserverLocationOnEarth.Date;
			}
		}

		public double MeanAnomaly
		{
			get
			{
				return DegreesMath.ClampAngle(356.0470 + 0.9856002585 * ObserverLocationOnEarth.Date);
			}
		}

		public double EccentricAnomaly
		{
			get
			{
				return MeanAnomaly + (180 / Math.PI) * Eccentricity * DegreesMath.Sin(MeanAnomaly) * (1 + Eccentricity * DegreesMath.Cos(MeanAnomaly));
			}
		}

		// ZZZ Distance&TrueAnomaly are a set of PolarCoordinates
		public double TrueAnomaly
		{
			get
			{
				var perihelionCoords = PerihelionCoordinates;
				return DegreesMath.Atan2(perihelionCoords.Y, perihelionCoords.X);
			}
		}

		public double Distance
		{
			get
			{
				var perihelionCoords = PerihelionCoordinates;
				return DegreesMath.Magnitude(perihelionCoords.X, perihelionCoords.Y);
			}
		}

		public double Longitude
		{
			get
			{
				return DegreesMath.ClampAngle(TrueAnomaly + AscendingPerihelionAngle);
			}
		}

		public double MeanLongitude
		{
			get
			{
				return DegreesMath.ClampAngle(AscendingPerihelionAngle + MeanAnomaly);
			}
		}

		public double EclipticObliquity
		{
			get
			{
				return 23.4393 - 3.563e-7 * ObserverLocationOnEarth.Date;
			}
		}

		public Coordinates EquatorialCoordinates
		{
			get
			{
				return EclipticCoordinates.ToEquatorial(EclipticObliquity);
			}
		}

		public RectangularCoordinates PerihelionCoordinates
		{
			get
			{
				return new RectangularCoordinates(
					MeanDistance * (DegreesMath.Cos(EccentricAnomaly) - Eccentricity),
					MeanDistance * DegreesMath.Sin(EccentricAnomaly) * Math.Sqrt(1 - Eccentricity * Eccentricity),
					0
				);
			}
		}

		public Coordinates EclipticCoordinates
		{
			get
			{
				return RectangularCoordinates.FromDistanceAndLongitude(Distance, Longitude);
			}
		}

		public double GMSTAt0Hrs
		{
			get
			{
				// divided by 15 to convert degrees to hours
				return MeanLongitude / 15.0 + 12.0/*hours*/;
				// ZZZ we're returning hours, should clamp betwee 0 and 24.
			}
		}


		public double SiderealTime
		{
			get
			{
				return GMSTAt0Hrs +
					ObserverLocationOnEarth.Time +
					ObserverLocationOnEarth.Longitude/*degrees*/ / 15.0 /*divided to convert degrees to hours*/;
			}
		}

		public double HourAngle
		{
			get
			{
				return SiderealTime - EquatorialCoordinates.RightAscension / 15.0 /* ZZZ again, converting degrees to horus */;
				// ZZZ fixme, should we clamp the HourAngle to something?
			}
		}
	}

	public abstract class Coordinates
	{
		public abstract double X { get; }
		public abstract double Y { get; }
		public abstract double Z { get; }

		public abstract double Distance { get; }
		public abstract double RightAscension { get; }
		public abstract double Declination { get; }

		public Coordinates ToEquatorial(double angle)
		{
			// ZZZ don't like the fact that this class needs to know about its inherited class
			return new RectangularCoordinates(
				X,
				Y * DegreesMath.Cos(angle) - Z * DegreesMath.Sin(angle),
				Y * DegreesMath.Sin(angle) + Z * DegreesMath.Cos(angle));
		}

		public SkyCoordinates RotateAraoundYForLatitude(double latitude)
		{
			return new SkyCoordinates(
				X * DegreesMath.Sin(latitude) - Z * DegreesMath.Cos(latitude),
				Y,
				X * DegreesMath.Cos(latitude) + Z * DegreesMath.Sin(latitude));
		}
	}

	public class SkyCoordinates : RectangularCoordinates
	{
		public SkyCoordinates(double x, double y, double z)
			:base(x,y,z)
		{ }

		public double Azimuth
		{
			get
			{
				return DegreesMath.ClampAngle(DegreesMath.Atan2(Y, X) + 180.0/*degrees*/);
			}
		}

		public double Altitude
		{
			get
			{
				return DegreesMath.Atan2(Z, DegreesMath.Magnitude(X, Y));
			}
		}
	}

	public class RectangularCoordinates : Coordinates
	{
		public RectangularCoordinates(double x, double y, double z)
		{
			_X = x;
			_Y = y;
			_Z = z;
		}

		public static Coordinates FromDistanceAndLongitude(double distance, double longitude)
		{
			return new RectangularCoordinates(
				distance * DegreesMath.Cos(longitude),
				distance * DegreesMath.Sin(longitude),
				0.0);
		}

		// ZZZ make the name better, more general?
		public static Coordinates FromHourAngleAndDeclination(double hourAngle, double declination)
		{
			// Assumes distance = 1
			return new RectangularCoordinates(
				DegreesMath.Cos(hourAngle) * DegreesMath.Cos(declination),
				DegreesMath.Sin(hourAngle) * DegreesMath.Cos(declination),
				DegreesMath.Sin(declination)
				);
		}

		public override double X { get { return _X; } }
		public override double Y { get { return _Y; } }
		public override double Z { get { return _Z; } }
		public override double Distance { get { return DegreesMath.Magnitude(X, Y, Z); } }
		// XXX beware if x and y are close to Z (i.e. coordinate is close to a celestial pole)
		public override double RightAscension { get { return DegreesMath.Atan2(Y, X); } }
		public override double Declination { get { return DegreesMath.Atan2(Z, DegreesMath.Magnitude(X, Y)); } }

		private double _X;
		private double _Y;
		private double _Z;
	}

	public class PolarCoordinates : Coordinates
	{
		public PolarCoordinates(double distance, double rightAscension, double declination)
		{
			_Distance = distance;
			_RightAscension = rightAscension;
			_Declination = declination;
		}

		public override double X { get { return Distance * DegreesMath.Cos(RightAscension) * DegreesMath.Cos(Declination); } }
		public override double Y { get { return Distance * DegreesMath.Sin(RightAscension) * DegreesMath.Cos(Declination); } }
		public override double Z { get { return Distance * DegreesMath.Sin(Declination); } }
		public override double Distance { get { return _Distance; } }
		public override double RightAscension { get { return _RightAscension; } }
		public override double Declination { get { return _Declination; } }

		private double _Distance;
		private double _RightAscension;
		private double _Declination;
	}

	public static class DegreesMath
	{
		public const double DegreesPerRadian = 180.0 / Math.PI;

		public static double Magnitude(double x, double y, double z)
		{
			return Math.Sqrt(x * x + y * y + z * z);
		}

		public static double Magnitude(double x, double y)
		{
			return Math.Sqrt(x * x + y * y);
		}

		public static double Atan2(double y, double x)
		{
			return Math.Atan2(y, x) * DegreesPerRadian;
		}

		public static double Sin(double angle)
		{
			return Math.Sin(angle / DegreesPerRadian);
		}

		public static double Cos(double angle)
		{
			return Math.Cos(angle / DegreesPerRadian);
		}

		public static double ClampAngle(double angle)
		{
			double rev = angle - (int)(angle / 360) * 360;
			if (rev < 0.0)
			{
				rev += 360.0;
			}
			return rev;
		}
	}
}
