using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Miq.Imgur
{
	public class ImgurImageRetriever
	{
		public ImgurImageRetriever()
		{
			HttpClient = new HttpClient();
			HttpClient.BaseAddress = new Uri("http://i.imgur.com/");
		}

		public Stream RetrieveTiny(string id)
		{
			string url = string.Format("{0}s.png", id);
			var response = HttpClient.GetAsync(url);
			if (!response.Result.IsSuccessStatusCode)
			{
				throw new NotImplementedException("Error handling is still missing");
			}

			Stream stream = response.Result.Content.ReadAsStreamAsync().Result;
			return stream;
		}

		HttpClient HttpClient;
	}
}
