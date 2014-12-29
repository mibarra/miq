using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Miq.Tests.Nursery;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Miq.Shell
{
	class Program
	{
		static void Main()
		{
			try
			{
				new Youtube().Run().Wait();
			}
			catch (AggregateException ex)
			{
				foreach (var e in ex.InnerExceptions)
				{
					Console.WriteLine("Error: " + e.Message);
				}
			}
		}

		class Youtube
		{
			public async Task Run()
			{
				UserCredential credential;
				using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
				{
					credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
						GoogleClientSecrets.Load(stream).Secrets,
						// This OAuth 2.0 access scope allows for full read/write access to the
						// authenticated user's account.
						new[] { YouTubeService.Scope.Youtube },
						"mibarra",
						CancellationToken.None,
						new FileDataStore(this.GetType().ToString())
					);
				}

				var youtubeService = new YouTubeService(new BaseClientService.Initializer()
				{
					HttpClientInitializer = credential,
					ApplicationName = this.GetType().ToString(),

				});

				//var channelsListRequest = youtubeService.Channels.List("contentDetails");
				//channelsListRequest.Mine = true;
				//// Retrieve the contentDetails part of the channel resource for the authenticated user's channel.
				//var channelsListResponse = await channelsListRequest.ExecuteAsync();

				//foreach (var channel in channelsListResponse.Items)
				//{
				//	// From the API response, extract the playlist ID that identifies the list
				//	// of videos uploaded to the authenticated user's channel.
				//	var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;
				//	Console.WriteLine("Videos in list {0}", uploadsListId);
				//	var nextPageToken = "";
				//	while (nextPageToken != null)
				//	{
				//		var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
				//		playlistItemsListRequest.PlaylistId = uploadsListId;
				//		playlistItemsListRequest.MaxResults = 50;
				//		playlistItemsListRequest.PageToken = nextPageToken;
				//		// Retrieve the list of videos uploaded to the authenticated user's channel.
				//		var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();
				//		foreach (var playlistItem in playlistItemsListResponse.Items)
				//		{
				//			// Print information about each video.
				//			Console.WriteLine("{0} ({1})", playlistItem.Snippet.Title, playlistItem.Snippet.ResourceId.VideoId);
				//		}
				//		nextPageToken = playlistItemsListResponse.NextPageToken;
				//	}
				//}


				var searchListRequest = youtubeService.Search.List("snippet");
				searchListRequest.Q = "Sagan"; // Replace with your search term.
				searchListRequest.MaxResults = 50;
				// Call the search.list method to retrieve results matching the specified query term.
				var searchListResponse = await searchListRequest.ExecuteAsync();
				List<string> videos = new List<string>();
				List<string> channels = new List<string>();
				List<string> playlists = new List<string>();
				// Add each result to the appropriate list, and then display the lists of
				// matching videos, channels, and playlists.
				foreach (var searchResult in searchListResponse.Items)
				{
					switch (searchResult.Id.Kind)
					{
						case "youtube#video":
							videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
							break;
						case "youtube#channel":
							channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
							break;
						case "youtube#playlist":
							playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
							break;
					}
				}

				Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
				Console.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
				Console.WriteLine(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists)));
			}
		}

		//		static void Main(string[] args)
		//		{
		//			Individual.Goal = @"
		//To be, or not to be, that is the question:
		//Whether 'tis Nobler in the mind to suffer
		//The Slings and Arrows of outrageous Fortune,
		//Or to take Arms against a Sea of troubles,
		//And by opposing end them: to die, to sleep
		//No more; and by a sleep, to say we end
		//The Heart-ache, and the thousand Natural shocks
		//That Flesh is heir to? 'Tis a consummation
		//";
		//			var ga = new GeneticAlgorithm(5000);
		//			for (int i = 0; i < 100000000; i++)
		//			{
		//				ga.Step();
		//				ga.PrintStatistics();
		//				if (ga.Population.BestIndividual.Fitness == Individual.Goal.Length)
		//					break;
		//			}

		//			Console.ReadLine();
		//		}
	}
}
