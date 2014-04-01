using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

using Miq.Imgur;

namespace Miq.Tests.Imgur
{
    [TestClass]
    public class ImgurAssetCollectionTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void Constructor_BuildsTheCollection()
        {
            var data = @"[{ ""id"": ""VOKNC"", ""title"": ""Russian Photographer Takes Stunning Portraits With Real Animals"", ""description"": null, ""datetime"": 1396291739, ""cover"": ""ilaHTiP"", ""cover_width"": 880, ""cover_height"": 632,
                            ""account_url"": null, ""privacy"": ""public"", ""layout"": ""blog"", ""views"": 77958, ""link"": ""http:\/\/imgur.com\/a\/VOKNC"", ""ups"": 3415, ""downs"": 47, ""score"": 3670, ""is_album"": true, ""vote"": null,
                            ""favorite"": false, ""nsfw"": false, ""section"": ""pics"" }, { ""id"": ""r0hbM9I"", ""title"": ""Boss said i can go home, but i need to peel these 100 apples first"", ""description"": null, ""datetime"": 1396273957,
                            ""type"": ""image\/gif"", ""animated"": true, ""width"": 160, ""height"": 205, ""size"": 3109455, ""views"": 1028023, ""bandwidth"": 3196591257465, ""vote"": null, ""favorite"": false, ""nsfw"": false,
                            ""section"": ""gifs"", ""account_url"": ""zipperke"", ""link"": ""http:\/\/i.imgur.com\/r0hbM9I.gif"", ""ups"": 8420, ""downs"": 38, ""score"": 9253, ""is_album"": false }]";
            var j = JArray.Parse(data);

            ImgurAssetCollection results = new ImgurAssetCollection(j);

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(typeof(Album), results[0].GetType());
            Assert.AreEqual(typeof(Image), results[1].GetType());
        }
    }
}
