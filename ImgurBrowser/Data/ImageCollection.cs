using Miq.Imgur;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.ImgurBrowser.Data
{
	public class ImageCollection : ObservableCollection<Image>
	{
		public ImageCollection()
		{
			imgur = new ImgurClient();
			retriever = new ImgurImageRetriever();

			ImgurAssetCollection gallery = imgur.Gallery();
			var someIds = new string[] {
				"D0edloJ", "rKoQ8gl", "a0ZYz8y", "K7AUWUh", "UVYlaAT", "mS3K3OI", "vgg0lT9", "Zs70Gvk",
				"R3eB3W2", "dr8d1VU", "neKar7M", "cvj6oCU", "MHYCmPT", "xxVnqZp", "cuG9r4N", "xMDWu3e",
				"8aTrOwH", "cyE6Hx1", "mZUiCKz", "qgdHkpX", "30o0LLs", "UBMyEx0", "mF0FsHQ", "y3wbzOx",
				"LA0L9NG", "p3JDGYf", "39c8gzp", "3Z1cYT2", "ooMJAQn", "2XlB6g6", "qzpreEY", "NtSJUpR",
				"8NwN6AN", "0wgQiSU", "lJSMsmN", "YAeFK5z", "uwSqAi6", "I5Pfmr6", "gFFm56v", "nPEcN1P",
				"zkXguhk", "YX9htaM", "GktQi3b", "4mM407G", "T1dEDNp", "LB5IXVq", "KRQ4hOo", "ABPKiQz",
				"5Xzp6Dh", "ZyRRIZj", "USlLAWt", "1j1Wd3n", "PpAWkKQ", "91F0KBr", "KVZpgAP", "JgY7zkU",
				"uRcW3cX", "kaRKQGF", "0uTQPpb", "Nvu3LwB", "j0vVavW", "52xqnAd", "orJOgVu", "zwGD8wT",
				"Kxa8q6t", "b7Yzu0c", "euMIrMH", "MAo9i0v", "UuraoWd", "JV8bbhj", "7Z9WN4w", "Vll3oc2",
				"fEtn2bm", "yfn56YE", "vJQGhHv", "cnLCpL7", "dex8CiF", "SxWrnal", "BRcVvP9", "xWopjvF",
				"gdAC00e", "kJDFKzR", "ohnlAzj", "S0olyRr", "IpHTBXK", "Piev31X", "K72JuQB", "yIWJ8xi",
				"f8K1IDi", "FKDuQIy", "BNc0Tmh", "NHA3bba", "kb1RFwe", "M8rlajl", "gSZjp5C", "OQM0SWE",
				"PjHowAn", "1T67otz", "HhcxdU0", "kB47VSt",
			};

			/*			foreach (var image in gallery.OfType<Miq.Imgur.Image>().Take(10))
						{
							Add(new Image(retriever.RetrieveTiny(image.Id)));
						}*/
		}

		ImgurClient imgur;
		ImgurImageRetriever retriever;
	}
}
