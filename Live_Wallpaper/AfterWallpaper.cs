using Microsoft.Win32;
using System.Windows;
using System.Windows.Interop;

namespace Wallpaper_Engine
{
    public class AfterWallpaper
    {

        public static string GetFilepath()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "default.mp4";
            ofd.InitialDirectory = @"C:";
            ofd.Filter = "動画ファイル(*.mp4;*.avi;*.mov;*.webm;*.flv)|*.mp4;*.avi;*.mov;*.webm;*.flv";
            ofd.Title = "開くファイルを選択してください";
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;

            ofd.ShowDialog();
            //ファイルパス取得

            string afterfilepath =  ofd.FileName;

            return afterfilepath;
        }
        
        public static AfterWallpaperWindow Show(string afterfilepath)
        {
            AfterWallpaperWindow aww = new AfterWallpaperWindow(afterfilepath);
            aww.Show();
            new Wallpaper(new WindowInteropHelper(aww).Handle).SetBottom();
            
            aww.WindowState = WindowState.Maximized;

            return aww;
        }
    }
}