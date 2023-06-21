using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;

namespace Wallpaper_Engine
{
    public class Database
    {
        private int? id;
        private string filepath;
        private string title;
        private DateTime? time;
        private long? size;

        public Database(int? id, string filepath, string title, DateTime? time, long? size)
        {
            this.id = id;
            this.filepath = filepath;
            this.title = title;
            this.time = time;
            this.size = size;
        }

        private string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\wakab\source\repos\Wallpaper_Engine\Wallpaper_Engine\WallpaperDatabase.mdf;Integrated Security = True";

        public void Insert()
        {
            string query = "INSERT INTO Wallpaper(filepath, title, time, size) VALUES(@filepath, @title, @time, @size)";
            using SqlConnection connection = new SqlConnection(ConnectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@filepath", filepath);
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@time", time);
            command.Parameters.AddWithValue("@size", size);

            try
            {
                // データベースの接続開始
                connection.Open();

                // SQLの実行
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                // データベースの接続終了
                connection.Close();
            }

        }

        public ObservableCollection<WallpaperList> Select()
        {
            ObservableCollection<WallpaperList> databaseLists = new ObservableCollection<WallpaperList>();

            string query = "SELECT id, filepath, title, time, size FROM Wallpaper";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    // データベースの接続開始
                    connection.Open();

                    // SQLの実行
                    using SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        databaseLists.Add(new WallpaperList()
                        {
                            Image = Thumbnail.Create(reader["filepath"] as string),
                            Title = reader["title"] as string,
                            id = (int)reader["id"],
                            filepath = reader["filepath"] as string,
                            time = reader["time"] as DateTime?,
                            size = reader["size"] as long?
                        });
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                finally
                {
                    // データベースの接続終了
                    connection.Close();
                }
            }
            return databaseLists;
        }

        public void Delete()
        {
            string query = "DELETE FROM Wallpaper WHERE id=@id";
            using SqlConnection connection = new SqlConnection(ConnectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            try
            {
                // データベースの接続開始
                connection.Open();

                command.Parameters.Add(new SqlParameter("@id", id));

                // SQLの実行
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                // データベースの接続終了
                connection.Close();
            }
        }
    }
}
