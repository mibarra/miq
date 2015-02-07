using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miq.PlanetaryPositions;

namespace Miq.Tests.PlanetaryPositions
{
	[TestClass]
	public class DayNumberTests
	{
		[TestMethod]
		[TestCategory("PlanetaryPositions")]
		public void DayNumber_BasicTests()
		{
			var x = new DayNumber(1990, 4, 19);
			Assert.AreEqual(-3543.0, x.Day, 0.000001);

			var y = new DayNumber(1990, 4, 19, 23, 59, 59.99);
			Assert.AreEqual(-3542, y.Day, 0.000001);
		}

		[TestMethod]
		[TestCategory("PlanetaryPositions")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DayNumber_Throws_WhenYearIsTooLow()
		{
			new DayNumber(-1, 4, 19);
		}

		[TestMethod]
		[TestCategory("PlanetaryPositions")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DayNumber_Throws_WhenYearIsTooHigh()
		{
			new DayNumber(10000, 4, 19);
		}

		[TestMethod]
		[TestCategory("PlanetaryPositions")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DayNumber_Throws_WhenMonthIsTooLow()
		{
			new DayNumber(1990, 0, 19);
		}

		[TestMethod]
		[TestCategory("PlanetaryPositions")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DayNumber_Throws_WhenMonthIsTooHigh()
		{
			new DayNumber(1990, 13, 19);
		}

		[TestMethod]
		[TestCategory("PlanetaryPositions")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DayNumber_Throws_WhenDayIsTooLow()
		{
			new DayNumber(1990, 4, 0);
		}

		[TestMethod]
		[TestCategory("PlanetaryPositions")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DayNumber_Throws_WhenDayIsTooHigh()
		{
			new DayNumber(1990, 4, 31);
		}

		[TestMethod]
		[TestCategory("PlanetaryPositions")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DayNumber_Throws_WhenDayIsTooHighNonLeapYear()
		{
			new DayNumber(1990, 2, 29);
		}
	}
}
