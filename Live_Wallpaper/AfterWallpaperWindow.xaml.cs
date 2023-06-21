using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wallpaper_Engine
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class AfterWallpaperWindow : Window
    {
        private string afterfilepath;
        public AfterWallpaperWindow(string afterfilepath)
        {
            this.afterfilepath = afterfilepath;
            InitializeComponent();
        }

        private void AfterWallpaperWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AfterMediaElement.Source = new Uri(afterfilepath);
            AfterMediaElement.Play();
        }

        private void AfterMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            AfterMediaElement.Position = new TimeSpan(0);
        }
    }
}
