using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.Imgur
{
    public class Album : ImgurAsset
    {
        public Album(JObject jObject)
            : base(jObject)
        {
            // Album: is_album(true):  cover, cover_width, cover_height, privacy, layout
        }
    }
}
