using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miq.Imgur;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Miq.Tests.Imgur
{
	[TestClass]
	public class ImageTests
	{
		[TestMethod]
		[TestCategory("Unit")]
		public void Deserialize_WithValidData_ReturnsImageObject()
		{
			string imageJson = @"{""id"":""ARbGOjd"",""title"":""long text here"",""description"":null,
                           ""datetime"":1395954024,""type"":""image/jpeg"",""animated"":false,""width"":2048,
                           ""height"":1536,""size"":188484,""views"":243844,""bandwidth"":45960692496,
                           ""favorite"":false,""nsfw"":false,""section"":""funny"",
                            ""link"":""http:\/\/i.imgur.com\/ARbGOjd.jpg""}";
			var j = JObject.Parse(imageJson);

			var actualImage = new Image(j);

			Assert.AreEqual("ARbGOjd", actualImage.Id);
			Assert.AreEqual(@"http://i.imgur.com/ARbGOjd.jpg", actualImage.Link);
			Assert.AreEqual("long text here", actualImage.Title);
			Assert.AreEqual(null, actualImage.Description);
			Assert.AreEqual(new DateTime(2014, 3, 27, 21, 0, 24), actualImage.DateTime);
			Assert.AreEqual(new MediaTypeHeaderValue("image/jpeg"), actualImage.Type);
			Assert.AreEqual("funny", actualImage.Section);
			Assert.AreEqual(45960692496, actualImage.Bandwidth);
			Assert.AreEqual(2048, actualImage.Width);
			Assert.AreEqual(1536, actualImage.Height);
			Assert.AreEqual(188484, actualImage.Size);
			Assert.AreEqual(243844, actualImage.Voting.Views);
			Assert.AreEqual(false, actualImage.Animated);
			Assert.AreEqual(false, actualImage.Voting.Favorite);
			Assert.AreEqual(false, actualImage.Nsfw);

		}

		[TestMethod]
		[TestCategory("Unit")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Deserialize_WithNull_ThrowsArgumentNullException()
		{
			new Image(null);
		}

		[TestMethod]
		[TestCategory("Unit")]
		[ExpectedException(typeof(ArgumentException))]
		public void Deserialize_ObjectWithoutId_ThrowsArgumentException()
		{
			var token = new JObject();

			new Image(token);
		}


		[TestMethod]
		[TestCategory("Unit")]
		[ExpectedException(typeof(ArgumentException))]
		public void Deserialize_ObjectwithoutLink_ThrowsArgumentException()
		{
			var token = JObject.FromObject(new { id = "foo" });

			new Image(token);
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void ImageRetriever_CanGet_SmallThumbnailOfAnImage()
		{
			var retriever = new ImgurImageRetriever();
			using (Stream imageStream = retriever.RetrieveTiny("FAQOMA6"))
			{
				Assert.IsNotNull(imageStream);
			}
		}
	}
}
