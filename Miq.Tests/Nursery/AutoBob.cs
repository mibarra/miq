using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

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

		[TestMethod]
		[TestCategory("AutoBob")]
		public void PaintADropInACanvas()
		{
			var canvas = new Canvas(new Size(1024, 768), System.Drawing.Color.White);
			var drop = new PaintDrop(new System.Drawing.Point(56, 128), System.Drawing.Color.FromArgb(6, 6, 98));
			var canvasPixelColor = canvas.Blend(drop);
			canvas.Save("E:/Stuff/test.png");
		}
	}
}