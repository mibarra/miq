using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Miq.Tests.Nursery
{
	public class Square : IEquatable<Square>, IComparable<Square>
	{
		public readonly int Rank;
		public readonly ChessFile File;

		public Square(int rank, ChessFile file)
		{
			if (rank == 0 && file == ChessFile.None)
			{
				return;
			}

			if (rank < 1 || rank > 8)
			{
				throw new ArgumentException("rank should be between 1 and 8");
			}
			if (file != ChessFile.A && file != ChessFile.B && file != ChessFile.C && file != ChessFile.D &&
				file != ChessFile.E && file != ChessFile.F && file != ChessFile.G && file != ChessFile.H)
			{
				throw new ArgumentException("file shound be File.A thru File.H");
			}

			Rank = rank;
			File = file;
		}

		public Square FirstSquareOnNextRank
		{
			get
			{
				return new Square(Rank - 1, ChessFile.A);
			}
		}

		public int BitIndex
		{
			get
			{
				return (Rank - 1) * 8 + (int)File - 1;
			}
		}

		internal static Square For(int i)
		{
			return new Square(1 + i / 8, (ChessFile)(1 + i % 8));
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return Rank != 0 ? string.Format("{0}{1}", File, Rank) : "None";
		}

		#region All Squares
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square None = new Square(0, ChessFile.None);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square A8 = new Square(8, ChessFile.A);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square B8 = new Square(8, ChessFile.B);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square C8 = new Square(8, ChessFile.C);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square D8 = new Square(8, ChessFile.D);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square E8 = new Square(8, ChessFile.E);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square F8 = new Square(8, ChessFile.F);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square G8 = new Square(8, ChessFile.G);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square H8 = new Square(8, ChessFile.H);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square A7 = new Square(7, ChessFile.A);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square B7 = new Square(7, ChessFile.B);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square C7 = new Square(7, ChessFile.C);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square D7 = new Square(7, ChessFile.D);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square E7 = new Square(7, ChessFile.E);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square F7 = new Square(7, ChessFile.F);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square G7 = new Square(7, ChessFile.G);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square H7 = new Square(7, ChessFile.H);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square A6 = new Square(6, ChessFile.A);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square B6 = new Square(6, ChessFile.B);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square C6 = new Square(6, ChessFile.C);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square D6 = new Square(6, ChessFile.D);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square E6 = new Square(6, ChessFile.E);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square F6 = new Square(6, ChessFile.F);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square G6 = new Square(6, ChessFile.G);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square H6 = new Square(6, ChessFile.H);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square A5 = new Square(5, ChessFile.A);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square B5 = new Square(5, ChessFile.B);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square C5 = new Square(5, ChessFile.C);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square D5 = new Square(5, ChessFile.D);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square E5 = new Square(5, ChessFile.E);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square F5 = new Square(5, ChessFile.F);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square G5 = new Square(5, ChessFile.G);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square H5 = new Square(5, ChessFile.H);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square A4 = new Square(4, ChessFile.A);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square B4 = new Square(4, ChessFile.B);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square C4 = new Square(4, ChessFile.C);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square D4 = new Square(4, ChessFile.D);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square E4 = new Square(4, ChessFile.E);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square F4 = new Square(4, ChessFile.F);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square G4 = new Square(4, ChessFile.G);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square H4 = new Square(4, ChessFile.H);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square A3 = new Square(3, ChessFile.A);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square B3 = new Square(3, ChessFile.B);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square C3 = new Square(3, ChessFile.C);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square D3 = new Square(3, ChessFile.D);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square E3 = new Square(3, ChessFile.E);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square F3 = new Square(3, ChessFile.F);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square G3 = new Square(3, ChessFile.G);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square H3 = new Square(3, ChessFile.H);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square A2 = new Square(2, ChessFile.A);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square B2 = new Square(2, ChessFile.B);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square C2 = new Square(2, ChessFile.C);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square D2 = new Square(2, ChessFile.D);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square E2 = new Square(2, ChessFile.E);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square F2 = new Square(2, ChessFile.F);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square G2 = new Square(2, ChessFile.G);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square H2 = new Square(2, ChessFile.H);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square A1 = new Square(1, ChessFile.A);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square B1 = new Square(1, ChessFile.B);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square C1 = new Square(1, ChessFile.C);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square D1 = new Square(1, ChessFile.D);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square E1 = new Square(1, ChessFile.E);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square F1 = new Square(1, ChessFile.F);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square G1 = new Square(1, ChessFile.G);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Square class is inmutable")]
		public readonly static Square H1 = new Square(1, ChessFile.H);
		#endregion

		#region Equality Implementation
		public override bool Equals(object obj)
		{
			Square b = obj as Square;
			if (obj == null)
			{
				return false;
			}

			return this == b;
		}

		public bool Equals(Square other)
		{
			return this == other;
		}

		public override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 23 + Rank.GetHashCode();
			hash = hash * 23 + File.GetHashCode();
			return hash;
		}

		public static bool operator ==(Square a, Square b)
		{
			if (Object.ReferenceEquals(a, b))
			{
				return true;
			}

			if ((object)a == null || (object)b == null)
			{
				return false;
			}

			return a.File == b.File && a.Rank == b.Rank;
		}

		public static bool operator !=(Square a, Square b)
		{
			return !(a == b);
		}
		#endregion

		#region Comparable Implementation
		public int CompareTo(Square other)
		{
			if (Rank != other.Rank)
			{
				return other.Rank - Rank;
			}

			if (File != other.File)
			{
				return File - other.File;
			}

			return 0;
		}
		#endregion
	}

	public enum PieceColor
	{ None, White, Black }

	public enum PieceFigure
	{ None, Pawn, Knight, Bishop, Rook, Queen, King }

	public static class ChessExtensions
	{
		public static PieceColor ToPieceColor(this char c)
		{
			return Char.IsUpper(c) ? PieceColor.White : PieceColor.Black;
		}

		public static string ToLowerLetter(this PieceColor @this)
		{
			switch (@this)
			{
				case PieceColor.White: return "w";
				case PieceColor.Black: return "b";
				default: throw new ArgumentException("Invalid piece color");
			}
		}

		public static PieceFigure ToPieceFigure(this char c)
		{
			switch (Char.ToUpper(c))
			{
				case 'P': return PieceFigure.Pawn;
				case 'N': return PieceFigure.Knight;
				case 'B': return PieceFigure.Bishop;
				case 'R': return PieceFigure.Rook;
				case 'Q': return PieceFigure.Queen;
				case 'K': return PieceFigure.King;
			}

			throw new ArgumentException("not a valid chess figure character");
		}

		public static bool Has(this CastlingAvailability @this, CastlingAvailability castlingType)
		{
			return (@this & castlingType) != 0;
		}

		public static IEnumerable<IGrouping<int, PieceLocation>> FillMissingRanks(
		this IOrderedEnumerable<IGrouping<int, PieceLocation>> @this)
		{
			int rank = 8;
			foreach (var group in @this)
			{
				while (rank != group.Key)
				{
					yield return new Grouping<int, PieceLocation>(rank, new List<PieceLocation>());
					rank--;
				}

				yield return group;
				rank = group.Key - 1;
			}

			while (rank > 0)
			{
				yield return new Grouping<int, PieceLocation>(rank, new List<PieceLocation>());
				rank--;
			}

			yield break;
		}
	}

	public class Piece
	{
		public readonly PieceColor PieceColor;
		public readonly PieceFigure PieceFigure;

		public PieceLocation At(Square location)
		{
			return new PieceLocation(this, location);
		}

		public override string ToString()
		{
			return string.Format("{0}{1}", PieceColor, PieceFigure);
		}

		public char ToPieceLetter()
		{
			char letter;
			switch (PieceFigure)
			{
				case PieceFigure.King: letter = 'k'; break;
				case PieceFigure.Queen: letter = 'q'; break;
				case PieceFigure.Rook: letter = 'r'; break;
				case PieceFigure.Knight: letter = 'n'; break;
				case PieceFigure.Bishop: letter = 'b'; break;
				case PieceFigure.Pawn: letter = 'p'; break;
				default: throw new ArgumentException("Invalid piece figure");
			}

			if (PieceColor == PieceColor.White)
			{
				letter = Char.ToUpper(letter);
			}

			return letter;
		}

		public static Piece FromChar(char c)
		{
			return new Piece(c.ToPieceColor(), c.ToPieceFigure());
		}

		public static Piece Get(PieceColor color, PieceFigure figure)
		{
			switch (figure)
			{
				case PieceFigure.Pawn: return color == PieceColor.White ? WhitePawn : BlackPawn;
				case PieceFigure.Knight: return color == PieceColor.White ? WhiteKnight : BlackKnight;
				case PieceFigure.Bishop: return color == PieceColor.White ? WhiteBishop : BlackBishop;
				case PieceFigure.Rook: return color == PieceColor.White ? WhiteRook : BlackRook;
				case PieceFigure.Queen: return color == PieceColor.White ? WhiteQueen : BlackQueen;
				case PieceFigure.King: return color == PieceColor.White ? WhiteKing : BlackKing;
				default: return Piece.None;
			}
		}

		#region All Pieces
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="Piece class is inmutable")]
		public readonly static Piece None = new Piece(PieceColor.None, PieceFigure.None);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Piece class is inmutable")]
		public readonly static Piece WhiteKing = new Piece(PieceColor.White, PieceFigure.King);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Piece class is inmutable")]
		public readonly static Piece WhiteQueen = new Piece(PieceColor.White, PieceFigure.Queen);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Piece class is inmutable")]
		public readonly static Piece WhiteRook = new Piece(PieceColor.White, PieceFigure.Rook);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Piece class is inmutable")]
		public readonly static Piece WhiteKnight = new Piece(PieceColor.White, PieceFigure.Knight);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Piece class is inmutable")]
		public readonly static Piece WhiteBishop = new Piece(PieceColor.White, PieceFigure.Bishop);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Piece class is inmutable")]
		public readonly static Piece WhitePawn = new Piece(PieceColor.White, PieceFigure.Pawn);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Piece class is inmutable")]
		public readonly static Piece BlackKing = new Piece(PieceColor.Black, PieceFigure.King);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Piece class is inmutable")]
		public readonly static Piece BlackQueen = new Piece(PieceColor.Black, PieceFigure.Queen);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Piece class is inmutable")]
		public readonly static Piece BlackRook = new Piece(PieceColor.Black, PieceFigure.Rook);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Piece class is inmutable")]
		public readonly static Piece BlackKnight = new Piece(PieceColor.Black, PieceFigure.Knight);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Piece class is inmutable")]
		public readonly static Piece BlackBishop = new Piece(PieceColor.Black, PieceFigure.Bishop);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Piece class is inmutable")]
		public readonly static Piece BlackPawn = new Piece(PieceColor.Black, PieceFigure.Pawn);
		#endregion

		private Piece(PieceColor color, PieceFigure figure)
		{
			PieceColor = color;
			PieceFigure = figure;
		}
	}

	public class PieceLocation
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="Piece class is inmmutable")]
		public readonly Piece Piece;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="Square class is inmmutable")]
		public readonly Square Location;

		public PieceLocation(Piece piece, Square location)
		{
			Piece = piece;
			Location = location;
		}
	}

	public enum ChessFile
	{
		None, A, B, C, D, E, F, G, H
	}

	[Flags]
	public enum CastlingAvailability
	{
		None = 0,
		WhiteKingSide = 1,
		WhiteQueenSide = 2,
		WhiteBoth = 3,
		BlackKingSide = 4,
		BlackQueenSide = 8,
		BackBoth = 12
	}

	public interface BoardDataSource
	{
		IEnumerable<PieceLocation> PiecePlacement { get; }
		PieceColor ActiveColor { get; }
		CastlingAvailability CastlingAvailability { get; }
		Square EnPassantTarget { get; }
		ushort HalfmoveClock { get; }
		ushort FullmoveClock { get; }
	}

	public interface BoardDataSink
	{
		IEnumerable<PieceLocation> PiecePlacement { set; }
		PieceColor ActiveColor { set; }
		CastlingAvailability CastlingAvailability { set; }
		Square EnPassantTarget { set; }
		ushort HalfmoveClock { set; }
		ushort FullmoveClock { set; }
	}

	public class FenParser : BoardDataSource
	{
		public void Parse(string line)
		{
			// TODO what if the line can't be parsed?
			var field = line.Split(' ');
			// TODO what if fields is not six items?
			PiecePlacement = ParsePiecePlacement(field[0]);
			ActiveColor = ParseActiveColor(field[1]);
			CastlingAvailability = ParseCastlingAvailability(field[2]);
			EnPassantTarget = ParseEnPassantTarget(field[3]);
			HalfmoveClock = ParseHalfmoveClock(field[4]);
			FullmoveClock = ParseFullmoveClock(field[5]);
		}

		public IEnumerable<PieceLocation> PiecePlacement { get; private set; }
		public PieceColor ActiveColor { get; private set; }
		public CastlingAvailability CastlingAvailability { get; private set; }
		public Square EnPassantTarget { get; private set; }
		public ushort HalfmoveClock { get; private set; }
		public ushort FullmoveClock { get; private set; }

		IEnumerable<PieceLocation> ParseRankLine(string rankSpec, int rank)
		{
			ChessFile currentFile = ChessFile.A;
			foreach (Char c in rankSpec)
			{
				if (Char.IsDigit(c))
				{
					currentFile = (ChessFile)((int)currentFile + (int)Char.GetNumericValue(c));
				}
				else
				{
					var location = new Square(rank, currentFile);
					var piece = Piece.FromChar(c);
					yield return piece.At(location);
					currentFile++;
				}
			}
			// TODO what if current file is not h?
		}

		Square ParseEnPassantTarget(string field)
		{
			if (field == "-")
			{
				return Square.None;
			}

			// what if is not two chars long?
			// what if is rank is not valid?
			// what if line is not valid?
			return new Square(
				(int)Char.GetNumericValue(field[1]),
				(ChessFile)Enum.Parse(typeof(ChessFile), field[0].ToString(), true)
			);
		}

		CastlingAvailability ParseCastlingAvailability(string field)
		{
			switch (field)
			{
				case "-": return CastlingAvailability.None;
				case "q": return CastlingAvailability.BlackQueenSide;
				case "k": return CastlingAvailability.BlackKingSide;
				case "kq": return CastlingAvailability.BackBoth;
				case "Q": return CastlingAvailability.WhiteQueenSide;
				case "Qq": return CastlingAvailability.WhiteQueenSide | CastlingAvailability.BlackQueenSide;
				case "Qk": return CastlingAvailability.WhiteQueenSide | CastlingAvailability.BlackKingSide;
				case "Qkq": return CastlingAvailability.WhiteQueenSide | CastlingAvailability.BackBoth;
				case "K": return CastlingAvailability.WhiteKingSide;
				case "Kq": return CastlingAvailability.WhiteKingSide | CastlingAvailability.BlackQueenSide;
				case "Kk": return CastlingAvailability.WhiteKingSide | CastlingAvailability.BlackKingSide;
				case "Kkq": return CastlingAvailability.WhiteKingSide | CastlingAvailability.BackBoth;
				case "KQ": return CastlingAvailability.WhiteBoth;
				case "KQq": return CastlingAvailability.WhiteBoth | CastlingAvailability.BlackQueenSide;
				case "KQk": return CastlingAvailability.WhiteBoth | CastlingAvailability.BlackKingSide;
				case "KQkq": return CastlingAvailability.WhiteBoth | CastlingAvailability.BackBoth;
				// TODO what if the castling is incorrect?
			}

			throw new NotImplementedException("Implementation not completed. Error handling is missing");
		}

		PieceColor ParseActiveColor(string field)
		{
			// TODO field[1] should be lower case per the specification
			switch (field)
			{
				case "w": return PieceColor.White;
				case "b": return PieceColor.Black;
				// TODO what if we got a bad character?
			}
			throw new NotImplementedException("finish this implementation. Error handling is missing.");
		}

		IEnumerable<PieceLocation> ParsePiecePlacement(string field)
		{
			var ranksSpec = field.Split('/');
			// TODO what if there aren't 8 ranks?
			return ranksSpec.SelectMany((rank, index) => ParseRankLine(rank, 8 - index));
		}

		ushort ParseHalfmoveClock(string field)
		{
			// TODO what if the field is invalid?
			return UInt16.Parse(field);
		}

		ushort ParseFullmoveClock(string field)
		{
			// TODO what if the field is invalid?
			return UInt16.Parse(field);
		}
	}

	public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
	{
		public TKey Key
		{ get; private set; }

		public IEnumerator<TElement> GetEnumerator()
		{
			return Elements.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Grouping(TKey key, IEnumerable<TElement> elements)
		{
			Key = key;
			Elements = elements;
		}

		readonly IEnumerable<TElement> Elements;
	}

	public class FenGenerator : BoardDataSink
	{
		public string Generate()
		{
			return string.Join(" ", PiecesField, ActiveColorField, CastlingField, EnPassantField, HalfmoveClockField, FullmoveClockField);
		}

		public IEnumerable<PieceLocation> PiecePlacement
		{
			set
			{
				PiecesField = string.Join("/", value.GroupBy(pl => pl.Location.Rank)
											  .OrderByDescending(g => g.Key)
											  .FillMissingRanks()
											  .Select(g => ToLine(g.OrderBy(pl => pl.Location.File))));
			}
		}

		private string ToLine(IOrderedEnumerable<PieceLocation> pieces)
		{
			var line = new StringBuilder();
			var currentFile = ChessFile.A;
			int skipped;
			foreach (var piece in pieces)
			{
				skipped = piece.Location.File - currentFile;
				if (skipped > 0)
					line.Append(skipped);
				line.Append(piece.Piece.ToPieceLetter());
				currentFile = piece.Location.File + 1;
			}

			skipped = 1 + ChessFile.H - currentFile;
			if (skipped > 0)
				line.Append(skipped);

			return line.ToString();
		}

		public PieceColor ActiveColor
		{
			set
			{
				ActiveColorField = value.ToLowerLetter();
			}
		}

		public CastlingAvailability CastlingAvailability
		{
			set
			{
				var field = new StringBuilder();
				if (value.Has(CastlingAvailability.WhiteKingSide)) field.Append("K");
				if (value.Has(CastlingAvailability.WhiteQueenSide)) field.Append("Q");
				if (value.Has(CastlingAvailability.BlackKingSide)) field.Append("k");
				if (value.Has(CastlingAvailability.BlackQueenSide)) field.Append("q");
				CastlingField = field.Length > 0 ? field.ToString() : "-";
			}
		}

		public Square EnPassantTarget
		{
			set
			{
				EnPassantField = value != Square.None ? value.ToString().ToLower() : "-";
			}
		}

		public ushort HalfmoveClock
		{
			set
			{
				HalfmoveClockField = value.ToString();
			}
		}

		public ushort FullmoveClock
		{
			set
			{
				FullmoveClockField = value.ToString();
				// TODO what if FullmoveClock is 0?
			}
		}

		string PiecesField = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
		string ActiveColorField = "w";
		string CastlingField = "KQkq";
		string EnPassantField = "-";
		string HalfmoveClockField = "0";
		string FullmoveClockField = "1";
	}

	public class BoardCopier
	{
		public BoardCopier(BoardDataSource source, BoardDataSink sink)
		{
			Source = source;
			Sink = sink;
		}

		public void Copy()
		{
			Sink.PiecePlacement = Source.PiecePlacement;
			Sink.ActiveColor = Source.ActiveColor;
			Sink.CastlingAvailability = Source.CastlingAvailability;
			Sink.EnPassantTarget = Source.EnPassantTarget;
			Sink.HalfmoveClock = Source.HalfmoveClock;
			Sink.FullmoveClock = Source.FullmoveClock;
		}

		BoardDataSource Source;
		BoardDataSink Sink;
	}

	public struct BitBoard
	{
		public BitBoard(ulong bits)
		{
			Bits = bits;
		}

		public BitBoard(params Square[] squares)
		{
			Bits = 0;
			foreach (var square in squares)
			{
				Set(square);
			}
		}

		public void ClearAll()
		{
			Bits = 0;
		}

		public void Set(Square location)
		{
			Bits |= (ulong)1 << location.BitIndex;
		}

		internal void Clear(Square location)
		{
			Bits &= ~((ulong)1 << location.BitIndex);
		}

		public bool IsSet(Square Location)
		{
			return (Bits & ((ulong)1 << Location.BitIndex)) != 0;
		}

		internal bool IsSet(int i)
		{
			return (Bits & ((ulong)1 << i)) != 0;
		}

		// TODO are we testing this?
		static public BitBoard operator |(BitBoard a, BitBoard b)
		{
			return new BitBoard(a.Bits | b.Bits);
		}

		static public BitBoard operator &(BitBoard a, BitBoard b)
		{
			return new BitBoard(a.Bits & b.Bits);
		}

		ulong Bits;
	}

	public class Board : BoardDataSink, BoardDataSource
	{
		public IEnumerable<PieceLocation> PiecePlacement
		{
			get
			{
				var allPieces = BlackBitBoard | WhiteBitBoard;
				return Enumerable.Range(0, 64)
								.Where(i => allPieces.IsSet(i))
								.Select(i => ToPieceLocation(i));
			}
			set
			{
				BlackBitBoard.ClearAll();
				WhiteBitBoard.ClearAll();
				KingBitBoard.ClearAll();
				QueenBitBoard.ClearAll();
				RookBitBoard.ClearAll();
				BishopBitBoard.ClearAll();
				KnightBitBoard.ClearAll();
				PawnBitBoard.ClearAll();

				foreach (var pieceLocation in value)
					PlacePiece(pieceLocation.Piece, pieceLocation.Location);
			}
		}
		
		public Piece this[Square index]
		{
			get { return PieceAt(index.BitIndex); }
			set { PlacePiece(value, index); }
		}

		// TODO is this being fully tested?
		public void Move(Square from, Square to)
		{
			// TODO what if the move is invalid?
			PlacePiece(RemovePiece(from), to);
			// TODO what if from is empty? bad moe, throw exception
			// TODO what if to is not empty? implement captures!
		}

		public static BitBoard WhiteAttacks
		{
			get
			{
				return new BitBoard(Square.D3, Square.F3);
			}
		}

		public PieceColor ActiveColor { get; set; }
		public CastlingAvailability CastlingAvailability { get; set; }
		public Square EnPassantTarget { get; set; }
		public ushort HalfmoveClock { get; set; }
		public ushort FullmoveClock { get; set; }

		PieceLocation ToPieceLocation(int i)
		{
			return new PieceLocation(PieceAt(i), Square.For(i));
		}

		Piece RemovePiece(Square location)
		{
			var piece = PieceAt(location.BitIndex);

			switch (piece.PieceFigure)
			{
				case PieceFigure.Pawn: PawnBitBoard.Clear(location); break;
				case PieceFigure.Knight: KnightBitBoard.Clear(location); break;
				case PieceFigure.Bishop: BishopBitBoard.Clear(location); break;
				case PieceFigure.Rook: RookBitBoard.Clear(location); break;
				case PieceFigure.Queen: QueenBitBoard.Clear(location); break;
				case PieceFigure.King: KingBitBoard.Clear(location); break;
			}

			switch (piece.PieceColor)
			{
				case PieceColor.White: WhiteBitBoard.Clear(location); break;
				case PieceColor.Black: BlackBitBoard.Clear(location); break;
			}

			return piece;
		}

		void PlacePiece(Piece piece, Square location)
		{
			switch (piece.PieceFigure)
			{
				case PieceFigure.Pawn: PawnBitBoard.Set(location); break;
				case PieceFigure.Knight: KnightBitBoard.Set(location); break;
				case PieceFigure.Bishop: BishopBitBoard.Set(location); break;
				case PieceFigure.Rook: RookBitBoard.Set(location); break;
				case PieceFigure.Queen: QueenBitBoard.Set(location); break;
				case PieceFigure.King: KingBitBoard.Set(location); break;
			}

			switch (piece.PieceColor)
			{
				case PieceColor.White: WhiteBitBoard.Set(location); break;
				case PieceColor.Black: BlackBitBoard.Set(location); break;
			}
		}

		Piece PieceAt(int i)
		{
			return Piece.Get(
				BlackBitBoard.IsSet(i) ? PieceColor.Black : PieceColor.White,
				GetPieceFigure(i)
				);
		}

		PieceFigure GetPieceFigure(int i)
		{
			if (RookBitBoard.IsSet(i)) return PieceFigure.Rook;
			if (BishopBitBoard.IsSet(i)) return PieceFigure.Bishop;
			if (KnightBitBoard.IsSet(i)) return PieceFigure.Knight;
			if (QueenBitBoard.IsSet(i)) return PieceFigure.Queen;
			if (KingBitBoard.IsSet(i)) return PieceFigure.King;
			if (PawnBitBoard.IsSet(i)) return PieceFigure.Pawn;

			return PieceFigure.None;
		}

		BitBoard BlackBitBoard;
		BitBoard WhiteBitBoard;
		BitBoard KingBitBoard;
		BitBoard QueenBitBoard;
		BitBoard RookBitBoard;
		BitBoard BishopBitBoard;
		BitBoard KnightBitBoard;
		BitBoard PawnBitBoard;
	}

	[TestClass]
	public class ChessTests
	{
		FenGenerator FenGenerator;
		FenParser FenParser;
		Board Board;

		#region Example Fen Data
		string AfterMove_1_e4_Fen =
			"rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
		string EndGameFen = "4k3/8/8/8/8/8/4P3/4K3 w - - 5 39";
		string StartingPositionFen =
			"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
		#endregion

		[TestInitialize]
		public void BeforeEach()
		{
			FenParser = new FenParser();
			FenGenerator = new FenGenerator();
			Board = new Board();
		}

		[TestMethod]
		public void FenParser_PiecePlacement_ReturnsCorrectResultsForSimplePosition()
		{
			var expected = new List<PieceLocation>() {
				Piece.BlackKing.At(Square.E8),
				Piece.WhitePawn.At(Square.E2),
				Piece.WhiteKing.At(Square.E1)
			};

			FenParser.Parse(EndGameFen);
			var actual = FenParser.PiecePlacement.ToList();

			var x = new PieceLocationEqualityComparer();
			CollectionAssert.AreEqual(expected, actual, x);
		}

		[TestMethod]
		public void FenParser_PiecePlacement_ReturnsCorrectResultsForStartingPosition()
		{
			var expected = new List<PieceLocation>() {
				new PieceLocation(Piece.BlackRook, Square.A8),
				new PieceLocation(Piece.BlackKnight, Square.B8),
				new PieceLocation(Piece.BlackBishop, Square.C8),
				new PieceLocation(Piece.BlackQueen, Square.D8),
				new PieceLocation(Piece.BlackKing, Square.E8),
				new PieceLocation(Piece.BlackBishop, Square.F8),
				new PieceLocation(Piece.BlackKnight, Square.G8),
				new PieceLocation(Piece.BlackRook, Square.H8),
				new PieceLocation(Piece.BlackPawn, Square.A7),
				new PieceLocation(Piece.BlackPawn, Square.B7),
				new PieceLocation(Piece.BlackPawn, Square.C7),
				new PieceLocation(Piece.BlackPawn, Square.D7),
				new PieceLocation(Piece.BlackPawn, Square.E7),
				new PieceLocation(Piece.BlackPawn, Square.F7),
				new PieceLocation(Piece.BlackPawn, Square.G7),
				new PieceLocation(Piece.BlackPawn, Square.H7),
				new PieceLocation(Piece.WhitePawn, Square.A2),
				new PieceLocation(Piece.WhitePawn, Square.B2),
				new PieceLocation(Piece.WhitePawn, Square.C2),
				new PieceLocation(Piece.WhitePawn, Square.D2),
				new PieceLocation(Piece.WhitePawn, Square.E2),
				new PieceLocation(Piece.WhitePawn, Square.F2),
				new PieceLocation(Piece.WhitePawn, Square.G2),
				new PieceLocation(Piece.WhitePawn, Square.H2),
				new PieceLocation(Piece.WhiteRook, Square.A1),
				new PieceLocation(Piece.WhiteKnight, Square.B1),
				new PieceLocation(Piece.WhiteBishop, Square.C1),
				new PieceLocation(Piece.WhiteQueen, Square.D1),
				new PieceLocation(Piece.WhiteKing, Square.E1),
				new PieceLocation(Piece.WhiteBishop, Square.F1),
				new PieceLocation(Piece.WhiteKnight, Square.G1),
				new PieceLocation(Piece.WhiteRook, Square.H1),
			};

			FenParser.Parse(StartingPositionFen);
			var actual = FenParser.PiecePlacement.ToArray();

			var x = new PieceLocationEqualityComparer();
			CollectionAssert.AreEqual(expected, actual, x);
		}

		[TestMethod]
		public void FenParser_ActiveColor_ReturnsCorrectResults()
		{
			FenParser.Parse(EndGameFen);

			Assert.AreEqual(PieceColor.White, FenParser.ActiveColor);
		}

		[TestMethod]
		public void FenParser_CastlingAvailability_RetunsCorrectResults()
		{
			var tests = new List<Tuple<string, CastlingAvailability>>()
			{
				new Tuple<string, CastlingAvailability>("k7/8/8/8/8/8/8/K7 w - - 0 1", CastlingAvailability.None),
				new Tuple<string, CastlingAvailability>("k7/8/8/8/8/8/8/K7 w Kq - 0 1", CastlingAvailability.WhiteKingSide | CastlingAvailability.BlackQueenSide),
				new Tuple<string, CastlingAvailability>("k7/8/8/8/8/8/8/K7 w KQkq - 0 1", CastlingAvailability.WhiteKingSide | CastlingAvailability.WhiteQueenSide | CastlingAvailability.BlackKingSide | CastlingAvailability.BlackQueenSide),
			};

			foreach (var test in tests)
			{
				FenParser.Parse(test.Item1);
				Assert.AreEqual(test.Item2, FenParser.CastlingAvailability);
			}
		}

		[TestMethod]
		public void FenParser_EnPassantTarget_ReturnsCorrectResults()
		{
			FenParser.Parse(EndGameFen);
			Assert.AreEqual(Square.None, FenParser.EnPassantTarget);

			FenParser.Parse(AfterMove_1_e4_Fen);
			Assert.AreEqual(Square.E3, FenParser.EnPassantTarget);
		}

		[TestMethod]
		public void FenParser_HalfmoveClock_ReturnCorrectResults()
		{
			FenParser.Parse(AfterMove_1_e4_Fen);
			Assert.AreEqual(0u, FenParser.HalfmoveClock);

			FenParser.Parse(EndGameFen);
			Assert.AreEqual(5u, FenParser.HalfmoveClock);
		}

		[TestMethod]
		public void FenParser_FullmoveClock_ReturnsCorrectResults()
		{
			FenParser.Parse(AfterMove_1_e4_Fen);
			Assert.AreEqual((ushort)1, FenParser.FullmoveClock);

			FenParser.Parse(EndGameFen);
			Assert.AreEqual((ushort)39, FenParser.FullmoveClock);
		}

		[TestMethod]
		public void FenGenerator_Generate_WithDefaults_CreatesGoodOutput()
		{
			string actual = FenGenerator.Generate();

			Assert.AreEqual(StartingPositionFen, actual);
		}

		[TestMethod]
		public void FenGenerator_Generate_SettingPiecePlacement_CreatesGoodOutput()
		{
			string afterMove_1_e4_FenPiecePlacement =
				"rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR";
			FenGenerator.PiecePlacement = new List<PieceLocation>()
			{
				#region Rank 8
				new PieceLocation(Piece.BlackRook, Square.A8),
				new PieceLocation(Piece.BlackKnight, Square.B8),
				new PieceLocation(Piece.BlackBishop, Square.C8),
				new PieceLocation(Piece.BlackQueen, Square.D8),
				new PieceLocation(Piece.BlackKing, Square.E8),
				new PieceLocation(Piece.BlackBishop, Square.F8),
				new PieceLocation(Piece.BlackKnight, Square.G8),
				new PieceLocation(Piece.BlackRook, Square.H8),
				#endregion
				#region Rank 7
				new PieceLocation(Piece.BlackPawn, Square.A7),
				new PieceLocation(Piece.BlackPawn, Square.B7),
				new PieceLocation(Piece.BlackPawn, Square.C7),
				new PieceLocation(Piece.BlackPawn, Square.D7),
				new PieceLocation(Piece.BlackPawn, Square.E7),
				new PieceLocation(Piece.BlackPawn, Square.F7),
				new PieceLocation(Piece.BlackPawn, Square.G7),
				new PieceLocation(Piece.BlackPawn, Square.H7),
				#endregion
				#region Rank 4
				new PieceLocation(Piece.WhitePawn, Square.E4),
				#endregion
				#region Rank 2
				new PieceLocation(Piece.WhitePawn, Square.A2),
				new PieceLocation(Piece.WhitePawn, Square.B2),
				new PieceLocation(Piece.WhitePawn, Square.C2),
				new PieceLocation(Piece.WhitePawn, Square.D2),
				new PieceLocation(Piece.WhitePawn, Square.F2),
				new PieceLocation(Piece.WhitePawn, Square.G2),
				new PieceLocation(Piece.WhitePawn, Square.H2),
				#endregion
				#region Rank 1
				new PieceLocation(Piece.WhiteRook, Square.A1),
				new PieceLocation(Piece.WhiteKnight, Square.B1),
				new PieceLocation(Piece.WhiteBishop, Square.C1),
				new PieceLocation(Piece.WhiteQueen, Square.D1),
				new PieceLocation(Piece.WhiteKing, Square.E1),
				new PieceLocation(Piece.WhiteBishop, Square.F1),
				new PieceLocation(Piece.WhiteKnight, Square.G1),
				new PieceLocation(Piece.WhiteRook, Square.H1),
				#endregion
			};
			var fen = FenGenerator.Generate();
			var actual = fen.Split(' ')[0];
			Assert.AreEqual(afterMove_1_e4_FenPiecePlacement, actual);

			string endGameFenPiecePlacement = "4k3/8/8/8/8/8/4P3/4K3";
			FenGenerator.PiecePlacement = new List<PieceLocation>()
			{
				new PieceLocation(Piece.BlackKing, Square.E8),
				new PieceLocation(Piece.WhitePawn, Square.E2),
				new PieceLocation(Piece.WhiteKing, Square.E1)
			};
			fen = FenGenerator.Generate();
			actual = fen.Split(' ')[0];
			Assert.AreEqual(endGameFenPiecePlacement, actual);
		}

		[TestMethod]
		public void FenGenerator_Generate_SettingActiveColor_CreatesGoodOutput()
		{
			FenGenerator.ActiveColor = PieceColor.Black;
			var color = FenGenerator.Generate().Split(' ')[1];
			Assert.AreEqual("b", color);

			FenGenerator.ActiveColor = PieceColor.White;
			color = FenGenerator.Generate().Split(' ')[1];
			Assert.AreEqual("w", color);
		}

		[TestMethod]
		public void FenGenerator_Generate_SettingCastling_CreatesGoodOutput()
		{
			FenGenerator.CastlingAvailability = CastlingAvailability.None;
			var actual = FenGenerator.Generate().Split(' ')[2];
			Assert.AreEqual("-", actual);

			FenGenerator.CastlingAvailability = CastlingAvailability.WhiteBoth | CastlingAvailability.BlackQueenSide;
			actual = FenGenerator.Generate().Split(' ')[2];
			Assert.AreEqual("KQq", actual);
		}

		[TestMethod]
		public void FenGenerator_Generate_SettingEnPassant_CreatesGoodOutput()
		{
			FenGenerator.EnPassantTarget = Square.None;
			var actual = FenGenerator.Generate().Split(' ')[3];
			Assert.AreEqual("-", actual);

			FenGenerator.EnPassantTarget = Square.E3;
			actual = FenGenerator.Generate().Split(' ')[3];
			Assert.AreEqual("e3", actual);

			FenGenerator.EnPassantTarget = Square.A6;
			actual = FenGenerator.Generate().Split(' ')[3];
			Assert.AreEqual("a6", actual);

			// TODO what if enpassant square is not in the 3 or 6 rank?
		}

		[TestMethod]
		public void FenGenerator_Generate_SettingTheClocks_CreatedGoodOutput()
		{
			FenGenerator.HalfmoveClock = 42;
			var actual = FenGenerator.Generate().Split(' ')[4];
			Assert.AreEqual("42", actual);

			FenGenerator.FullmoveClock = 42;
			actual = FenGenerator.Generate().Split(' ')[5];
			Assert.AreEqual("42", actual);
		}

		[TestMethod]
		public void SquareCompareTo_Works()
		{
			Assert.IsTrue(Square.A1.CompareTo(Square.A1) == 0);
			Assert.IsTrue(Square.A1.CompareTo(Square.A2) > 0);
			Assert.IsTrue(Square.A1.CompareTo(Square.B1) < 0);
		}

		[TestMethod]
		public void SquareOrderBy_Works()
		{
			var sampleList = new List<Square>() { Square.A8, Square.H8, Square.A1, Square.H1 };
			var actual = sampleList.OrderBy(i => i);

			CollectionAssert.AreEqual(sampleList.ToArray(), actual.ToArray());
		}

		[TestMethod]
		public void BoardCopier_Copy_CopiesAllValuesFromSourceToSink()
		{
			FenParser.Parse(EndGameFen);

			var sut = new BoardCopier(FenParser, FenGenerator);
			sut.Copy();

			Assert.AreEqual(EndGameFen, FenGenerator.Generate());
		}

		[TestMethod]
		public void Square_ToBitIndex()
		{
			Assert.AreEqual(0, Square.A1.BitIndex);
			Assert.AreEqual(7, Square.H1.BitIndex);
			Assert.AreEqual(8, Square.A2.BitIndex);
			Assert.AreEqual(63, Square.H8.BitIndex);
		}

		[TestMethod]
		public void ToPieceFigure_Bug_NotAllPiecesWhereMapped()
		{
			Assert.AreEqual(PieceFigure.King, 'k'.ToPieceFigure());
			Assert.AreEqual(PieceFigure.Queen, 'q'.ToPieceFigure());
			Assert.AreEqual(PieceFigure.Rook, 'r'.ToPieceFigure());
			Assert.AreEqual(PieceFigure.Bishop, 'b'.ToPieceFigure());
			Assert.AreEqual(PieceFigure.Knight, 'n'.ToPieceFigure());
			Assert.AreEqual(PieceFigure.Pawn, 'p'.ToPieceFigure());
		}

		[TestMethod]
		public void Board_StoresPiecesCorrectly_ForTheStartingPositionFen()
		{
			TestBoardCopy(StartingPositionFen);
		}

		[TestMethod]
		public void Board_StoresPiecesCorrectly_ForTheEndGameFen()
		{
			TestBoardCopy(EndGameFen);
		}

		[TestMethod]
		public void Board_StoresPiecesCorrectly_ForTheOneMoveFen()
		{
			TestBoardCopy(AfterMove_1_e4_Fen);
		}

		[TestMethod]
		public void Board_SquareIndexer_ReturnsCorrectResults()
		{
			LoadBoard(StartingPositionFen);

			Assert.AreEqual(Piece.WhiteRook, Board[Square.A1]);
			Assert.AreEqual(Piece.BlackKing, Board[Square.E8]);
			Assert.AreEqual(Piece.None, Board[Square.E4]);
		}

		[TestMethod]
		public void Board_MovePiece_ModifiesTheBoard()
		{
			LoadBoard(StartingPositionFen);

			Board.Move(Square.E2, Square.E4);

			Assert.AreEqual(Piece.None, Board[Square.E2]);
			Assert.AreEqual(Piece.WhitePawn, Board[Square.E4]);
		}

		[TestMethod]
		public void BitBoard_SquareArrayConstructor()
		{
			BitBoard a = new BitBoard(Square.A1);
			Assert.IsTrue(a.IsSet(Square.A1));

			BitBoard b = new BitBoard(Square.A1, Square.H8);
			Assert.IsTrue(b.IsSet(Square.A1));
			Assert.IsTrue(b.IsSet(Square.H8));
		}

		[TestMethod]
		public void Board_IndexerBySquare_ReturnsPiece()
		{
			Board[Square.D1] = Piece.WhiteQueen;
			Assert.AreEqual(Piece.WhiteQueen, Board[Square.D1]);
		}

		[TestMethod]
		public void BitBoard_BitAndOperator_Works()
		{
			var a = new BitBoard(Square.A1);
			Assert.AreEqual(a, a & a);
		}

		[TestMethod]
		public void Board_WhenAPawnIsSet_AttacksAreUpdated()
		{
			var expectedAttacks = new BitBoard(Square.D3, Square.F3);
			Board[Square.E2] = Piece.WhitePawn;
			Assert.AreEqual(Board.WhiteAttacks & expectedAttacks, expectedAttacks);
		}

		// attacks for black pawns
		// attacks for rook
		// attacks for knights
		// attacks for bishops
		// attacks for kings
		// attacks for queens

		// attacks for horiz sliders blocked by other pieces
		// attacks for vert sliders blocked by other pieces
		// attacks for diag sliders blocked by other pieces

		// stalemate
		//      player doesn't have any moves (game is a draw)
		// position is check mate
		//          king is in check and doesn't have moves to escape
		/*
For chess the classic way to detect check is to examine the king's square. Pretend that the king is a knight and see if it can capture any opponent knights; pretend it is a bishop and see if it can capture an opposing bishop, etc for all the different chess pieces.

But, all you really need to look at is the opponent's last move and answer two questions: Did the move cause check directly? Did it reveal check from a sliding piece? This is usually the fastest method, but moves like castling or ep are quite difficult to get things right, so you might want to revert to the classic method as described above for the uncommon cases of castling and ep captures.

An alternative approach is to maintain an incremental attack board which lets you know immediately if the king is attacked. Maintaining this board, however, will slow down your program noticeably...
		*/
		// position is check
		// attacks
		// pins

		// piece movement
		//  can't move to square with piece of same color
		//  capture if moved into square with piece of opponent
		//  queen, rook, bishop
		//  knight
		//  pawns, front, 2 front, diagonal capture, en passant capture, promotion
		//  king, one square sorrounding, castling
		//    moving king, looses castling        
		//    moving rook, looses castling with that rook
		//    squares between king and rook must be empty.
		//    3 squares king is in, goes thru, ends up during castling must not be attacked
		//  king left in check is ilegal

		// trap ilegal moves. Implement legal moves
		// make move, pgn move spec
		// increment clocks on moves

		// unmake moves

		// pgn parser
		// pgn generator

		// game's board history
		// game's moves history
		// move generator
		// check, checkmate, stalemate
		// hashcode

		// board evaluation function

		// end game support from chess tables
		// opening books support

		private void TestBoardCopy(string fen)
		{
			var FenToBoard = new BoardCopier(FenParser, Board);
			var BoardToFen = new BoardCopier(Board, FenGenerator);

			FenParser.Parse(fen);
			FenToBoard.Copy();
			BoardToFen.Copy();

			Assert.AreEqual(fen, FenGenerator.Generate());
		}

		private void LoadBoard(string fen)
		{
			FenParser.Parse(fen);
			var copier = new BoardCopier(FenParser, Board);
			copier.Copy();
		}

		class PieceLocationEqualityComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				var a = x as PieceLocation;
				var b = y as PieceLocation;

				if (object.ReferenceEquals(x, y))
				{
					return 0;
				}

				if ((object)a == null)
				{
					return -1;
				}

				if ((object)b == null)
				{
					return 1;
				}

				if (a.Piece.PieceColor == b.Piece.PieceColor &&
					a.Piece.PieceFigure == b.Piece.PieceFigure &&
					a.Location.Rank == b.Location.Rank &&
					a.Location.File == b.Location.File)
				{
					return 0;
				}

				return 1;
			}
		}
	}
}