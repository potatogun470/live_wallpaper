using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Wallpaper_Engine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool window_loaded = false;
        private string beforefilepath = null;
        private string afterfilepath = null;
        private ObservableCollection<WallpaperList> databaseLists;
        private WallpaperList item;
        private AfterWallpaperWindow aww;
        private Binding binding;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            window_loaded = true;
            databaseLists = new Database(null, null, null, null, null).Select();
            Wallpaper_ListView.DataContext = databaseLists;
            binding = new Binding(databaseLists);
            binding.Search("");
        }
        private void Search_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Search_TextBox.Text == "検索")
            {
                Search_TextBox.Text = null;
            }
            Search_TextBox.Foreground = Brushes.White;
        }

        private void Search_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Search_TextBox.Text == "")
            {
                Search_TextBox.Text = "検索";
            }

            Search_TextBox.Foreground = Brushes.Gray;
        }

        private void Search_TextChanged(object sender, EventArgs e)
        {
            if (window_loaded)
            {
                binding.Search(Search_TextBox.Text);
                Wallpaper_ListView.DataContext = binding.GetBindingList();
            }
        }

        private void Order_SelectionChanged(object sender, EventArgs e)
        {
            binding.Order(Order_ComboBox.Text);
            Wallpaper_ListView.DataContext = binding.GetBindingList();
        }

        private void File_Button_Click(object sender, RoutedEventArgs e)
        {
            //元の壁紙の画像のパス取得
            beforefilepath = BeforeWallpaper.GetFilepath();
            //追加するライブ壁紙のファイル選択
            afterfilepath = AfterWallpaper.GetFilepath();
            //追加するライブ壁紙のパスを表示
            File_TextBlock.Text = afterfilepath;
        }

        private void File_DragEnter(object sender, DragEventArgs e)
        {
            //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            //ファイル以外は受け付けない
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void File_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            string[] dragFilePathArr = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (dragFilePathArr.Length == 1)
            {
                //元の壁紙の画像のパス取得
                beforefilepath = BeforeWallpaper.GetFilepath();
                //追加するライブ壁紙のパスを取得
                afterfilepath = dragFilePathArr[0];
                File_TextBlock.Text = afterfilepath;
            }
            else
            {
                MessageBox.Show("ファイルは１つだけにしてください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (afterfilepath != null && Title_TextBox.Text != null)
            {
                //データベース挿入
                Database database = new Database(null, afterfilepath, Title_TextBox.Text, DateTime.Now, new FileInfo(afterfilepath).Length);
                database.Insert();
                databaseLists = database.Select();
                binding = new Binding(databaseLists);
                binding.Search(Search_TextBox.Text);
                binding.Order(Order_ComboBox.Text);
                Wallpaper_ListView.DataContext = binding.GetBindingList();
                File_TextBlock.Text = "";
                Title_TextBox.Text = "";
            }
            else if (afterfilepath == null && Title_TextBox.Text != null)
            {
                File_TextBlock.Text = "ファイルを選択してください";
            }
            else if (afterfilepath != null && Title_TextBox.Text == null)
            {
                File_TextBlock.Text = "タイトルを入力してください";
            }
            else
            {
                File_TextBlock.Text = "ファイルを選択し、タイトルを入力してください";
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Wallpaper_ListView.SelectedItem == null)
            {
                return;
            }

            item = (WallpaperList)Wallpaper_ListView.SelectedItem;
            Preview_MediaElement.Source = new Uri(item.filepath);
            Title_TextBlock.Text = item.Title;
            Filepath_TextBlock.Text = item.filepath;
            Time_TextBlock.Text = item.time.ToString();
            Size_TextBlock.Text = string.Format("{0:#,0}KB", item.size / 1024);
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            aww.Close();
            if (beforefilepath != null)
            {
                //元の壁紙の画像を表示
                BeforeWallpaper.Show(beforefilepath);
            }
        }

        private void Display_Button_Click(object sender, RoutedEventArgs e)
        {
            if (item != null)
            {
                beforefilepath = BeforeWallpaper.GetFilepath();
                aww = AfterWallpaper.Show(item.filepath);
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            Database database = new Database(item.id, null, null, null, null);
            database.Delete();
            databaseLists = database.Select();
            binding = new Binding(databaseLists);
            binding.Search(Search_TextBox.Text);
            binding.Order(Order_ComboBox.Text);
            Wallpaper_ListView.DataContext = binding.GetBindingList();
        }
    }
}
