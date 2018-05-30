using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace MagicBox.Models
{
    public class Model : INotifyPropertyChanged
    {
        private DateTimeOffset _date;
        private Uri _songUri;
        private Uri _photoUri;
        private String _mood;
        private String _feedback;
        private String _id;
        private String _diary;
        private String _musicName;
        private String userName;

        public String musicName
        {
            get
            {
                return _musicName;
            }
            set
            {
                _musicName = value;
                RaisePropertyChanged("musicName");
            }
        }

        public String getId()
        {
            return _id;
        }

        public String getUserName()
        {
            return userName;
        }

        public DateTimeOffset date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                RaisePropertyChanged("date");
            }
        }

        public Uri songUri
        {
            get
            {
                return _songUri;
            }
            set
            {
                _songUri = value;
                RaisePropertyChanged("songUri");
            }
        }

        public Uri photoUri
        {
            get
            {
                return _photoUri;
            }
            set
            {
                _photoUri = value;
                RaisePropertyChanged("photoUri");
            }
        }
        public String mood
        {
            get
            {
                return _mood;
            }
            set
            {
                _mood = value;
                RaisePropertyChanged("mood");
            }
        }
        public String feedback
        {
            get
            {
                return _feedback;
            }
            set
            {
                _feedback = value;
                RaisePropertyChanged("feedback");
            }
        }
        public String diary
        {
            get
            {
                return _diary;
            }
            set
            {
                _diary = value;
                RaisePropertyChanged("diary");
            }
        }

        public Model()
        {
            this._id = Guid.NewGuid().ToString();
            this.date = DateTime.Now.Date;
            this.songUri = new Uri("ms-appx:///Assets/example.mp3");
            this.photoUri = new Uri("ms-appx:///Assets/example.jpg");
            this.mood = "sad";
            this.diary = "好吧";
            this.feedback = "啥子哟";
            this._musicName = "不爱我就拉倒";
            this.userName = App.userName;
        }

        public Model(Uri songUri, Uri photoUri, String mood, String diary, String feedback, String musicName)
        {
            this._id = Guid.NewGuid().ToString();
            this.date = DateTime.Now.Date;
            this.songUri = songUri;
            this.photoUri = photoUri;
            this.mood = mood;
            this.diary = diary;
            this.feedback = feedback;
            this._musicName = musicName;
            this.userName = App.userName;
        }

        public Model(String id, DateTimeOffset date, Uri songUri, Uri photoUri, String mood, String diary, String feedback, String musicName)
        {
            this._id = id;
            this.date = date;
            this.songUri = songUri;
            this.photoUri = photoUri;
            this.mood = mood;
            this.diary = diary;
            this.feedback = feedback;
            this._musicName = musicName;
            this.userName = App.userName;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(String _str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(_str));
            }
        }
    }
}
