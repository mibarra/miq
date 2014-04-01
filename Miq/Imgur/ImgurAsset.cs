using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.Imgur
{
    public class ImgurAsset
    {
        public ImgurAsset(JObject jObject)
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

            Id = jObject.GetValueFromJObject<string>("id");
            Link = jObject.GetValueFromJObject<string>("link");
            Title = jObject.GetValueFromJObject<string>("title");
            Description = jObject.GetValueFromJObject<string>("description");
            DateTime = jObject.GetDateTimeValueFromJObject("datetime");
            AccountUrl = jObject.GetValueFromJObject<string>("account_url");
            Section = jObject.GetValueFromJObject<string>("section");
            Nsfw = jObject.GetValueFromJObject<bool>("nsfw");

            Voting = new Voting(jObject);
        }

        public string Id { get; private set; }

        public string Link { get; private set; }

        public string Title { get; set; }

        public string AccountUrl { get; set; }

        public string Description { get; set; }

        public DateTime DateTime { get; set; }

        public string Section { get; set; }

        public bool Nsfw { get; set; }

        public Voting Voting { get; private set; }
    }
}
