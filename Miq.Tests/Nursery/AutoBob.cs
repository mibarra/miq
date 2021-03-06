﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace Miq.Tests.Nursery
{
	[TestClass]
	public class AutoBob
	{
		class DropAlpha
		{
			static Random Rng = new Random();

			static double CenterAlpha()
			{
				return 0.4 + Rng.NextDouble() * 0.2;
			}

			static double EdgeAlpha()
			{
				return 0.05 + Rng.NextDouble() * 0.02;
			}

			static double CornerAlpha()
			{
				return 0.01 + Rng.NextDouble() * 0.02;
			}

			enum CoordinateTypes
			{
				Center,
				Edge,
				Corner
			}

			static CoordinateTypes CoordinateType(int i, int j)
			{
				if (i == j && i == 1)
				{
					return CoordinateTypes.Center;
				}

				if (i == j)
				{
					return CoordinateTypes.Corner;
				}

				if ((i == 2 && j == 0) || (i == 0 || j == 2))
				{
					return CoordinateTypes.Corner;
				}
				return CoordinateTypes.Edge;
			}

			public double this[int i, int j]
			{
				get
				{
					if (i < 0 || i > 2 || j < 0 || j > 2)
					{
						throw new ArgumentOutOfRangeException();
					}

					switch (CoordinateType(i, j))
					{
						case CoordinateTypes.Center: return CenterAlpha();
						case CoordinateTypes.Edge: return EdgeAlpha();
						case CoordinateTypes.Corner: return CornerAlpha();
						default: throw new InvalidOperationException("Internal Program Error");
					}
				}
			}
		}

		class PaintDrop
		{
			public System.Drawing.Point Position { get; private set; }
			public System.Drawing.Color Color { get; private set; }
			public DropAlpha Alpha { get; private set; }

			public PaintDrop(System.Drawing.Point position, System.Drawing.Color color)
			{
				Position = position;
				Color = color;
				Alpha = new DropAlpha();
			}
		}

		class Canvas
		{
			public Canvas(System.Drawing.Size size, System.Drawing.Color backgroundColor)
			{
				Size = size;
				LineStride = size.Width * 3;
				Pixels = new byte[LineStride * size.Height];
				Fill(backgroundColor);
			}

			public System.Drawing.Color Blend(PaintDrop drop)
			{
				System.Drawing.Color canvasPixelColor = System.Drawing.Color.White;

				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						var alpha = drop.Alpha[i, j];
						var offset = new System.Drawing.Size(i - 1, j - 1);
						var origColor = Blend(drop.Position + offset, drop.Color, alpha);
						if (i == 1 && j == 1)
						{
							canvasPixelColor = origColor;
						}
					}
				}
				return canvasPixelColor;
			}

			public System.Drawing.Color GetColor(System.Drawing.Point position)
			{
				return GetColor(Index(position));
			}

			public void Save(string filename)
			{
				var format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
				using (var bitmap = new Bitmap(Size.Width, Size.Height, format))
				{
					var cover = new Rectangle(0, 0, Size.Width, Size.Height);
					System.Drawing.Imaging.BitmapData data = bitmap.LockBits(cover, System.Drawing.Imaging.ImageLockMode.ReadWrite, format);
					Marshal.Copy(Pixels, 0, data.Scan0, Pixels.Length);
					bitmap.UnlockBits(data);
					bitmap.Save(filename);
				}
			}

			/*
			WriteableBitmap wbm = new WriteableBitmap(1024, 768, 43, 43, System.Windows.Media.PixelFormats.Rgb24, null);
			wbm.WritePixels(new System.Windows.Int32Rect(0, 0, 1024, 768), pixels, 1024 * 3, 0);
			var z = (BitmapSource)wbm;
			// z can be set as a image source in wpf's image control.
			*/

			byte BlendComponent(byte source, byte destination, double alpha)
			{
				return (byte)(alpha * source + (1 - alpha) * destination);
			}

			System.Drawing.Color Blend(System.Drawing.Point position, System.Drawing.Color color, double alpha)
			{
				int index = Index(position);
				if (index < 0 || index >= Pixels.Length)
				{
					return System.Drawing.Color.White;
				}
				System.Drawing.Color origColor = GetColor(index);
				Pixels[index] = BlendComponent(color.B, Pixels[index], alpha);
				Pixels[index + 1] = BlendComponent(color.G, Pixels[index + 1], alpha);
				Pixels[index + 2] = BlendComponent(color.R, Pixels[index + 2], alpha);
				return origColor;
			}

			private System.Drawing.Color GetColor(int pixelIndex)
			{
				return System.Drawing.Color.FromArgb(Pixels[pixelIndex + 2], Pixels[pixelIndex + 1], Pixels[pixelIndex]);
			}

			int Index(System.Drawing.Point position)
			{
				return Index(position.X, position.Y);
			}

			int Index(int x, int y)
			{
				return x * 3 + y * LineStride;
			}

			void Fill(System.Drawing.Color backgroundColor)
			{
				for (int y = 0; y < 768; y++)
				{
					for (int x = 0; x < 1024; x++)
					{
						int index = Index(x, y);
						Pixels[index + 0] = backgroundColor.B;
						Pixels[index + 1] = backgroundColor.G;
						Pixels[index + 2] = backgroundColor.R;
					}
				}
			}

			System.Drawing.Size Size;
			byte[] Pixels;
			int LineStride;
		}

		class PaintingEntity
		{
			public PaintingEntity(Canvas canvas, double blendingAdjustment)
			{
				Canvas = canvas;
				// Check blending is between -0.03 and  0.03
				BlendingAdjustment = blendingAdjustment;
				Rng = new Random();
			}

			public Canvas Canvas { get; private set; }
			public double PaintAmount
			{
				get { return _PaintAmount; }
				private set { _PaintAmount = Math.Max(0, Math.Min(100, value)); }
			}
			public System.Drawing.Color PaintColor
			{
				get
				{
					return _PaintColor;
					/*					double maxAdjustment = (Rng.NextDouble() - 0.5) * 20;
										double adjustmentFor3dEffect = 1 + (maxAdjustment * (PaintAmount - 80) / 20);
										return System.Drawing.Color.FromArgb(
											(int)Math.Round(paintColor.R * adjustmentFor3dEffect),
											(int)Math.Round(paintColor.G * adjustmentFor3dEffect),
											(int)Math.Round(paintColor.B * adjustmentFor3dEffect));*/
				}
				private set
				{
					_PaintColor = value;
				}
			}
			public PaintingEntityState State { get; private set; }

			public void Clean()
			{
				PaintAmount = 0;
				State = PaintingEntityState.DryWithNoPaint;
			}

			public void Load(System.Drawing.Color paintColor, int paintAmount)
			{
				PaintAmount = Math.Max(0, Math.Min(100, paintAmount));
				PaintColor = paintColor;
				// ZZZ what if PaintAmount is 0?
				State = PaintingEntityState.WetLoaded;
			}

			public void Line(System.Drawing.Point a, System.Drawing.Point b)
			{
				// Bresenham's Line Algorithm From wikipedia's page.
				int dx = Math.Abs(b.X - a.X);
				int dy = Math.Abs(b.Y - a.Y);
				int sx = a.X < b.X ? 1 : -1;
				int sy = a.Y < b.Y ? 1 : -1;
				int err = dx - dy;

				var p = new System.Drawing.Point(a.X, a.Y);
				for (; ; )
				{
					DragOverCanvas(p);
					if (p == b)
					{
						return;
					}

					int e2 = 2 * err;

					if (e2 > -dy)
					{
						err -= dy;
						p.X += sx;
					}

					if (e2 < dx)
					{
						err += dx;
						p.Y += sy;
					}
				}
			}

			public void Stab(System.Drawing.Point position)
			{
				DragOverCanvas(position);
			}

			void DragOverCanvas(System.Drawing.Point position)
			{
				PaintingEntityState nextState;
				switch (State)
				{
					case PaintingEntityState.WetLoaded:
						nextState = WetDragOverCanvas(position);
						break;
					case PaintingEntityState.DryWithPaint:
						nextState = DryDagOverCanvas(position);
						break;
					case PaintingEntityState.DryWithNoPaint:
						nextState = PickCanvasColor(position);
						break;
					default:
						throw new InvalidOperationException("PE is in an invalid state");
				}
				State = nextState;
			}

			private PaintingEntityState PickCanvasColor(System.Drawing.Point position)
			{
				PaintColor = Canvas.GetColor(position);
				return PaintingEntityState.DryWithPaint;
			}

			private PaintingEntityState DryDagOverCanvas(System.Drawing.Point position)
			{
				DropPaint(position);
				return PaintingEntityState.DryWithPaint;
			}

			void DropPaint(System.Drawing.Point position)
			{
				var drop = new PaintDrop(position, PaintColor);
				System.Drawing.Color colorOnCanvas = Canvas.Blend(drop);
				double blendingControl = BlendingControl;
				PaintColor = BlendColors(_PaintColor, colorOnCanvas, blendingControl);
			}

			System.Drawing.Color BlendColors(System.Drawing.Color paintColor, System.Drawing.Color colorOnCanvas, double blendingControl)
			{
				System.Drawing.Color newColor = System.Drawing.Color.FromArgb(
					(int)Math.Round(blendingControl * colorOnCanvas.R + (1 - blendingControl) * paintColor.R),
					(int)Math.Round(blendingControl * colorOnCanvas.G + (1 - blendingControl) * paintColor.G),
					(int)Math.Round(blendingControl * colorOnCanvas.B + (1 - blendingControl) * paintColor.B)
				);
				return newColor;
			}

			double BlendingControl
			{
				get
				{
					double blendingControl;
					if (State == PaintingEntityState.WetLoaded)
					{
						blendingControl = 0.001 + Rng.NextDouble() * 0.029 * (1 - (PaintAmount / 100));
					}
					else
					{
						// XXX need pressure stuff 
						blendingControl = 0.2 + Rng.NextDouble() * 0.75; // times pressure adjustment 1 low pressure; 0 high pressure
					}
					return blendingControl + BlendingAdjustment;
				}
			}

			PaintingEntityState WetDragOverCanvas(System.Drawing.Point position)
			{
				DropPaint(position);
				PaintAmount -= PaintDecreaseAmount;
				PaintingEntityState nextState = PaintAmount == 0
													? PaintingEntityState.DryWithPaint
													: PaintingEntityState.WetLoaded;
				return nextState;
			}

			private double PaintDecreaseAmount
			{
				get
				{
					return 0.09 + Rng.NextDouble() * 0.02;
				}
			}

			System.Drawing.Color _PaintColor;
			private double _PaintAmount;
			private Random Rng;
			private double BlendingAdjustment;
		}

		enum PaintingEntityState
		{
			WetLoaded,
			DryWithPaint,
			DryWithNoPaint
		}

		class PositionedPaintingEntity
		{
			public PaintingEntity PaintingEntity;
			public System.Drawing.Point Position;
		}

		class Brush
		{
			private Canvas Canvas;
			private System.Drawing.Size Size;
			private System.Drawing.Color PaintColor;
			private double PaintAmount;

			public Brush(Canvas canvas, System.Drawing.Size size)
			{
				Canvas = canvas;
				Size = size;
			}

			public void GeneratePaintingEntities(double blendingAdjustment)
			{
				PaintingEntities = new List<PositionedPaintingEntity>();
				foreach (System.Drawing.Point point in PaintingElementsPoints(Size.Height, Size.Width))
				{
					PaintingEntities.Add(new PositionedPaintingEntity()
					{
						PaintingEntity = new PaintingEntity(Canvas, blendingAdjustment),
						Position = new System.Drawing.Point(point.X - Size.Width / 2, point.Y - Size.Height / 2)
					});
				}
			}

			public List<PositionedPaintingEntity> PaintingEntities;

			private IEnumerable<System.Drawing.Point> GridPoints(int majorAxis, int minorAxis)
			{
				for (int x = 0; x < minorAxis; x += 3)
				{
					for (int y = 0; y < majorAxis; y += 2)
					{
						var point = new System.Drawing.Point(x, y);
						yield return point;
					}
				}
			}

			private IEnumerable<System.Drawing.Point> PaintingElementsPoints(int majorAxis, int minorAxis)
			{
				return GridPoints(majorAxis, minorAxis).Select(RandomizePoint)
													   .Where(point => PointInsideEllipse(point, majorAxis, minorAxis));
			}

			private bool PointInsideEllipse(System.Drawing.Point point, int majorAxis, int minorAxis)
			{
				double h = minorAxis / 2;
				double k = majorAxis / 2;
				double det = (Math.Pow(point.X - h, 2) / Math.Pow(h, 2)) +
							 (Math.Pow(point.Y - k, 2) / Math.Pow(k, 2));
				return det <= 1;
			}

			private System.Drawing.Point RandomizePoint(System.Drawing.Point point)
			{
				point.X += Rng.Next(-1, 1);
				point.Y += Rng.Next(-1, 1);
				return point;
			}

			Random Rng = new Random();

			//  Attack plan:
			//		can use rotation
			//		can use pressure
			//	Don't load any paint in a brush, see if it traces the line as expected
			//  refactor: make the brush ctor to take the canvas and the brush size, remove those
			//					args from GeneratePaintingEntities.
			//  refactor: think on how to best handle the bleedingAdjustment
			//  can send a stab stroke to the brush

			// Line stroke( args)
			//		args:
			//			start & end location
			//			rotation + pressure at each location
			//	sample points on the line between start & end at regular intervals
			//	calc location for each PE at each sample point
			//	send line commands to PEs for each pair of sample points

			// Stab stroke({point, rotation, pressure}, offset, direction)
			//		offset	0-5
			//		direction (angle)
			//	calculate {offset}points  {fixed distance?}
			//	draw a line with each PE for each pair of points
			//

			public void Load(System.Drawing.Color paintColor, int paintAmount)
			{
				PaintColor = paintColor;
				PaintAmount = paintAmount;
			}

			private void LoadPaintingEntities()
			{
				foreach (var element in PaintingEntities)
				{
					double randomAdjust = 1 + Rng.NextDouble() * 0.1 - 0.05;
					int amountToLoad = (int)Math.Round(PaintAmount * randomAdjust);
					element.PaintingEntity.Load(PaintColor, amountToLoad);
				}
			}

			public void Line(System.Drawing.Point a, System.Drawing.Point b)
			{
				GeneratePaintingEntities(0.0);
				LoadPaintingEntities();
				foreach (var element in PaintingEntities)
				{
					var startPosition = new System.Drawing.Point(a.X + element.Position.X, a.Y + element.Position.Y);
					var endPosition = new System.Drawing.Point(b.X + element.Position.X, b.Y + element.Position.Y);
					element.PaintingEntity.Line(startPosition, endPosition);
				}
			}
		}

		[TestMethod]
		[TestCategory("AutoBob")]
		public void PaintALine()
		{
			var canvas = new Canvas(new Size(1024, 768), System.Drawing.Color.White);
			var brushSize = new System.Drawing.Size(
				(int)Math.Round(0.6 * 43.6),
				(int)Math.Round(2 * 43.6));
			var brush = new Brush(canvas, brushSize);
			brush.Load(System.Drawing.Color.ForestGreen, 75);
			brush.Line(
				new System.Drawing.Point(50, 380),
				new System.Drawing.Point(900, 380));

			brush.Load(System.Drawing.Color.OrangeRed, 75);
			brush.Line(
				new System.Drawing.Point(50, 180),
				new System.Drawing.Point(900, 180));

			brush.Load(System.Drawing.Color.BlueViolet, 75);
			brush.Line(
				new System.Drawing.Point(50, 580),
				new System.Drawing.Point(900, 580));

			brush.Load(System.Drawing.Color.LightGoldenrodYellow, 10);
			brush.Line(
				new System.Drawing.Point(150, 50),
				new System.Drawing.Point(150, 700));

			brush.Line(
				new System.Drawing.Point(350, 50),
				new System.Drawing.Point(350, 700));

			brush.Line(
				new System.Drawing.Point(550, 50),
				new System.Drawing.Point(550, 700));

			brush.Line(
				new System.Drawing.Point(750, 50),
				new System.Drawing.Point(750, 700));

			canvas.Save("E:/Stuff/test.png");
		}
	}
}