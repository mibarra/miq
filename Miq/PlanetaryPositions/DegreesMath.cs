using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.PlanetaryPositions
{
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

		public static double ArcSin(double angle)
		{
			return Math.Asin(angle) * DegreesPerRadian;
		}

		public static double Tan(double angle)
		{
			return Math.Tan(angle / DegreesPerRadian);
		}
	}
}
