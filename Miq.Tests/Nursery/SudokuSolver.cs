using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Diagnostics;

namespace Miq.Tests.Nursery
{
	/// <summary>
	/// Solving Every Sudoku Puzzle
	/// by Peter Norvig
	/// 
	/// http://norvig.com/sudoku.html
	/// 
	/// </summary>
	public class Sudoku
	{
		static IEnumerable<string> Cross(string A, string B)
		{
			foreach (var a in A)
			{
				foreach (var b in B)
				{
					yield return "" + a + b;
				}
			}
		}

		private static IEnumerable<string> Cross(string A, char b)
		{
			foreach (var a in A)
			{
				yield return "" + a + b;
			}
		}

		static IEnumerable<string> Cross(char a, string B)
		{
			foreach (var b in B)
			{
				yield return "" + a + b;
			}
		}

		static string digits = "123456789";
		static string rows = "ABCDEFGHI";
		static string cols = digits;
		static string[] squares = Cross(rows, cols).ToArray();
		static string[][] unitlist;
		static Dictionary<string, string[][]> units;
		static Dictionary<string, string[]> peers;

		static Sudoku()
		{
			var unitlistcols = cols.Select(c => Cross(rows, c).ToArray()).ToArray();
			var unitlistrows = rows.Select(r => Cross(r, cols).ToArray()).ToArray();
			var rowBoxes = new string[] { "ABC", "DEF", "GHI" };
			var colBoxes = new string[] { "123", "456", "789" };
			var unitlistboxes = Cross(rowBoxes, colBoxes).ToArray();
			unitlist = unitlistcols.Concat(unitlistrows.Concat(unitlistboxes)).ToArray();

			units = squares.ToDictionary(
							s => s,
							s => unitlist.Where(u => u.Contains(s)).ToArray());

			peers = squares.ToDictionary(
							s => s,
							s => units[s].SelectMany(a => a)
										 .Distinct()
										 .Where(sq => s != sq).ToArray());

		}

		private static IEnumerable<string[]> Cross(string[] A, string[] B)
		{
			foreach (var a in A)
			{
				foreach (var b in B)
				{
					yield return Cross(a, b).ToArray();
				}
			}
		}

		/// <summary>
		/// Convert grid to a dict of possible values, {square: digits}, or
		/// return False if a contradiction is detected.
		/// </summary>
		public Dictionary<string, string> parse_grid(string grid)
		{
			// To start, every square can be any digit; then assign values from the grid.
			var values = squares.ToDictionary(s => s, s => digits);
			foreach (var kvp in grid_values(grid).Where(i => digits.Contains(i.Value)))
			{
				var s = kvp.Key;
				var d = "" + kvp.Value;
				if (assign(values, s, d) == null)
				{
					return null;
				}
			}

			return values;
		}

		/// <summary>
		/// Convert grid into a dict of {square: char} with '0' or '.' for empties.
		/// </summary>
		public Dictionary<string, char> grid_values(string grid)
		{
			var chars = grid.Where(c => digits.Contains(c) || c == '0' || c == '.').ToArray();
			if (chars.Length != 81)
			{
				throw new ArgumentException("invalid grid");
			}

			var values = new Dictionary<string, char>();
			for (int index = 0; index < squares.Length; index++)
			{
				values.Add(squares[index], chars[index]);
			}
			return values;
		}


