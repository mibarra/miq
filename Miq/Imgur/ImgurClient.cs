using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Miq.Imgur
{
    public class ImgurClient : IDisposable
    {
        public ImgurClient()
        {
            HttpClient = new HttpClient();
            HttpClient.BaseAddress = new Uri(ApiEndpoint);
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            // XXX ??? MediaTypeWithQualityHeaderValue? or is MediaTypeHeaderValue enough?
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", ClientId);
        }

        public RateLimitCredits Credits()
        {
            return new RateLimitCredits(HitService("credits.json"));
        }

        public Image Image(string id)
        {
            return new Image(HitService(string.Format("image/{0}.json", id)));
        }

        public ImgurAssetCollection Gallery()
        {
            // ZZZ fix arguments: section {hot,top,user}
            // ZZZ fix arguments: sort {viral,time}
            // ZZZ fix arguments: page (int >= 0)
            // ZZZ fix arguments: window {day,week,month,year} if section is top
            // ZZZ fix arguments: showViral (bool)
            return new ImgurAssetCollection(HitService<JArray>("gallery/hot/viral/0.json"));
        }

        HttpClient HttpClient;
        string ClientId = "cc7a67f84a31063";
        //string ClientSecret = "c631b56cd31629d3162471b41edcd51509f8cc61";
        string ApiEndpoint = "https://api.imgur.com/3/";

        private JObject HitService(string url)
        {
            return HitService<JObject>(url);
        }

        private TReturn HitService<TReturn>(string url)
            where TReturn : JToken
        {
            // TODO XXX requested url must NOT begin with '/'
            var response = /* await */ HttpClient.GetAsync(url);
            if (!response.Result.IsSuccessStatusCode)
            {
                throw new NotImplementedException("Error handling is still missing");
            }

            var responseContent = response.Result.Content;

            // TODO header cache
            // TODO response.Result.Headers.CacheControl
            // TODO response.Result.Headers.Date 

            var responseString = /* await */ responseContent.ReadAsStringAsync().Result;

            // TODO before doing this, check content-type header == application/json
            // response.Result.content.Headers.ContentType
            var j = JObject.Parse(responseString);

            bool success = (bool)j["success"];
            if (!success)
            {
                throw new NotImplementedException("Error handling is still missing");
            }

            // int status = (int)j["status"];
            // TODO check if status is 200
            return (TReturn)j["data"];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ImgurClient()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (HttpClient != null)
                {
                    HttpClient.Dispose();
                }
            }
        }
    }
}
