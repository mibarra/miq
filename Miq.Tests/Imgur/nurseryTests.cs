﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Miq.Imgur;

namespace Miq.Tests.Imgur
{
	[TestClass]
	public class imgurClientTests
	{
		[TestMethod]
		[TestCategory("Integration")]
		public void CallCreditsPage()
		{
			var client = new ImgurClient();

			var credits = client.Credits();

			Assert.AreEqual(12500, credits.ClientLimit);
			Assert.AreEqual(500, credits.UserLimit);
			Assert.IsTrue(credits.ClientRemaining >= 0 && credits.ClientRemaining <= credits.ClientLimit);
			Assert.IsTrue(credits.UserRemaining >= 0 && credits.UserRemaining <= credits.UserRemaining);
			Assert.IsTrue(DateTime.Now < credits.UserReset);
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void CallGallery()
		{
			// XXX This 'test' doesn't test anything.

			// GOAL for today; WPF app showing thumbnails for the images on a grid.
			var client = new ImgurClient();

			var images = client.Gallery();
			var img = images.OfType<Image>().First();

			// Testing retrieving info for one image.
			var di = client.Image(img.Id);
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void MyTestMethod()
		{
			// Example from the asp.net page on the http client; interesting point is the ReadAsAsync<Product>();
			//			using (var client = new HttpClient())
			//{
			//	client.BaseAddress = new Uri("http://localhost:9000/");
			//	client.DefaultRequestHeaders.Accept.Clear();
			//	client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			//	// New code:
			//	HttpResponseMessage response = await client.GetAsync("api/products/1");
			//	if (response.IsSuccessStatusCode)
			//	{
			//		Product product = await response.Content.ReadAsAsync<Product>();
			//		Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
			//	}
			//}
		}


		/* TODO implement paging
		 *             // paging: for plural actions, query string parameters
			// parameters:
			//      page    number of page being requested
			//      perPage limit results per page
			// /galery does not support the perPage parameter
			// /album/{id}/images is not paged
			// sample: https://api.imgur.com/3/account/imgur/images/0.json?perPage=42&page=6

*/

		/* TODO implement Errors
		 When receiving any content from imgur check for errors:
		 check success flag in the response if false:
			  status has the error code
			  data.error; error message
			  data.request; requested url
			  data.method; requested method.
		 code
		 200  no error
		 400  bad parameter (missing or invalid) On uploads (bad image format or corrupt)
		 401  missing authentication
		 403  forbidden resource or rate limit has been reached
		 404  bad resource requested
		 429  rate limiting engaged
		 500  imgur is broken
		 Implement the fault trip trigger pattern
		 *         // Global Parameters
		// ?_fake_status={200,400,401,403,404,500}  make the api return this status code. for testing error handling code.
		*/

		/* TODO implement endpoint Account
		 * (interesting to explore particular users)*/

		// TODO implement endpoint Album
		// (will implement later, we want images first :) )

		/* TODO implement endpoint Comment 
		 * (will implement later, ...)
		 */

		/* TODO implement endpoint Image
		//                  image info => /image/{id} => image model */

		/* TODO implement endpoint Conversation
		 */

		/* TODO implement endpoint Notification 
		 */

		// TODO implement endpoint Memegen
		//

		/* TODO implement gallery album:
		Key	Format	Description
id	string	The ID for the image
title	string	The title of the album in the gallery
description	string	The description of the album in the gallery
datetime	integer	Time inserted into the gallery, epoch time
cover	string	The ID of the album cover image
cover_width	integer	The width, in pixels, of the album cover image
cover_height	integer	The height, in pixels, of the album cover image
account_url	string	The account username or null if it's anonymous.
privacy	string	The privacy level of the album, you can only view public if not logged in as album owner
layout	string	The view layout of the album.
views	integer	The number of image views
link	string	The URL link to the album
ups	integer	Upvotes for the image
downs	integer	Number of downvotes for the image
score	integer	Imgur popularity score
is_album	boolean	if it's an album or not
vote	string	The current user's vote on the album. null if not signed in or if the user hasn't voted on it.
images_count	integer	The total number of images in the album (only available when requesting the direct album)
images	Array of Images	An array of all the images in the album (only available when requesting the direct album)
		 * 
		 * example:
		 * {
	"data": {
		"id": "lDRB2",
		"title": "Imgur Office",
		"description": null,
		"datetime": 1357856292,
		"cover": "24nLu",
		"account_url": "Alan",
		"privacy": "public",
		"layout": "blog",
		"views": 13780,
		"link": "http://alanbox.imgur.com/a/lDRB2",
		"ups": 1602,
		"downs": 14,
		"score": 1917,
		"is_album": true,
		"vote": null,
		"images_count": 11,
		"images": [
			{
				"id": "24nLu",
				"title": null,
				"description": null,
				"datetime": 1357856352,
				"type": "image/jpeg",
				"animated": false,
				"width": 2592,
				"height": 1944,
				"size": 855658,
				"views": 135772,
				"bandwidth": 116174397976,
				"link": "http://i.imgur.com/24nLu.jpg"
			},
			{
				"id": "Ziz25",
				"title": null,
				"description": null,
				"datetime": 1357856394,
				"type": "image/jpeg",
				"animated": false,
				"width": 2592,
				"height": 1944,
				"size": 919391,
				"views": 135493,
				"bandwidth": 124571044763,
				"link": "http://i.imgur.com/Ziz25.jpg"
			},
			{
				"id": "9tzW6",
				"title": null,
				"description": null,
				"datetime": 1357856385,
				"type": "image/jpeg",
				"animated": false,
				"width": 2592,
				"height": 1944,
				"size": 655028,
				"views": 135063,
				"bandwidth": 88470046764,
				"link": "http://i.imgur.com/9tzW6.jpg"
			},
			{
				"id": "dFg5u",
				"title": null,
				"description": null,
				"datetime": 1357856378,
				"type": "image/jpeg",
				"animated": false,
				"width": 2592,
				"height": 1944,
				"size": 812738,
				"views": 134704,
				"bandwidth": 109479059552,
				"link": "http://i.imgur.com/dFg5u.jpg"
			},
			{
				"id": "oknLx",
				"title": null,
				"description": null,
				"datetime": 1357856338,
				"type": "image/jpeg",
				"animated": false,
				"width": 1749,
				"height": 2332,
				"size": 717324,
				"views": 32938,
				"bandwidth": 23627217912,
				"link": "http://i.imgur.com/oknLx.jpg"
			},
			{
				"id": "OL6tC",
				"title": null,
				"description": null,
				"datetime": 1357856321,
				"type": "image/jpeg",
				"animated": false,
				"width": 2592,
				"height": 1944,
				"size": 1443262,
				"views": 32346,
				"bandwidth": 46683752652,
				"link": "http://i.imgur.com/OL6tC.jpg"
			},
			{
				"id": "cJ9cm",
				"title": null,
				"description": null,
				"datetime": 1357856330,
				"type": "image/jpeg",
				"animated": false,
				"width": 2592,
				"height": 1944,
				"size": 544702,
				"views": 31829,
				"bandwidth": 17337319958,
				"link": "http://i.imgur.com/cJ9cm.jpg"
			},
			{
				"id": "7BtPN",
				"title": null,
				"description": null,
				"datetime": 1357856369,
				"type": "image/jpeg",
				"animated": false,
				"width": 2592,
				"height": 1944,
				"size": 844863,
				"views": 31257,
				"bandwidth": 26407882791,
				"link": "http://i.imgur.com/7BtPN.jpg"
			},
			{
				"id": "42ib8",
				"title": null,
				"description": null,
				"datetime": 1357856424,
				"type": "image/jpeg",
				"animated": false,
				"width": 2592,
				"height": 1944,
				"size": 905073,
				"views": 30945,
				"bandwidth": 28007483985,
				"link": "http://i.imgur.com/42ib8.jpg"
			},
			{
				"id": "BbwIx",
				"title": null,
				"description": null,
				"datetime": 1357856360,
				"type": "image/jpeg",
				"animated": false,
				"width": 1749,
				"height": 2332,
				"size": 662413,
				"views": 30107,
				"bandwidth": 19943268191,
				"link": "http://i.imgur.com/BbwIx.jpg"
			},
			{
				"id": "x7b91",
				"title": null,
				"description": null,
				"datetime": 1357856406,
				"type": "image/jpeg",
				"animated": false,
				"width": 1944,
				"height": 2592,
				"size": 618567,
				"views": 29259,
				"bandwidth": 18098651853,
				"link": "http://i.imgur.com/x7b91.jpg"
			}
		]
	},
	"success": true,
	"status": 200
}*/

		/* TODO implement data wrapper
		 * Responses have: { success: bool, status: code, data: {} }
		 */

		/* TODO implement download of Image thumbnails
There are 6 total thumbnails that an image can be resized to. Each one is accessable by appending a single character suffix to the end of the image id, and before the file extension. The thumbnails are:

Thumbnail Suffix	Thumbnail Name	    Thumbnail Size	Keeps Image Proportions
s	                Small Square	    90x90	        No
b	                Big Square	        160x160	        No
t	                Small Thumbnail	    160x160	        Yes
m	                Medium Thumbnail	320x320	        Yes
l	                Large Thumbnail	    640x640	        Yes
h	                Huge Thumbnail	    1024x1024	    Yes
For example, the image located at http://i.imgur.com/12345.jpg has the Medium Thumbnail located at http://i.imgur.com/12345m.jpg
		  */

		/* TODO implement rate limit on all responses
		 * 
		 * 
		 *         // TODO implement Rate limiting
		// ~12.5k requests per day (blocked for a month if hits the limit five times in a month)
		// remaining credit for application: response X-RateLimit-ClientRemaining header
		// remaining credit for user: X-RateLimit-UserLimit header in response
		//
		// header   desc
		//  X-RateLimit-UserLimit       total credits that can be allocated
		//      -UserRemaining          total credits available
		//      -UserReset              timestamp for credits reset
		//      -ClientLimit            total credits for the app per day
		//      -ClientRemaining        total credits available for the app in a day
		//
		//  or hit https://api.imgur.com/3/credits

X-RateLimit-ClientLimit: 12500
X-RateLimit-ClientRemaining: 12500
X-RateLimit-UserLimit: 500
X-RateLimit-UserRemaining: 499
X-RateLimit-UserReset: 1395976271
		*/
	}
}
