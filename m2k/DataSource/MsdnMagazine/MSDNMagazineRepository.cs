using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using HtmlAgilityPack;
using Magazine;
using System.ServiceModel.Syndication;

namespace MSDNMagzine
{
	public class MsdnMagazineRepository : MsMagazineRepository
	{
		private const string Url = "http://msdn.microsoft.com/magazine/rss/default.aspx?z=z&iss=1";

		public MsdnMagazineRepository(string outputfolder)
			: base(outputfolder)
		{
			Issue = string.Format("MSDN Magazine {0} {1}", DateTime.Now.Month, DateTime.Now.Year);
			OutPutFolder = Path.Combine(outputfolder, Utility.GetValidFileName(Issue));
		}

		public override IEnumerable<Article> GetArticles()
		{
			XmlReader feedSource = XmlReader.Create(Url);
			SyndicationFeed magazineRss = SyndicationFeed.Load(feedSource);
			IEnumerable<MsdnArticle> articles = magazineRss.Items.Select(item => MsdnArticle.FromSyndicationItem(item, OutPutFolder));
			return articles;
		}
	}

	public class MsdnArticle : Article
	{
		public MsdnArticle(string issueFolder, Uri articleUrl, string title, string category)
			: base(issueFolder, articleUrl, title, category)
		{ }

		public override string GetArticleContent(string rawHtml)
		{
			var doc = new HtmlDocument();
			doc.LoadHtml(rawHtml);
			var mainContent = doc.DocumentNode.SelectSingleNode("//div[@id='MainContent']");

			var html = string.Format(@"<HTML><BODY>{0}</BODY></HTML>", mainContent.OuterHtml);
			return html;
		}

		internal static MsdnArticle FromSyndicationItem(SyndicationItem item, string OutputFolder)
		{
			try
			{
				string title = item.Title.Text;
				Uri link = item.Links[0].Uri;

				string category = "Article";
				var categoryItem = item.Categories.FirstOrDefault();
				if (categoryItem != null)
				{
					category = categoryItem.Name;
				}

				var article = new MsdnArticle(OutputFolder, link, title, category);
				return article;
			}
			catch (Exception e)
			{
				throw;
			}

		}
	}
}
