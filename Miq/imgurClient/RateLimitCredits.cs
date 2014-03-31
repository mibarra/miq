using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.imgurClient
{
    public class RateLimitCredits
    {
        public RateLimitCredits(int clientRemaining, int userRemaining, DateTime userReset)
        {
            ClientRemaining = clientRemaining;
            UserRemaining = userRemaining;
            UserReset = userReset;
        }

        public int UserLimit { get; set; }

        public int UserRemaining { get; private set; }
        
        public DateTime UserReset { get; private set; }
        
        public int ClientLimit { get; set; }
        
        public int ClientRemaining { get; private set; }

        public static RateLimitCredits Deserialize(JObject jObject)
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

            var rateLimitCredits = new RateLimitCredits(
                jObject.getValueFromJObject<int>("ClientRemaining"),
                jObject.getValueFromJObject<int>("UserRemaining"),
                jObject.getDateTimeValueFromJObject("UserReset"));

            rateLimitCredits.ClientLimit = jObject.getValueFromJObject<int>("ClientLimit");
            rateLimitCredits.UserLimit = jObject.getValueFromJObject<int>("UserLimit");

            return rateLimitCredits;
        }
    }
}
