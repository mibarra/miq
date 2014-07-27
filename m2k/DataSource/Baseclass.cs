using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml;
using System.ServiceModel.Syndication;
using HtmlAgilityPack;

namespace miq.m2k
{
    public class MsMagazineRepository
    {
        public string Url { get; set; }
        public string Title { get; set; }

        public string OutputMobiFileName { get; private set; }
        public string OpfFileName { get; private set; }

        protected string Issue;
        protected string OutPutFolder;
        private string _tocFileName;
        private readonly string _outputfolder;

        public MsMagazineRepository(string outputfolder, string url, string title)
        {
            Url = url;
            Title = title;

            Issue = string.Format("{0} {1} {2}", Title, DateTime.Now.Month, DateTime.Now.Year);
            OutPutFolder = Path.Combine(outputfolder, Utility.GetValidFileName(Issue));


            _outputfolder = outputfolder;
        }

        public IEnumerable<Article> GetArticles()
        {
            XmlReader feedSource = XmlReader.Create(Url);
            SyndicationFeed magazineRss = SyndicationFeed.Load(feedSource);
            IEnumerable<Article> articles = magazineRss.Items.Select(item => Article.FromSyndicationItem(item, OutPutFolder));
            return articles;
        }

        public string Process()
        {
            Console.WriteLine(string.Format("Start download {0}", Issue));
            OutputMobiFileName = string.Format(@"{0}\{1}.mobi", OutPutFolder, Issue);
            OpfFileName = string.Format(@"{0}\{1}.opf", OutPutFolder, Issue);
            _tocFileName = string.Format(@"{0}\toc.html", OutPutFolder);

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
            Console.WriteLine("Downloaded {0} articles into {1}", articles.Count(), outputFolder);
            return OpfFileName;
        }
    }

    public class Article
    {
        public string OutFileName
        {
            get { return _outFileName; }
        }

        public string Category { get; private set; }
        public string Title { get; private set; }

        private readonly Uri _articleUri;
        private readonly string _issueFolder;
        private readonly string _outFileName;
        private readonly string _imgFolder;

        public Article(string issueFolder, Uri articleUrl, string title, string category)
        {
            _issueFolder = issueFolder;
            _articleUri = articleUrl;
            Title = title;
            Category = category;
            _imgFolder = Path.Combine(_issueFolder, Utility.GetValidFileName(Title));
            // kindlegen cannot parse filename with "+". Remove it from filename
            _outFileName = Path.Combine(_issueFolder, string.Format("{0}.html", Utility.GetValidFileName(Title.Replace("+", ""))));
        }

        public void SaveAsHtml()
        {
            Directory.CreateDirectory(_imgFolder);

            var content = new StringBuilder(GetArticleContent(Utility.GetHtml(_articleUri)));
            var images = Utility.GetAllImagesFromUrl(content.ToString());
            int i = 0;
            foreach (var image in images)
            {
                try
                {
                    var imagePath = Path.Combine(_imgFolder, i + Utility.GetImageExtension(image));
                    Utility.DownloadImage(image, imagePath);
                    content.Replace(image, imagePath);
                    i++;
                }
                catch (Exception)
                {
                    // don't die
                }
            }
            File.WriteAllText(OutFileName, content.ToString(), Encoding.UTF8);
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
            catch (Exception)
            {
                throw;
            }

        }

        public override string GetArticleContent(string rawHtml)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(rawHtml);
            var mainContent = doc.DocumentNode.SelectSingleNode("//div[@id='MainContent']");

            var html = string.Format(@"<HTML><BODY>{0}</BODY></HTML>", mainContent.OuterHtml);
            return html;
        }
    }
}
