using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miq.imgurClient;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace Miq.Tests.imgurClient
{
    [TestClass]
    public class ImageTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void Deserialize_WithValidData_ReturnsImageObject()
        {
            Image expectedImage = new Image(id: "ARbGOjd", link: @"http://i.imgur.com/ARbGOjd.jpg");
            expectedImage.Title = "long text here";
            expectedImage.Description = null;
            expectedImage.DateTime = new DateTime(2014, 3, 27, 21, 0, 24);
            expectedImage.Type = new MediaTypeHeaderValue("image/jpeg");
            expectedImage.Section = "funny";
            expectedImage.Bandwidth = 45960692496;
            expectedImage.Width = 2048;
            expectedImage.Height = 1536;
            expectedImage.Size = 188484;
            expectedImage.Views = 243844;
            expectedImage.Animated = false;
            expectedImage.Favorite = false;
            expectedImage.Nsfw = false;
            string imageJson = @"{""id"":""ARbGOjd"",""title"":""long text here"",""description"":null,
                           ""datetime"":1395954024,""type"":""image/jpeg"",""animated"":false,""width"":2048,
                           ""height"":1536,""size"":188484,""views"":243844,""bandwidth"":45960692496,
                           ""favorite"":false,""nsfw"":false,""section"":""funny"",
                            ""link"":""http:\/\/i.imgur.com\/ARbGOjd.jpg""}";
            var j = JObject.Parse(imageJson);

            Image actualImage = Image.Deserialize(j);

            Assert.AreEqual(expectedImage.Id, actualImage.Id);
            Assert.AreEqual(expectedImage.Link, actualImage.Link);
            Assert.AreEqual(expectedImage.Title, actualImage.Title);
            Assert.AreEqual(expectedImage.Description, actualImage.Description);
            Assert.AreEqual(expectedImage.DateTime, actualImage.DateTime);
            Assert.AreEqual(expectedImage.Type, actualImage.Type);
            Assert.AreEqual(expectedImage.Section, actualImage.Section);
            Assert.AreEqual(expectedImage.Bandwidth, actualImage.Bandwidth);
            Assert.AreEqual(expectedImage.Width, actualImage.Width);
            Assert.AreEqual(expectedImage.Height, actualImage.Height);
            Assert.AreEqual(expectedImage.Size, actualImage.Size);
            Assert.AreEqual(expectedImage.Views, actualImage.Views);
            Assert.AreEqual(expectedImage.Animated, actualImage.Animated);
            Assert.AreEqual(expectedImage.Favorite, actualImage.Favorite);
            Assert.AreEqual(expectedImage.Nsfw, actualImage.Nsfw);

        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_WithNull_ThrowsArgumentNullException()
        {
            Image.Deserialize(null);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Deserialize_ObjectWithoutId_ThrowsArgumentException()
        {
            var token = new JObject();

            Image.Deserialize(token);
        }


        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Deserialize_ObjectwithoutLink_ThrowsArgumentException()
        {
            var token = new JObject(new { id = "foo" });

            Image.Deserialize(token);
        }
    }
}
