using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Miq.Tests.Nursery
{
    [TestClass]
    public class RedditClient
    {

        [TestMethod]
        [Ignore]
        public void LogOn_Works() // XXX incomplete, it's not asserting anything
        {
            // /api/login/nonspinningrotator?user=nonspinningrotator&passwd=nueva.cosa&api_type=json

            var request = WebRequest.Create("http://www.reddit.com/api/login/nonspinningrotator?user=nonspinningrotator&passwd=nueva.cosa&rem=true&api_type=json") as HttpWebRequest;
            request.Method = "POST";
            request.UserAgent = "nonspinningrotator's Reddit Client v1.0";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.ContentLength = 0;

            var response = request.GetResponse() as HttpWebResponse;
            var responseContentStream = new StreamReader(response.GetResponseStream());
            var responseContent = responseContentStream.ReadToEnd();
            var loginCookie = response.Cookies;
        }

        // get list of subscribed reddits   /subreddits/mine/subscriber
        
        // no more than 1 request every 2 seconds
        // 1 request to a page every 30 seconds
        // set user agent with version
        // request many resources per hit, instead of 1 hit per resource

        // {"json": {"errors": [], "data": {"modhash": "dpeeclrxah7bdfb4b79fe03a5b77cb0e34317d276259475ee3", "cookie": "8042349,2013-11-03T15:03:21,c1f9dae3b064f1cde8fe5764e673a5b23cee480b"}}}

        //response.Headers["Set-Cookie"]
//"reddit_session=8042349%2C2013-11-03T15%3A03%3A21%2Cc1f9dae3b064f1cde8fe5764e673a5b23cee480b; Domain=reddit.com; Max-Age=762396997; Path=/; expires=Thu, 31-Dec-2037 23:59:59 GMT; HttpOnly"

        // response {"json": {"errors": [], "data": {"modhash": "dpeeclrxah7bdfb4b79fe03a5b77cb0e34317d276259475ee3", "cookie": "8042349,2013-11-03T15:03:21,c1f9dae3b064f1cde8fe5764e673a5b23cee480b"}}}

        [TestMethod]
        [Ignore]
        public void GetListOfSubscribedReddits() // XXX incomplete, it's not asserting anything
        {
            var request = WebRequest.Create("http://www.reddit.com/subreddits/mine/subscriber.json") as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = new CookieContainer();
            var sessionCookie = new Cookie(
                "reddit_session",
                "8042349%2C2013-11-03T15%3A03%3A21%2Cc1f9dae3b064f1cde8fe5764e673a5b23cee480b",
                "/",
                "reddit.com"
            );
            request.CookieContainer.Add(sessionCookie);

            var response = request.GetResponse() as HttpWebResponse;
            var responseContentStream = new StreamReader(response.GetResponseStream());
            var responseContent = responseContentStream.ReadToEnd();
        }

        [TestMethod]
        public void ParseSubredditsMineResults()
        {
            var list = JObject.Parse(RedditClientSampleResponses.subredditsMineSubscriber);

            Assert.AreEqual("Listing", (string)list["kind"]);

            var subreddit = list["data"]["children"][0];

            Assert.AreEqual("t5", subreddit["kind"]);

            var stuff = subreddit["data"];
            var properties = stuff.Children().OfType<JProperty>().Select(x => x.Name).ToArray();
        }
    }
}
