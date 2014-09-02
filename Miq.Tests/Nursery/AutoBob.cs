using System;
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
				get { return paintAmount; }
				private set { paintAmount = Math.Max(0, Math.Min(100, value)); }
			}
			public System.Drawing.Color PaintColor
			{
				get
				{
					return paintColor;
/*					double maxAdjustment = (Rng.NextDouble() - 0.5) * 20;
					double adjustmentFor3dEffect = 1 + (maxAdjustment * (PaintAmount - 80) / 20);
					return System.Drawing.Color.FromArgb(
						(int)Math.Round(paintColor.R * adjustmentFor3dEffect),
						(int)Math.Round(paintColor.G * adjustmentFor3dEffect),
						(int)Math.Round(paintColor.B * adjustmentFor3dEffect));*/
				}
				private set
				{
					paintColor = value;
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
				PaintAmount = paintAmount;
				PaintColor = paintColor;
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
				PaintColor = BlendColors(paintColor, colorOnCanvas, blendingControl);
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

			System.Drawing.Color paintColor;
			private double paintAmount;
			private Random Rng;
			private double BlendingAdjustment;
		}

		enum PaintingEntityState
		{
			WetLoaded,
			DryWithPaint,
			DryWithNoPaint
		}

		class Brush
		{
			// Attack plan:
			// Draw with a brush
			//	can load paint in a brush
			//		can create PEs in a brush
			//			calc grid bounding rectangle <= rectangle with size of ellipse
			//			points grid <= 
			//			randomize grid
			//			pick points inside ellipse

			IEnumerable<System.Drawing.Point> GridPoints(int majorAxis, int minorAxis)
			{
				for (int x = 0; x < minorAxis; x+=3)
				{
					for (int y = 0; y < majorAxis; y+=2)
					{
						var point = new System.Drawing.Point(x, y);
						yield return point;
					}
				}
			}

			public IEnumerable<System.Drawing.Point> PaintingElementsPoints(int majorAxis, int minorAxis)
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

			//	can send line stroke to a brush
			//		can use rotation
			//		can use pressure

			// Manages PEs

			// Create brush by:
			// placing PEs together to form shape of bursh footprint

			// To reset PEs;  Select locations each time brush is about to stroke
			// Each brush type as its own way to place PEs
			//	1&2 inch brush
			//			have an elipse;		major axis		minor axis
			//				1 inch bursh	1 inch			0.55 inches
			//				2 inch brush	2 inch			0.6 inches
			//		make a grid 2x1 pixels appart (x and y direction)
			//		put a point at each grid location
			//		randonmize each point by +-1 pixel both locations
			//		test if point is inside the ellipse, if so, create a PE there.
			//		[should keep about 300 PE for the 2inch brush]
			//		[and around 135 PEs for the 1inch brush]
			//	round brush
			//		same as 1&2 inch brush but bounding ellipse is 1x0.8 inches.
			//	fan brush
			//	fan brush corner
			//	filbert Brush
			//	liner brush (One PE), start with this one?
			//	palette knife
			//		No paint blending when has paint, but enable is doesnt have paint
			//		No 3d effect
			//		Disable painting flag
			//		Scratch mode
			//

			// Brush Orientation
			//	angle in degrees
			//	top and clockwise 0, 90, 180, 270
			//	chaging the angle should rotate the PEs relative position
			//	PEs don't store position, Brush should keep track of the relative position of PE
			//  Brush uses that to calculate final positions when doing line or stab in PE.

			// Paint Loading
			//	LoadPaint(color, amount) => foreach PE; PE.loadpaint(color, amount +- random 5%)

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
		}

		[TestMethod]
		[TestCategory("AutoBob")]
		public void PaintALine()
		{
			var canvas = new Canvas(new Size(1024, 768), System.Drawing.Color.White);
			var pe = new PaintingEntity(canvas, 0.0);
			pe.Load(System.Drawing.Color.Black, 100);
/*			pe.Stab(new System.Drawing.Point(56, 128));
			pe.Stab(new System.Drawing.Point(60, 128));
			pe.Stab(new System.Drawing.Point(64, 128));
			pe.Stab(new System.Drawing.Point(56, 132));
			pe.Stab(new System.Drawing.Point(60, 132));
			pe.Stab(new System.Drawing.Point(64, 132));
			pe.Stab(new System.Drawing.Point(56, 136));
			pe.Stab(new System.Drawing.Point(60, 136));
			pe.Stab(new System.Drawing.Point(64, 136));
			pe.Line(new System.Drawing.Point(70, 200), new System.Drawing.Point(800, 300));
			pe.Line(new System.Drawing.Point(70, 202), new System.Drawing.Point(800, 302));*/

			var b = new Brush();
			var x = b.PaintingElementsPoints((int)Math.Round(2 * 43.6), (int)Math.Round(0.6 * 43.6)).ToArray();
			foreach (var point in x)
			{
				pe.Stab(point);
			}

			canvas.Save("E:/Stuff/test.png");
		}
	}
}