using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Miq.Imgur;

namespace Miq.Tests.Imgur
{
    [TestClass]
    public class RateLimitCreditsTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void Deserialize_WithValidData_ReturnsRateLimitCreditsObject()
        {
            string creditsJson = @"{
                                    ""UserLimit"":500, ""UserRemaining"":500, ""UserReset"":1396307769,
                                    ""ClientLimit"":12500, ""ClientRemaining"":12500}";
            JObject j = JObject.Parse(creditsJson);

            var actualCredits = new RateLimitCredits(j);

            Assert.AreEqual(500, actualCredits.UserLimit);
            Assert.AreEqual(500, actualCredits.UserRemaining);
            Assert.AreEqual(new DateTime(2014, 3, 31, 23, 16, 09), actualCredits.UserReset);
            Assert.AreEqual(12500, actualCredits.ClientLimit);
            Assert.AreEqual(12500, actualCredits.ClientRemaining);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_WithNull_ThrowsArgumentNullException()
        {
            new RateLimitCredits(null);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Deserialize_WithoutUserRemaining_ThrowsArgumentException()
        {
            var jObject = JObject.FromObject(new { UserReset = 1396306675L, ClientRemaining = 100 });

            new RateLimitCredits(jObject);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Deserialize_WithoutUserReset_ThrowsArgumentException()
        {
            var jObject = JObject.FromObject(new { UserRemaining = 100, ClientRemaining = 100 });

            new RateLimitCredits(jObject);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Deserialize_WithoutClientRemaining_ThrowsArgumentException()
        {
            var jObject = JObject.FromObject(new { UserRemaining = 100, UserReset = 1396306675 });

            new RateLimitCredits(jObject);
        }
    }
}
