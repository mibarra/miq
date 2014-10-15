using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miq.Tests.Nursery
{
	[TestClass]
	public class AntSimulatorTests
	{
		struct CellData
		{
			public float FoodScent;
			public float HomeScent;
			public byte Food;
			public bool IsNest;
		}

		struct Location
		{
			public Location(int x, int y)
			{
				X = x;
				Y = y;
			}

			public int X;
			public int Y;
		}

		class Terrain
		{
			public Terrain(float evaporationRate, int width, int height)
			{
				CellData = new CellData[width, height];
				EvaporationRate = evaporationRate;
				_center = new Location(CellData.GetLength(0) / 2, CellData.GetLength(1) / 2);
			}

			public void Evaporate()
			{
				for (int j = 0; j < CellData.GetLength(1); j++)
				{
					for (int i = 0; i < CellData.GetLength(0); i++)
					{
						CellData[i, j].HomeScent *= EvaporationRate;
						CellData[i, j].FoodScent *= EvaporationRate;
					}
				}
			}

			public Location Center { get { return _center; } }

			public bool IsNest(int x, int y)
			{
				return Math.Abs(x - _center.X) < 3 && Math.Abs(y - _center.Y) < 3;
			}

			public byte GetFood(int x, int y)
			{
				return CellData[x, y].Food;
			}

			public void ChangeFood(int x, int y, byte delta)
			{
				CellData[x, y].Food += delta;
			}

			public float GetFoodScent(int x, int y)
			{
				return CellData[x, y].FoodScent;
			}

			public void ChangeFoodScent(int x, int y, float delta)
			{
				CellData[x, y].FoodScent += delta;

			}

			public float GetHomeScent(int x, int y)
			{
				return CellData[x, y].HomeScent;
			}

			public void ChangeHomeScent(int x, int y, float delta)
			{
				CellData[x, y].HomeScent += delta;
			}

			CellData[,] CellData;
			Location _center;
			float EvaporationRate;
		}

		enum Direction
		{
			N, NE, E, SE, S, SW, W, NW
		}

		struct AntData
		{
			public bool HasFood;
			public Location Location;
		}

		class Colony
		{
			public Colony(int census, Terrain terrain)
			{
				AntData = new AntData[census];
				Terrain = terrain;
			}

			public void ActuateAnts()
			{
				// XXX
			}

			public void Move(int antIndex, Direction direction)
			{
				// XXX
			}

			public void PickFood(int antIndex)
			{
				// XXX
			}

			public void DropFood(int antIndex)
			{

			}

			Terrain Terrain;
			AntData[] AntData;
		}

		class Simulation
		{
			public void Step()
			{
				// Evaporate
				// Actuate Ants
			}
		}

		[TestMethod]
		public void CellData_Exists_HasExpectedFields_FieldsAreMutable()
		{
			CellData cell = new CellData();

			cell.FoodScent = 1.0F;
			cell.HomeScent = 0.5F;
			cell.Food = 255;
			cell.IsNest = true;

			Assert.AreEqual(1.0F, cell.FoodScent, float.Epsilon);
			Assert.AreEqual(0.5F, cell.HomeScent, float.Epsilon);
			Assert.AreEqual(255, cell.Food);
			Assert.IsTrue(cell.IsNest);
		}
	}
}
