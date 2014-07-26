using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screenshotter
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;

            var timer = new System.Threading.Timer(
                e =>
                {
                    string filename = i.ToString("D4");
                    TakeScreenshot(filename);
                    i++;
                },
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(3));
            Console.ReadLine();
        }

        private static void TakeScreenshot(string filename)
        {
            string directoryName = @"C:\Users\Miguel\AppData\Local\Temp\Screenshots\";
            string extension = ".png";
            string path = directoryName + filename + extension;
            System.Drawing.Size size = GetScreenSize();
            using (var bmp = new System.Drawing.Bitmap(size.Width, size.Height))
            using (var gr = System.Drawing.Graphics.FromImage(bmp))
            {
                gr.CopyFromScreen(0, 0, 0, 0, size);
                bmp.Save(path);
            }
        }

        private static System.Drawing.Size GetScreenSize()
        {
            var size = new System.Drawing.Size(
                (int)System.Windows.SystemParameters.FullPrimaryScreenWidth,
                (int)System.Windows.SystemParameters.FullPrimaryScreenHeight);
            return size;
        }
    }
}
