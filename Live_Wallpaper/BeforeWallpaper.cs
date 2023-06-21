using System;
using System.IO;
using System.Collections.Generic;
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
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Wallpaper_Engine
{

    public class BeforeWallpaper
    {
        /*private string beforefilepath;

        public BeforeWallpaper(string beforefilepath)
        {
            this.beforefilepath = beforefilepath;
        }*/

        [DllImport("user32.dll")]
        private static extern bool SystemParametersInfo(int uiAction, int uiParam, StringBuilder pvParam, int fWinIni);
        
        private const int SPI_SETDESKWALLPAPER = 0x0014;
        private const int SPIF_UPDATEINIFILE = 1;
        private const int SPIF_SENDWININICHANGE = 2;

        public static string GetFilepath()
        {
            int SPI_GETDESKWALLPAPER = 0x73;
            int MAX_PATH = 260;

            StringBuilder sb = new StringBuilder(MAX_PATH);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, MAX_PATH, sb, 0);
            //ファイルパス取得
            string beforefilepath = sb.ToString();

            return beforefilepath;
        }

        public static void Show(string beforefilepath)
        {
            StringBuilder sb = new StringBuilder(beforefilepath);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, sb.Length, sb, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
