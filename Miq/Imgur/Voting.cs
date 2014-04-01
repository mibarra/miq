using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.Imgur
{
    public class Voting
    {
        public Voting(JObject jObject)
        {
            if (jObject == null)
            {
                throw new ArgumentNullException("jToken");
            }

            Views = jObject.GetValueFromJObject<int>("views");
            Favorite = jObject.GetValueFromJObject<bool>("favorite");
            Vote = jObject.GetValueFromJObject<string>("Vote");
            Ups = jObject.GetValueFromJObject<int>("ups");
            Downs = jObject.GetValueFromJObject<int>("downs");
            Score = jObject.GetValueFromJObject<int>("score");
        }

        public int Views { get; set; }
        public bool Favorite { get; set; }
        public string Vote { get; set; }
        public int Ups { get; set; }
        public int Downs { get; set; }
        public int Score { get; set; }
    }
}
