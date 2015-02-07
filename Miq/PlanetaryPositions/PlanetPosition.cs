using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.PlanetaryPositions
{

	public abstract class PlanetPosition
	{
		// Main Orbital Parameters
		public abstract double AscendingNodeLongitude { get; }
		public abstract double Inclination { get; }
		public abstract double ArgumentOfPerihelion { get; }
		public abstract double MeanDistance { get; }
		public abstract double Eccentricity { get; }
		public abstract double MeanAnomaly { get; }

		public virtual double LongitudePerturbation { get { return 0; } }
		public virtual double LatitudePerturbation { get { return 0; } }
		public virtual double DistancePerturbation { get { return 0; } }

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

		public double TrueAnomaly
		{
			get
			{
				var perihelionCoords = PerihelionCoordinates;
				return DegreesMath.ClampAngle(DegreesMath.Atan2(perihelionCoords.Y, perihelionCoords.X));
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

		protected double BaseEccentricAnomaly()
		{
			return MeanAnomaly + DegreesMath.DegreesPerRadian * Eccentricity * DegreesMath.Sin(MeanAnomaly) * (1.0 + Eccentricity * DegreesMath.Cos(MeanAnomaly));
		}

		protected double ImproveEccentricAnomaly(double e)
		{
			return e - (e - DegreesMath.DegreesPerRadian * Eccentricity * DegreesMath.Sin(e) - MeanAnomaly) / (1 - Eccentricity * DegreesMath.Cos(e));
		}

		public double EccentricAnomaly
		{
			get
			{
				return SuccessiveAproximation(
					BaseEccentricAnomaly,
					ImproveEccentricAnomaly,
					(e1, e0) => Math.Abs(e1 - e0) < 0.0001);
			}
		}

		// ZZZ extract to a Math-type class...
		private double SuccessiveAproximation(Func<double> InitialGuess, Func<double, double> Improve, Func<double, double, bool> GoodEnough)
		{
			double prev;
			double guess = InitialGuess();
			do
			{
				prev = guess;
				guess = Improve(guess);
			} while (!GoodEnough(guess, prev));
			return guess;
		}

		public virtual double MeanLongitude
		{
			get
			{
				return DegreesMath.ClampAngle(AscendingNodeLongitude + ArgumentOfPerihelion + MeanAnomaly);
			}
		}


		public Coordinates BaseEclipticCoordinates
		{
			get
			{
				return new RectangularCoordinates(
				Distance * (DegreesMath.Cos(AscendingNodeLongitude) * DegreesMath.Cos(TrueAnomaly + ArgumentOfPerihelion) - DegreesMath.Sin(AscendingNodeLongitude) * DegreesMath.Sin(TrueAnomaly + ArgumentOfPerihelion) * DegreesMath.Cos(Inclination)),
	 Distance * (DegreesMath.Sin(AscendingNodeLongitude) * DegreesMath.Cos(TrueAnomaly + ArgumentOfPerihelion) + DegreesMath.Cos(AscendingNodeLongitude) * DegreesMath.Sin(TrueAnomaly + ArgumentOfPerihelion) * DegreesMath.Cos(Inclination)),
	Distance * DegreesMath.Sin(TrueAnomaly + ArgumentOfPerihelion) * DegreesMath.Sin(Inclination)
					);
			}
		}

		public double MeanElongation
		{
			get
			{
				return MeanLongitude - SunAtSameObserverLocation.MeanLongitude;
			}
		}

		// ZZZ delete DAte property, use this one isntead
		public EarthLocation ObserverLocationOnEarth { get; set; }

		public SunPosition SunAtSameObserverLocation
		{
			get
			{
				// ZZZ moon position class needs to know about sun position :(
				// should be on a different class that compuetes geocentric coordinates of a planet
				// from the planet position and earth(sunPosition).
				var sun = new SunPosition();
				sun.ObserverLocationOnEarth = ObserverLocationOnEarth;
				return sun;
			}
		}

		public double ArgumentOfLatitude
		{
			get
			{
				return MeanLongitude - AscendingNodeLongitude;
			}
		}

		public virtual Coordinates EclipticCoordinates
		{
			get
			{
				var baseCoords = BaseEclipticCoordinates;
				// ZZZ hard to get this right because of the longitude/right ascension latitude/declination misnaming.
				return new PolarCoordinates(
					baseCoords.Distance + DistancePerturbation,
					baseCoords.Longitude + LongitudePerturbation,
					baseCoords.Latitude + LatitudePerturbation);
			}
		}

		public virtual Coordinates EquatorialCoordinates
		{
			get
			{
				var sun = SunAtSameObserverLocation;
				return EclipticCoordinates.ToEquatorial(sun.ObliquityOfTheEcliptic);
			}
		}

		public virtual double HourAngle
		{
			get
			{
				SunPosition sun = SunAtSameObserverLocation;
				return DegreesMath.ClampAngle(sun.SiderealTime * 15/*conver tot degrees*/ - EquatorialCoordinates.RightAscension);
			}
		}

		public double GeocentricLatitude
		{
			get
			{
				return ObserverLocationOnEarth.Latitude - 0.1924/*degrees*/ * DegreesMath.Sin(2 * ObserverLocationOnEarth.Latitude);
			}
		}

		public double DistanceFromEarthCenter/*tho*/ {
			get
			{
				return 0.99833 + 0.00167 * DegreesMath.Cos(2 * ObserverLocationOnEarth.Latitude);
			}
		}

		public double Parallax
		{
			get
			{
				return DegreesMath.ArcSin(1 / EclipticCoordinates.Distance);
			}
		}

		public double AuxiliaryAngle
		{
			get
			{
				return DegreesMath.Atan2(DegreesMath.Tan(GeocentricLatitude), DegreesMath.Cos(HourAngle));
			}
		}

		public Coordinates TopocentricCoordinates
		{
			get
			{
				var equatorial = EquatorialCoordinates;
				double rightAscensionAdjust = Parallax * DistanceFromEarthCenter * DegreesMath.Cos(GeocentricLatitude) * DegreesMath.Sin(HourAngle) / DegreesMath.Cos(equatorial.Declination);
				double declinationAdjust = Parallax * DistanceFromEarthCenter * DegreesMath.Sin(GeocentricLatitude) * DegreesMath.Sin(AuxiliaryAngle - equatorial.Declination) / DegreesMath.Sin(AuxiliaryAngle);
				return new PolarCoordinates(
					equatorial.Distance,
					equatorial.RightAscension - rightAscensionAdjust,
					equatorial.Declination - declinationAdjust);
			}
		}
	}

	public class SunPosition : PlanetPosition
	{
		public override double AscendingNodeLongitude { get { throw new InvalidOperationException("Parameter not aplicable to SunPosition"); } }
		public override double Inclination { get { throw new InvalidOperationException("Parameter not aplicable to SunPosition"); } }
		public override double ArgumentOfPerihelion { get { throw new InvalidOperationException("Parameter not aplicable to SunPosition"); } }
		public override double MeanDistance
		{
			get
			{
				return 1.0;
			}
		}
		public override double Eccentricity
		{
			get
			{
				return 0.016709 - 1.151e-9 * ObserverLocationOnEarth.Date.Day;
			}
		}
		public override double MeanAnomaly
		{
			get
			{
				return DegreesMath.ClampAngle(356.0470 + 0.9856002585 * ObserverLocationOnEarth.Date.Day);
			}
		}

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

		public double AscendingPerihelionAngle
		{
			get
			{
				// ZZZ create "day ajusted parameter" for this kind of values?
				return 282.9404 + 4.70935e-5 * ObserverLocationOnEarth.Date.Day;
			}
		}

		public double Longitude
		{
			get
			{
				return DegreesMath.ClampAngle(TrueAnomaly + AscendingPerihelionAngle);
			}
		}

		public override double MeanLongitude
		{
			get
			{
				return DegreesMath.ClampAngle(AscendingPerihelionAngle + MeanAnomaly);
			}
		}

		public double ObliquityOfTheEcliptic
		{
			get
			{
				// 23° 26' 21.4794"
				return 23.4393 - 3.563e-7 * ObserverLocationOnEarth.Date.Day;
			}
		}

		public override Coordinates EquatorialCoordinates
		{
			get
			{
				return EclipticCoordinates.ToEquatorial(ObliquityOfTheEcliptic);
			}
		}

		public override Coordinates EclipticCoordinates
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

		public override double HourAngle
		{
			get
			{
				return SiderealTime - EquatorialCoordinates.RightAscension / 15.0 /* ZZZ again, converting degrees to horus */;
				// ZZZ fixme, should we clamp the HourAngle to something?
			}
		}
	}

	public class MercuryPosition : PlanetPosition
	{
		public override double AscendingNodeLongitude
		{
			get
			{
				return DegreesMath.ClampAngle(48.3313/* degrees*/ + 3.24587e-5/* degrees per day */ * ObserverLocationOnEarth.Date.Day);
			}
		}
		public override double Inclination { get { return 7.0047 + 5.00E-8 * ObserverLocationOnEarth.Date.Day; } }
		public override double ArgumentOfPerihelion
		{
			get
			{
				return DegreesMath.ClampAngle(29.1241/*deg*/ + 1.01444E-5/*deg*/ * ObserverLocationOnEarth.Date.Day);
			}
		}
		public override double MeanDistance { get { return 0.387098; } }
		public override double Eccentricity { get { return 0.205635 + 5.59E-10 * ObserverLocationOnEarth.Date.Day; } }
		public override double MeanAnomaly
		{
			get
			{
				return DegreesMath.ClampAngle(168.6562/*deg*/ + 4.0923344368/*deg per day*/ * ObserverLocationOnEarth.Date.Day);
			}
		}
	}

	public class MoonPosition : PlanetPosition
	{
		public override double AscendingNodeLongitude
		{
			get
			{
				return DegreesMath.ClampAngle(125.1228 /* degrees*/ - 0.0529538083 /* degrees per day */ * ObserverLocationOnEarth.Date.Day);
			}
		}
		public override double Inclination { get { return 5.1454/*degrees*/; } }
		public override double ArgumentOfPerihelion
		{
			get
			{
				return DegreesMath.ClampAngle(318.0634/*deg*/ + 0.1643573223/*deg*/ * ObserverLocationOnEarth.Date.Day);
			}
		}
		public override double MeanDistance { get { return 60.2666/*earth radii*/; } }
		public override double Eccentricity { get { return 0.054900; } }
		public override double MeanAnomaly
		{
			get
			{
				return DegreesMath.ClampAngle(115.3654/*deg*/ + 13.0649929509/*deg per day*/ * ObserverLocationOnEarth.Date.Day);
			}
		}

		public override double LongitudePerturbation
		{
			get
			{
				// ZZZ check performance, we're recomputing TONS of values
				var sun = SunAtSameObserverLocation;
				return
					-1.274/*deg*/ * DegreesMath.Sin(MeanAnomaly - 2 * MeanElongation) + // Evection
					+0.658/*deg*/ * DegreesMath.Sin(2 * MeanElongation) + //         (Variation)
					-0.186/*deg*/ * DegreesMath.Sin(sun.MeanAnomaly) + //          (Yearly equation)
					-0.059/*deg*/ * DegreesMath.Sin(2 * MeanAnomaly - 2 * MeanElongation) +
					-0.057/*deg*/ * DegreesMath.Sin(MeanAnomaly - 2 * MeanElongation + sun.MeanAnomaly) +
					+0.053/*deg*/ * DegreesMath.Sin(MeanAnomaly + 2 * MeanElongation) +
					+0.046/*deg*/ * DegreesMath.Sin(2 * MeanElongation - sun.MeanAnomaly) +
					+0.041/*deg*/ * DegreesMath.Sin(MeanAnomaly - sun.MeanAnomaly) +
					-0.035/*deg*/ * DegreesMath.Sin(MeanElongation) + //            (Parallactic equation)
					-0.031/*deg*/ * DegreesMath.Sin(MeanAnomaly + sun.MeanAnomaly) +
					-0.015/*deg*/ * DegreesMath.Sin(2 * ArgumentOfLatitude - 2 * MeanElongation) +
					+0.011/*deg*/ * DegreesMath.Sin(MeanAnomaly - 4 * MeanElongation);
			}
		}

		public override double LatitudePerturbation
		{
			get
			{
				return
					-0.173/*deg*/ * DegreesMath.Sin(ArgumentOfLatitude - 2 * MeanElongation) +
					-0.055/*deg*/ * DegreesMath.Sin(MeanAnomaly - ArgumentOfLatitude - 2 * MeanElongation) +
					-0.046/*deg*/ * DegreesMath.Sin(MeanAnomaly + ArgumentOfLatitude - 2 * MeanElongation) +
					+0.033/*deg*/ * DegreesMath.Sin(ArgumentOfLatitude + 2 * MeanElongation) +
					+0.017/*deg*/ * DegreesMath.Sin(2 * MeanAnomaly + ArgumentOfLatitude);

			}
		}

		public override double DistancePerturbation
		{
			get
			{
				return
					-0.58 * DegreesMath.Cos(MeanAnomaly - 2 * MeanElongation) +
					-0.46 * DegreesMath.Cos(2 * MeanElongation);
			}
		}
	}
}
