using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace WpfPlayground
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PrintLogicalTree(this);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            PrintVisualTree(this);
        }

        private void PrintVisualTree(Object obj, int level = 0)
        {
            Debug.WriteLine(new string(' ', level) + obj);

            var childCount = VisualTreeHelper.GetChildrenCount(obj as DependencyObject);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(obj as DependencyObject, i);
                PrintVisualTree(child, level + 1);
            }
        }

        private void PrintLogicalTree(object obj, int level = 0)
        {
            Debug.WriteLine(new string(' ', level) + obj);
            if (!(obj is DependencyObject))
            {
                return;
            }

            foreach (object child in LogicalTreeHelper.GetChildren(obj as DependencyObject))
            {
                PrintLogicalTree(child, level + 1);
            }
        }
    }
}

