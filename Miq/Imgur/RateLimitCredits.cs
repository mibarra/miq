using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.Imgur
{
    public class RateLimitCredits
    {
        public RateLimitCredits(JObject jObject)
        {
            if (jObject == null)
            {
                throw new ArgumentNullException("jObject");
            }
            if (jObject.Property("ClientRemaining") == null)
            {
                throw new ArgumentException("Expecting ClientRemaining property", "jObject");
            }
            if (jObject.Property("UserRemaining") == null)
            {
                throw new ArgumentException("Expecting UserRemaining property", "jObject");
            }
            if (jObject.Property("UserReset") == null)
            {
                throw new ArgumentException("Expecting UserReset property", "jObject");
            }

            ClientRemaining = jObject.GetValueFromJObject<int>("ClientRemaining");
            UserRemaining = jObject.GetValueFromJObject<int>("UserRemaining");
            UserReset = jObject.GetDateTimeValueFromJObject("UserReset");

            ClientLimit = jObject.GetValueFromJObject<int>("ClientLimit");
            UserLimit = jObject.GetValueFromJObject<int>("UserLimit");
        }

        public int UserLimit { get; set; }

        public int UserRemaining { get; private set; }

        public DateTime UserReset { get; private set; }

        public int ClientLimit { get; set; }

        public int ClientRemaining { get; private set; }
    }
}
