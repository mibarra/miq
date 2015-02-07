using Miq.Imgur;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.ImgurBrowser.Data
{
    public class ImageCollection : ObservableCollection<Image>, IDisposable
    {
        public ImageCollection()
        {
            Imgur = new ImgurClient();
            Retriever = new ImgurImageRetriever();
        }

        public Image GetNextImage()
        {
            if (GalleryImages == null)
            {
                var AllImages = Imgur.Gallery().OfType<Imgur.Image>();
                GalleryImages = AllImages.GetEnumerator();
            }

            if (!GalleryImages.MoveNext())
            {
                return null;
            }

            string nextId = GalleryImages.Current.Id;
            Image bitmap = new Image(Retriever.RetrieveTiny(nextId));
            return bitmap;
        }

        ImgurClient Imgur;
        ImgurImageRetriever Retriever;
        IEnumerator<Imgur.Image> GalleryImages;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ImageCollection()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Imgur != null)
                {
                    Imgur.Dispose();
                    Imgur = null;
                }
                if (Retriever != null)
                {
                    Retriever.Dispose();
                    Retriever = null;
                }
            }
        }
    }
}
