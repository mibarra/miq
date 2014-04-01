using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Miq.Imgur
{
    public class Image : ImgurAsset
    {
        public Image(JObject jObject)
            : base(jObject)
        {
            Type = jObject.GetMediaTypeValueFromJObjecT("type");
            Animated = jObject.GetValueFromJObject<bool>("animated");
            Width = jObject.GetValueFromJObject<int>("width");
            Height = jObject.GetValueFromJObject<int>("height");
            Size = jObject.GetValueFromJObject<int>("size");
            Bandwidth = jObject.GetValueFromJObject<long>("bandwidth");
        }

        public MediaTypeHeaderValue Type { get; set; }

        public bool Animated { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Size { get; set; }

        public long Bandwidth { get; set; }
    }
}
