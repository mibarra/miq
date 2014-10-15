using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Miq.Tests.Nursery
{
	public class IntSpan
	{
		private Dictionary<int, int> Self;

		public IntSpan()
		{
			Self = new Dictionary<int, int>();
		}

		public void Add(params int[] args)
		{
			foreach (var i in args)
			{
				Self.Add(i, 1);
			}
		}

		public void AddRange(params int[] args)
		{
			if ((args.Length % 2) != 0)
			{
				throw new ArgumentException("args must come in pairs.");
			}

			// XXX can we do this using Linq?
			for (int i = 0; i < args.Length; i += 2)
			{
				for (int j = args[i]; j <= args[i + 1]; j++)
				{
					Add(j);
				}
			}
		}

		public void Remove(params int[] args)
		{
			foreach (var i in args)
			{
				Self.Remove(i);
			}
		}

		public void RemoveRange(params int[] args)
		{
			if ((args.Length % 2) != 0)
			{
				throw new ArgumentException("args must come in pairs.");
			}

			// XXX can we do this using Linq?
			for (int i = 0; i < args.Length; i += 2)
			{
				for (int j = args[i]; j <= args[i + 1]; j++)
				{
					Remove(j);
				}
			}
		}

		public int[] ToArray()
		{
			return Self.Keys.OrderBy(i => i).ToArray();
		}
	}

	[TestClass]
	public class IntSpanTests
	{
		[TestMethod]
		public void CanBeInstanciated()
		{
			var set = new IntSpan();
			Assert.IsInstanceOfType(set, typeof(IntSpan));
		}

		[TestMethod]
		public void SimpleCases_TwoRangesOverlappingInVariousWays()
		{
			for (int i = -5; i <= 5; i++)
			{
				for (int j = -5; j <= 5; j++)
				{
					var intSpan = new IntSpan();
					var testSet = new TestSet();

					intSpan.AddRange(-2, 2);
					testSet.AddRange(-2, 2);

					CollectionAssert.AreEqual(testSet.ToArray(), intSpan.ToArray(), "add init range");
				}
			}
		}

		/// <summary>
		/// Basic implementation of spans based on a dictionary to use on tests
		/// </summary>
		class TestSet
		{
			private Dictionary<int, int> Self;

			public TestSet()
			{
				Self = new Dictionary<int, int>();
			}

			public void Add(params int[] args)
			{
				foreach (var i in args)
				{
					Self.Add(i, 1);
				}
			}

			public void AddRange(params int[] args)
			{
				if ((args.Length % 2) != 0)
				{
					throw new ArgumentException("args must come in pairs.");
				}

				// XXX can we do this using Linq?
				for (int i = 0; i < args.Length; i += 2)
				{
					for (int j = args[i]; j <= args[i + 1]; j++)
					{
						Add(j);
					}
				}
			}

			public void Remove(params int[] args)
			{
				foreach (var i in args)
				{
					Self.Remove(i);
				}
			}

			public void RemoveRange(params int[] args)
			{
				if ((args.Length % 2) != 0)
				{
					throw new ArgumentException("args must come in pairs.");
				}

				// XXX can we do this using Linq?
				for (int i = 0; i < args.Length; i += 2)
				{
					for (int j = args[i]; j <= args[i + 1]; j++)
					{
						Remove(j);
					}
				}
			}

			public int[] ToArray()
			{
				return Self.Keys.OrderBy(i => i).ToArray();
			}
		}
	}
}
