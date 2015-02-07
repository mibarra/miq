using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.PlanetaryPositions
{

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

		public double Longitude { get { return RightAscension; } }

		public double Latitude { get { return Declination; } }
	}

	public class SkyCoordinates : RectangularCoordinates
	{
		public SkyCoordinates(double x, double y, double z)
			: base(x, y, z)
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
		public override double RightAscension { get { return DegreesMath.ClampAngle(DegreesMath.Atan2(Y, X)); } }
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
}