		/// <summary>
		/// Eliminate all the other values (except d) from values[s] and propagate.
		/// Return values, except return null if a contradiction is detected.
		/// </summary>
		private Dictionary<string, string> assign(Dictionary<string, string> values, string s, string d)
		{
			var other_values = values[s].Replace(d, "");
			if (other_values.Select(d2 => eliminate(values, s, "" + d2)).All(r => r != null))
			{
				return values;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Eliminate d from values[s]; propagate when values or places <= 2.
		/// Return values, except return null if a contradiction is detected.
		/// </summary>
		Dictionary<string, string> eliminate(Dictionary<string, string> values, string s, string d)
		{
			if (!values[s].Contains(d))
			{
				return values; // Already eliminated
			}

			values[s] = values[s].Replace(d, "");

			// (1) If a square s is reduced to one value d2, then eliminate d2 from the peers.
			if (values[s].Length == 0)
			{
				return null; // Contradiction: removed last value
			}
			else if (values[s].Length == 1)
			{
				var d2 = values[s];
				if (!peers[s].Select(s2 => eliminate(values, s2, d2)).All(r => r != null))
				{
					return null;
				}
			}

			// (2) If a unit u is reduced to only one place for a value d, then put it there.
			foreach (var u in units[s])
			{
				var dplaces = u.Where(s2 => values[s2].Contains(d));
				if (dplaces.Count() == 0)
				{
					return null; // Contradiction: no place for this value
				}
				else if (dplaces.Count() == 1)
				{
					// d can only be in one place in unit; assign it there
					if (assign(values, dplaces.First(), d) == null)
					{
						return null;
					}
				}
			}
			return values;
		}

		/// <summary>
		/// Display these values as a 2-D grid.
		/// </summary>
		public void display(Dictionary<string, string> values)
		{
			if (values == null)
			{
				Debug.WriteLine("Bad values");
				return;
			}

			var width = 1 + squares.Max(s => values[s].Length);
			var line = string.Join("+", Repeat(3, Repeat(3 * width, '-')));
			foreach (var r in rows)
			{
				foreach (var c in cols)
				{
					var source = values["" + r + c];
					int spaces = width - source.Length;
					int padLeft = spaces / 2 + source.Length;
					var s = source.PadLeft(padLeft).PadRight(width);
					Debug.Write(s);
					if (c == '3' || c == '6')
					{
						Debug.Write("|");
					}
				}

				Debug.WriteLine(" ");
				if (r == 'C' || r == 'F')
				{
					Debug.WriteLine(line);
				}
			}
			Debug.WriteLine("  ");
		}

		string Repeat(int count, char c)
		{
			return new string(c, count);
		}

		string[] Repeat(int count, string s)
		{
			return ArrayList.Repeat(s, count).Cast<string>().ToArray();
		}

		public Dictionary<string, string> solve(string grid)
		{
			var values = parse_grid(grid);
			return search(values);
		}


		/// <summary>
		/// Using depth-first search and propagation, try all possible values.
		/// </summary>
		public Dictionary<string, string> search(Dictionary<string, string> values)
		{
			if (values == null)
			{
				return null; // Failed earlier
			}

			if (values.Values.All(v => v.Length == 1))
			{
				return values; // Solved!
			}

			// Chose the unfilled square s with the fewest possibilities
			var sortedSquares = squares.Where(sq => values[sq].Length > 1)
									   .OrderBy(sq => values[sq].Length);
			var s = sortedSquares.First();
			return values[s].Select(d =>
			{
				var vals = assign(new Dictionary<string, string>(values), s, "" + d); 
				return search(vals);
			}).FirstOrDefault(r => r != null);
		}
	}

	[TestClass]
	public class SudokuSolverTests
	{
		[TestMethod]
		public void TestMethod1()
		{
			var r1 = "4.....8.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......";

			var r2 = @"
400000805
030000000
000700000
020000060
000080400
000010000
000603070
500200000
104000000";

			var r3 = @"
4 . . |. . . |8 . 5 
. 3 . |. . . |. . . 
. . . |7 . . |. . . 
------+------+------
. 2 . |. . . |. 6 . 
. . . |. 8 . |4 . . 
. . . |. 1 . |. . . 
------+------+------
. . . |6 . 3 |. 7 . 
5 . . |2 . . |. . . 
1 . 4 |. . . |. . . 
";

			var x = new Sudoku();
			x.display(x.solve(r1));
			x.display(x.solve(r2));
			x.display(x.solve(r3));


			var grid1 = "003020600900305001001806400008102900700000008006708200002609500800203009005010300";
			x.display(x.solve(grid1));
		}
	}
}
