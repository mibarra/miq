using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.Imgur
{
    public class ImgurAssetCollection : IEnumerable<ImgurAsset>
    {
        public ImgurAssetCollection(JArray jArray)
        {
            Assets = new ImgurAsset[jArray.Count];
            for (int i = 0; i < jArray.Count; i++)
            {
                JObject obj = jArray[i] as JObject;
                if (obj == null)
                {
                    throw new ArgumentException("Expecting array element to be object", "jArray");
                }

                ImgurAsset asset;
                if ((bool)obj["is_album"])
                {
                    asset = new Album(obj);
                }
                else
                {
                    asset = new Image(obj);
                }

                Assets[i] = asset;
            }
        }

        public int Count
        {
            get
            {
                return Assets.Length;
            }
        }

        public ImgurAsset this[int index]
        {
            get { return Assets[index]; }
        }

        ImgurAsset[] Assets;

        public IEnumerator<ImgurAsset> GetEnumerator()
        {
            return Assets.AsEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Assets.GetEnumerator();
        }
    }
}
