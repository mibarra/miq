using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.PlanetaryPositions
{
	public class EarthLocation
	{
		public EarthLocation(double longitude, double latitude, DayNumber date, double time)
		{
			Longitude = longitude;
			Latitude = latitude;
			Date = date;
			Time = time;
		}

		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public DayNumber Date { get; set; }
		public double Time { get; set; } // ZZZ Time is the fractional part of Date
	}
}
