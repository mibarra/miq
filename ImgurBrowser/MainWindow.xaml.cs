using Miq.ImgurBrowser.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Miq.ImgurBrowser
{
    public partial class MainWindow : Window, IDisposable
    {
        public MainWindow()
        {
            InitializeComponent();
            SetupImageLoader();
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Images = ImageList.DataContext as ImageCollection;
            LoadImages();
        }

        #region Loading Images
        void SetupImageLoader()
        {
            ImageLoader = new BackgroundWorker();
            ImageLoader.DoWork += ImageLoader_DoWork;
            ImageLoader.ProgressChanged += ImageLoader_ProgressChanged;
            ImageLoader.WorkerReportsProgress = true;
        }

        private void LoadImages()
        {
            ImageLoader.RunWorkerAsync(Images);
        }

        void ImageLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            ImageCollection images = e.Argument as ImageCollection;
            Data.Image nextImage;
            while ((nextImage = images.GetNextImage()) != null)
            {
                ImageLoader.ReportProgress(0, nextImage);
            }
        }

        private void ImageLoader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Data.Image image = e.UserState as Data.Image;
            Images.Insert(0, image);
        }

        BackgroundWorker ImageLoader;
        ImageCollection Images;
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        ~MainWindow()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(ImageLoader != null)
                {
                    ImageLoader.Dispose();
                    ImageLoader = null;
                }
                
            }
        }
    }
}
