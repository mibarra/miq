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

            var image = new Image(getValueFromJObject<string>(jObject, "id"), getValueFromJObject<string>(jObject, "link"));

            image.Title = getValueFromJObject<string>(jObject, "title");
            image.Description = getValueFromJObject<string>(jObject, "description");
            image.DateTime = getDateTimeValueFromJObject(jObject, "datetime");
            image.Type = getMediaTypeValueFromJObjecT(jObject, "type");
            image.Section = getValueFromJObject<string>(jObject, "section");
            image.Bandwidth = getValueFromJObject<long>(jObject, "bandwidth");
            image.Width = getValueFromJObject<int>(jObject, "width");
            image.Height = getValueFromJObject<int>(jObject, "height");
            image.Size = getValueFromJObject<int>(jObject, "size");
            image.Views = getValueFromJObject<int>(jObject, "views");
            image.Animated = getValueFromJObject<bool>(jObject, "animated");
            image.Favorite = getValueFromJObject<bool>(jObject, "favorite");
            image.Nsfw = getValueFromJObject<bool>(jObject, "nsfw");

            return image;
        }

        private static MediaTypeHeaderValue getMediaTypeValueFromJObjecT(JObject jObject, string propertyName)
        {
            string type = getValueFromJObject<string>(jObject, propertyName);
            return new MediaTypeHeaderValue(type);
        }

        private static DateTime getDateTimeValueFromJObject(JObject jObject, string propertyName)
        {
            long timestamp = getValueFromJObject<long>(jObject, propertyName);
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(timestamp);
        }

        private static TReturn getValueFromJObject<TReturn>(JObject jObject, string propertyName)
        {
            var jTitleProperty = jObject.Property(propertyName);
            TReturn valueToAssign = default(TReturn);
            JTokenType type = GetJTokenTypeFor(typeof(TReturn));
            if (jTitleProperty != null && jTitleProperty.Value.Type == type)
            {
                valueToAssign = jTitleProperty.Value.Value<TReturn>();
            }
            return valueToAssign;

        }

        private static JTokenType GetJTokenTypeFor(Type type)
        {
            switch (type.Name)
            {
                case "String": return JTokenType.String;
                case "Int64": return JTokenType.Integer;
                case "Int32": return JTokenType.Integer;
                case "Boolean": return JTokenType.Boolean;
            }

            throw new NotImplementedException(type.Name + " not implemented yet.");
        }
    }

}
