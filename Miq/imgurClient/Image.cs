using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Miq.imgurClient
{
    public class Image
    {
        public Image(string id, string link)
        {
            Id = id;
            Link = link;
        }

        public string Id { get; private set; }

        public string Link { get; private set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DateTime { get; set; }

        public MediaTypeHeaderValue Type { get; set; }

        public string Section { get; set; }

        public long Bandwidth { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Size { get; set; }

        public int Views { get; set; }

        public bool Animated { get; set; }

        public bool Favorite { get; set; }

        public bool Nsfw { get; set; }

        public static Image Deserialize(JObject jObject)
        {
            if (jObject == null)
            {
                throw new ArgumentNullException("jToken");
            }
            if (jObject.Property("id") == null)
            {
                throw new ArgumentException("id property missing in jObject", "jObject");
            }
            if (jObject.Property("link") == null)
            {
                throw new ArgumentException("link property missing in jObject", "jObject");
            }

            var image = new Image(jObject.getValueFromJObject<string>("id"), jObject.getValueFromJObject<string>("link"));

            image.Title = jObject.getValueFromJObject<string>("title");
            image.Description = jObject.getValueFromJObject<string>("description");
            image.DateTime = jObject.getDateTimeValueFromJObject("datetime");
            image.Type = jObject.getMediaTypeValueFromJObjecT("type");
            image.Section = jObject.getValueFromJObject<string>("section");
            image.Bandwidth = jObject.getValueFromJObject<long>("bandwidth");
            image.Width = jObject.getValueFromJObject<int>("width");
            image.Height = jObject.getValueFromJObject<int>("height");
            image.Size = jObject.getValueFromJObject<int>("size");
            image.Views = jObject.getValueFromJObject<int>("views");
            image.Animated = jObject.getValueFromJObject<bool>("animated");
            image.Favorite = jObject.getValueFromJObject<bool>("favorite");
            image.Nsfw = jObject.getValueFromJObject<bool>("nsfw");

            return image;
        }
    }
}
