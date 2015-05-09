using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml;
using System.ServiceModel.Syndication;
using HtmlAgilityPack;
using System.Resources;
using System.Globalization;

namespace Miq.M2K
{
    internal class MsMagazineRepository
    {
        internal string Url { get; set; }
        internal string Title { get; set; }

        internal string OutputMobiFileName { get; private set; }
        internal string OpfFileName { get; private set; }

        protected string Issue;
        protected string OutPutFolder;
        private string _tocFileName;
        private readonly string _outputfolder;

        internal MsMagazineRepository(string outputfolder, string url, string title)
        {
            Url = url;
            Title = title;

            Issue = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", Title, DateTime.Now.Month, DateTime.Now.Year);
            OutPutFolder = Path.Combine(outputfolder, Utility.GetValidFileName(Issue));


            _outputfolder = outputfolder;
        }

        internal IEnumerable<Article> GetArticles()
        {
            using (XmlReader feedSource = XmlReader.Create(Url))
            {
                SyndicationFeed magazineRss = SyndicationFeed.Load(feedSource);
                IEnumerable<Article> articles = magazineRss.Items.Select(item => Article.FromSyndicationItem(item, OutPutFolder));
                return articles;
            }
        }

        internal string Process()
        {
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, Properties.Resources.StartDownload, Issue));
            OutputMobiFileName = Path.Combine(OutPutFolder, Issue + ".mobi");
            OpfFileName = Path.Combine(OutPutFolder, Issue + ".opf");
            _tocFileName = Path.Combine(OutPutFolder, "toc.html");

            // get articles list
            var articles = GetArticles().ToList();
            // download each article
            foreach (var article in articles)
                article.SaveAsHtml();
            // generate mobi
            var outputFolder = Path.Combine(_outputfolder, Issue);
            var toc = Utility.CreateTableOfContent(articles);
            File.WriteAllText(_tocFileName, toc, Encoding.UTF8);
            var odf = Utility.CreateOpf(articles, Issue);
            File.WriteAllText(OpfFileName, odf, Encoding.UTF8);
            Console.WriteLine(Properties.Resources.ArticlesDownloaded, articles.Count(), outputFolder);
            return OpfFileName;
        }
    }

    internal class Article
    {
        internal string OutFileName
        {
            get { return _outFileName; }
        }

        internal string Category { get; private set; }
        internal string Title { get; private set; }

        private readonly Uri _articleUri;
        private readonly string _issueFolder;
        private readonly string _outFileName;
        private readonly string _imgFolder;

        internal Article(string issueFolder, Uri articleUrl, string title, string category)
        {
            _issueFolder = issueFolder;
            _articleUri = articleUrl;
            Title = title;
            Category = category;
            _imgFolder = Path.Combine(_issueFolder, Utility.GetValidFileName(Title));
            // kindlegen cannot parse filename with "+". Remove it from filename
            _outFileName = Path.Combine(_issueFolder, Utility.GetValidFileName(Title.Replace("+", "")) + ".html");
        }

        internal void SaveAsHtml()
        {
            Directory.CreateDirectory(_imgFolder);

            var content = new StringBuilder(GetArticleContent(Utility.GetHtml(_articleUri)));
            var images = Utility.GetAllImagesFromUrl(content.ToString());
            int i = 0;
            foreach (var image in images)
            {
                var imagePath = Path.Combine(_imgFolder, i + Utility.GetImageExtension(image));
                Utility.DownloadImage(image, imagePath);
                content.Replace(image, imagePath);
                i++;
            }
            File.WriteAllText(OutFileName, content.ToString(), Encoding.UTF8);
        }


        internal static Article FromSyndicationItem(SyndicationItem item, string OutputFolder)
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

                var article = new Article(OutputFolder, link, title, category);
                return article;
            }
            catch (Exception)
            {
                throw;
            }

        }

        internal static string GetArticleContent(string rawHtml)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(rawHtml);
            var mainContent = doc.DocumentNode.SelectSingleNode("//div[@id='MainContent']");

            var html = string.Format(CultureInfo.InvariantCulture, @"<HTML><BODY>{0}</BODY></HTML>", mainContent.OuterHtml);
            return html;
        }
    }
}
