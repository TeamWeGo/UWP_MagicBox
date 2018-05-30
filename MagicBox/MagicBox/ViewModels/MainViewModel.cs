using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using MagicBox.Models;
using SQLitePCL;

namespace MagicBox.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public static String currentUser = "";

        // private static MainViewModel viewModel;
        public MainViewModel()
        {
        }

        public void initiate()
        {
            this.allItems.Clear();
            currentUser = App.userName;

            //默认加入的用于测试的item
            //var item = new Model();  
            //this.allItems.Add(item); 

            //从数据库载入该用户的items
            var db = App.connection;
            using (var statement = db.Prepare(App.SELECT))
            {
                statement.Bind(1, currentUser);
                while (SQLiteResult.ROW == statement.Step())
                {
                    DateTime date = Convert.ToDateTime(statement[1].ToString());
                    var songUri = new Uri(statement[2].ToString());
                    var photoUri = new Uri(statement[3].ToString());
                    var newItem = new Model(statement[0].ToString(), date, songUri, photoUri, statement[4].ToString(), statement[6].ToString(), statement[5].ToString(), statement[7].ToString());
                    this.allItems.Add(newItem);

                }
            }
        }

        private ObservableCollection<Models.Model> allItems = new ObservableCollection<Models.Model>();
        public ObservableCollection<Models.Model> AllItems { get { return this.allItems; } }

        private Models.Model selectedItem = default(Models.Model);
        public Models.Model SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public void AddItem(Uri songUri, Uri photoUri, String mood, String diary, String feedback, String musicName)
        {
            String id = Guid.NewGuid().ToString();
            var newItem = new Model(id, DateTime.Now.Date, songUri, photoUri, mood, diary, feedback, musicName);
            this.allItems.Add(newItem);

            String song = songUri.ToString();
            String photo = photoUri.ToString();
            String date = DateTime.Now.Date.ToString();

            var db = App.connection;
            try
            {
                using (var statement = db.Prepare(App.INSERT))
                {

                    statement.Bind(1, id);
                    statement.Bind(2, date);
                    statement.Bind(3, song);
                    statement.Bind(4, photo);
                    statement.Bind(5,mood);
                    statement.Bind(6, feedback);
                    statement.Bind(7, diary);
                    statement.Bind(8, musicName);
                    statement.Bind(9, currentUser);
                    statement.Step();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public void updateItem(String id, Uri songUri, Uri photoUri, String mood, String diary, String feedback, String musicName)
        {
            //修改ViewModel
            foreach (var it in allItems)
            {
                if (it.getId() == id)
                {
                    it.songUri = songUri;
                    it.photoUri = photoUri;
                    it.mood = mood;
                    it.diary = diary;
                    it.feedback = feedback;
                    it.musicName = musicName;
                    break;
                }
            }

            String song = songUri.ToString();
            String photo = photoUri.ToString();

            //修改数据库
            var db = App.connection;
            try
            {
                using (var statement = db.Prepare(App.UPDATE))
                {
                    statement.Bind(1, song);
                    statement.Bind(2, photo);
                    statement.Bind(3, mood);
                    statement.Bind(4, feedback);
                    statement.Bind(5, diary);
                    statement.Bind(6, musicName);
                    statement.Bind(7, id);
                    statement.Step();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public void deleteItem(String id)
        {
            foreach (var it in allItems)
            {
                if (it.getId() == id)
                {
                    allItems.Remove(it);
                    break;
                }
            }
            var db = App.connection;
            try
            {
                using (var statement = db.Prepare(App.DELETE))
                {
                    statement.Bind(1, id);            
                    statement.Step();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

    }
}
