using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Wallpaper_Engine
{
    public class Binding
    {
        private ObservableCollection<WallpaperList> databaseLists;
        private ObservableCollection<WallpaperList> bindingLists;

        public Binding(ObservableCollection<WallpaperList>databaseLists)
        {
            this.databaseLists = databaseLists;
        }

        public void Order(string order)
        {
            if (bindingLists != null && bindingLists.Count >= 2)
            {
                switch (order)
                {
                    case "名前（昇順）":
                        bindingLists = new ObservableCollection<WallpaperList>(bindingLists.OrderBy(n => n.Title));
                        break;
                    case "名前（降順）":
                        bindingLists = new ObservableCollection<WallpaperList>(bindingLists.OrderByDescending(n => n.Title));
                        break;
                    case "追加日時（新しい順）":
                        bindingLists = new ObservableCollection<WallpaperList>(bindingLists.OrderBy(n => n.time));
                        break;
                    case "追加日時（古い順）":
                        bindingLists = new ObservableCollection<WallpaperList>(bindingLists.OrderByDescending(n => n.time));
                        break;
                    case "サイズ（大きい順）":
                        bindingLists = new ObservableCollection<WallpaperList>(bindingLists.OrderBy(n => n.size));
                        break;
                    case "サイズ（小さい順）":
                        bindingLists = new ObservableCollection<WallpaperList>(bindingLists.OrderByDescending(n => n.size));
                        break;
                    default:
                        break;
                }
            }
        }

        public void Search(string title)
        {
            if (title == "検索")
            {
                title = "";
            }

            ObservableCollection<WallpaperList> bindingLists = new ObservableCollection<WallpaperList>();

            foreach (WallpaperList item in databaseLists)
            {
                if (item.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    bindingLists.Add(item);
                }
            }
            this.bindingLists = bindingLists;
        }

        public ObservableCollection<WallpaperList> GetBindingList()
        {
            return bindingLists;
        }
    }

    public class WallpaperList
    {
        public BitmapSource Image { get; set; }
        public string Title { get; set; }
        public int? id;
        public string filepath;
        public DateTime? time;
        public long? size;
    }
}