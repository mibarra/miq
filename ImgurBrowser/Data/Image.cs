using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Miq.ImgurBrowser.Data
{
	public class Image
	{
		public BitmapFrame Bitmap { get; set; }

		public Image(Stream stream)
		{
			Bitmap = BitmapFrame.Create(stream);
		}
	}
}
