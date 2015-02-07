using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.PlanetaryPositions
{
	public struct DayNumber
	{
		private const double HoursPerDay = 24.0;
		private const double MinutesPerDay = 1440.0;
		private const double SecondsPerDay = 86400;

		private static readonly int[] DaysPerMonthLeapYear = {
			0, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
		private static readonly int[] DaysPerMonthNormalYear = {
			0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};

		public readonly double Day;

		public DayNumber(int year, int month, int day)
		{
			Day = DateToDaysNumber(year, month, day);
		}

		public DayNumber(int year, int month, int day, int hours, int minutes, double seconds)
		{
			Day = DateToDaysNumber(year, month, day) + TimeToDaysNumber(hours, minutes, seconds);
		}

		private static int DateToDaysNumber(int year, int month, int day)
		{
			if (year >= 1 && year <= 9999 && month >= 1 && month <= 12)
			{
				int[] days = IsLeapYear(year) ? DaysPerMonthLeapYear : DaysPerMonthNormalYear;
				if (day >= 1 && day <= days[month])
				{
					return 367 * year - 7 * (year + (month + 9) / 12) / 4 + 275 * month / 9 + day - 730530;
				}
			}

			throw new ArgumentOutOfRangeException(null, "The given year, month, day values cannot be converted into a daynumber.");
		}

		private static double TimeToDaysNumber(int hours, int minutes, double seconds)
		{
			return hours / HoursPerDay + minutes / MinutesPerDay + seconds / SecondsPerDay;
		}

		public static bool IsLeapYear(int year)
		{
			if (year < 1 || year > 9999)
			{
				throw new ArgumentOutOfRangeException("year", "Only years 1 to 9999 are currently supported.");
			}

			return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
		}
	}
}
