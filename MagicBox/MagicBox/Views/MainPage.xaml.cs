using System;
using Windows.System;
using MagicBox.Models;
using MagicBox.ViewModels;
using Windows.Media.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Popups;
using System.Text;
using SQLitePCL;
using Windows.Media.Capture;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Navigation;
using Windows.Networking.Connectivity;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.Generic;

namespace MagicBox.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel
        {
            get { return DataContext as MainViewModel; }
        }

        public MainPage()
        {
            InitializeComponent();
            ViewModel.initiate();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
            weatherInit();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
        }

        private void ListViewItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Model)e.ClickedItem;
            if (this.ActualWidth < 80)
            {
                //Frame.Navigate(typeof(DetailPage));
            }
            else
            {
                photoImageDetail.Source = new BitmapImage(ViewModel.SelectedItem.photoUri);
                moodTextBlockDetail.Text = ViewModel.SelectedItem.mood;
                dateDatePickerDetail.Date = ViewModel.SelectedItem.date;
                diaryTextBoxDetail.Text = ViewModel.SelectedItem.diary;
                feedbackTextBlock.Text = ViewModel.SelectedItem.feedback;
                mediaElement.Source = ViewModel.SelectedItem.songUri;
                musicNameTextBlock.Text = ViewModel.SelectedItem.musicName;
                var tempdate = ViewModel.SelectedItem.date;
                tempdate = tempdate.AddDays(2);
                if (DateTimeOffset.Compare(DateTimeOffset.Now, tempdate) > 0)
                {
                    diaryTextBoxDetail.IsEnabled = false;
                }
                else
                {
                    diaryTextBoxDetail.IsEnabled = true;
                }
                createButton.Content = "Update";
            }
        }

        /* private void PlayClick(object sender, RoutedEventArgs e)
           {
               Play.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
               Pause.Visibility = Windows.UI.Xaml.Visibility.Visible;
               mediaElement.Play();
           }

           private void PauseClick(object sender, RoutedEventArgs e)
           {
               Pause.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
               Play.Visibility = Windows.UI.Xaml.Visibility.Visible;
               mediaElement.Pause();
           }
           */
        private async void SelectClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".wma");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                string type = file.FileType;
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                if (type == ".mp3" || type == ".wma")
                {
                    //  mediaElement.Visibility = Visibility.Collapsed;
                    //    Play.Visibility = Visibility.Collapsed;
                    //   Pause.Visibility = Visibility.Visible;
                    var filename = Guid.NewGuid().ToString() + ".mp3";
                    await file.CopyAsync(ApplicationData.Current.LocalFolder, filename, NameCollisionOption.ReplaceExisting);
                    Uri uri = new Uri("ms-appdata:///local/" + filename);
                    musicNameTextBlock.Text = file.Name;
                    mediaElement.Source = uri;
                    mediaElement.Play();
                }
            }
        }

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            if(App.userName == "")
            {
                var message = new MessageDialog("请先登陆！").ShowAsync();
                Frame.Navigate(typeof(SignInPage));
                return;
            }
            if(diaryTextBoxDetail.Text.Trim() == String.Empty)
            {
                var message = new MessageDialog("日记内容未填写！").ShowAsync();
                return;
            }
            if (mediaElement.Source == null) {
                var message = new MessageDialog("歌曲未选择！").ShowAsync();
                return;
            }

            if(createButton.Content.ToString() == "Update")
            {
                ViewModel.updateItem(ViewModel.SelectedItem.getId(), mediaElement.Source,(photoImageDetail.Source as BitmapImage).UriSource,
                    moodTextBlockDetail.Text, diaryTextBoxDetail.Text, feedbackTextBlock.Text, musicNameTextBlock.Text);
                var message = new MessageDialog("更新成功！").ShowAsync();
                ViewModel.SelectedItem = null;
            }
            else if(createButton.Content.ToString() == "Create")
            {
                Uri x = (photoImageDetail.Source as BitmapImage).UriSource;
                ViewModel.AddItem(mediaElement.Source, (photoImageDetail.Source as BitmapImage).UriSource,
                    moodTextBlockDetail.Text, diaryTextBoxDetail.Text, feedbackTextBlock.Text, musicNameTextBlock.Text);
                var message = new MessageDialog("创建成功！").ShowAsync();
            }
            diaryTextBoxDetail.IsEnabled = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedItem = null;
            Frame.Navigate(typeof(MainPage));
            diaryTextBoxDetail.IsEnabled = true;
        }

        private void DeleteClick(object sender, RoutedEventArgs e) {
            var message = new MessageDialog("删除日记").ShowAsync();
            diaryTextBoxDetail.Text =  "diary";
            if (ViewModel.SelectedItem.getId() != null)
            {
                ViewModel.deleteItem(ViewModel.SelectedItem.getId());
            }
            BitmapImage temp = new BitmapImage(new Uri("ms-appx:///Assets/example.jpg"));
            photoImageDetail.Source = temp;
            moodTextBlockDetail.Text = "mood";
            dateDatePickerDetail.Date = DateTimeOffset.Now;
            diaryTextBoxDetail.Text = "";
            feedbackTextBlock.Text = "feedback";
            mediaElement.Source = null;
            musicNameTextBlock.Text = "";
            createButton.Content = "Create";
            diaryTextBoxDetail.IsEnabled = true;
        }

        private async void PhotoClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            var picture = await picker.PickSingleFileAsync();
            if (picture != null)
            {
                var temp = Guid.NewGuid().ToString()+".png";
                await picture.CopyAsync(ApplicationData.Current.LocalFolder, temp, NameCollisionOption.ReplaceExisting);
                BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appdata:///local/" + temp));
                photoImageDetail.Source = bitmapImage;

                IBuffer buffer = await FileIO.ReadBufferAsync(picture);
                using (DataReader dr = DataReader.FromBuffer(buffer))
                {
                    byte[] byte_Data = new byte[dr.UnconsumedBufferLength];
                    dr.ReadBytes(byte_Data);

                    MakeRequest(byte_Data);
                }
            }
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            StringBuilder strBuilder = new StringBuilder("%%");
            strBuilder.Insert(1, searchTextBox.Text);
            String result = String.Empty;

            var db = App.connection;
            using (var statement = db.Prepare(App.SEARCH))
            {
                statement.Bind(1, strBuilder.ToString());
                statement.Bind(2, strBuilder.ToString());
                statement.Bind(3, strBuilder.ToString());
                
                while (SQLiteResult.ROW == statement.Step())
                {
                    if(statement[0].ToString() == App.userName)
                    {
                        result += "日期: " + statement[1].ToString() + " ";
                        result += "歌名: " + statement[2].ToString() + " ";
                        result += "心情: " + statement[3].ToString() + "\n";
                    }                 
                }
            }
            if(result != "")
            {
                var message = new MessageDialog(result).ShowAsync();
            }
            else
            {
                var message = new MessageDialog("没有查找到相关内容！").ShowAsync();
            }
            
            
        }

        private async void takePhotoClick(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.AllowCropping = false;
            //captureUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.Large3M;

            StorageFile picture = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (picture != null)
            {
                var temp = Guid.NewGuid().ToString()+".Jpeg";
                await picture.CopyAsync(ApplicationData.Current.LocalFolder, temp, NameCollisionOption.ReplaceExisting);
                BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appdata:///local/" + temp));
                photoImageDetail.Source = bitmapImage;

                IBuffer buffer = await FileIO.ReadBufferAsync(picture);
                using (DataReader dr = DataReader.FromBuffer(buffer))
                {
                    byte[] byte_Data = new byte[dr.UnconsumedBufferLength];
                    dr.ReadBytes(byte_Data);

                    MakeRequest(byte_Data);
                }
            }
        }

        private async void MakeRequest(byte[] byteData)
        {
            var client = new HttpClient();

            const string uriBase =
            "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect";
            // Request headers - replace this example key with your valid key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "5f571faff94e48f2a94e3b221d3bdaf8"); // 

            // NOTE: You must use the same region in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westcentralus, replace "westus" in the 
            //   URI below with "westcentralus".
            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored JPEG image.

            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
                "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            // Request body. Posts a locally stored JPEG image.

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                moodTextBlockDetail.Text = "正在识别表情……";
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                JsonReader jsonReader = new JsonTextReader(new StringReader(contentString));
                dynamic Data = Newtonsoft.Json.JsonConvert.DeserializeObject(contentString);
                if (contentString == "[]")
                {
                    moodTextBlockDetail.Text = "detect emotion fail";
                    feedback();
                    return;
                }
                var face = Data[0];
                var emotion = face.faceAttributes.emotion;
                double max = 0;
                var flag = -1;
                var i = 0;
                foreach (var item in emotion)
                {
                    String value = (String)item;
                    double valuedata = double.Parse(value);
                    if (valuedata > max)
                    {
                        max = valuedata;
                        flag = i;
                    }
                    i++;
                }
                string[] emotions = new string[8] { "anger", "contempt", "disgust", "fear", "happiness", "neutral", "sadness", "surprise" };
                moodTextBlockDetail.Text = emotions[flag];
                feedback();
            }
        }

        private void feedback()
        {
            String emotion = moodTextBlockDetail.Text;

            string[] positive = new string[]
            {
                "也许有一天，你发觉日子特别的艰难，那可能是这次的收获特别的巨大。",
                "生活总会给你另一个机会，这个机会叫明天。",
                "遥不可及的并非是十年之后，而是今天之前。触手可及的就是明天，愿一觉醒来，又是全新的自己。",
                "最幸福的事情就是无忧无虑陪着自己心爱的人与世无争。",
                "你不尝试着做些能力之外的事情，就永远无法成长。",
                "深谙世故却不世故，才是最善良的成熟。",
                "智者顺时而谋,愚者逆时而动。",
                "人若在面临抉择而无法取舍的时候,应该选择自己尚未经验过的那一个。",
                "淡定是一种人生涵养,纯真是一种性格使然。",
                "蝴蝶变成了花,不用再以不停的飞翔表明自己自由,平淡安静的留守也是一种幸福的忧愁"
            };

            String[] passive = new string[]
            {
                "积极向上的人总是把苦难化为积极向上的动力。",
                "当你感到悲哀痛苦时，最好是去学些什么东西。学习会使你永远立于不败之地。",
                "世上只有一种英雄主义，就是在认清生活真相之后依然热爱生活。",
                "付出不一定有收获，努力了就值得了。",
                "做一个精彩的自己，跟着自己的直觉走，别怕失去，别怕失败，别怕路远，做了才有对错，经历才有回忆。",
                "人一生至少要有两次冲动，一次奋不顾身的爱情，一次说走就走的旅行。",
                "你不尝试着做些能力之外的事情，就永远无法成长。",
                "深谙世故却不世故，才是最善良的成熟。",
                "总是需要一些温暖。哪怕是一点点自以为是的纪念。",
                "想念只是一种仪式，真正的记忆与生俱来。",
                "幸福是比较级，要有东西垫底才感觉的到。",
                "你可以坚持自己的理想，但不需固执自己的想法。",
                "生活不是等待风暴过去，而是学会在雨中慢舞。",
                "有时候你以为天要塌下来了，其实是自己站歪了。",
                "那些繁华哀伤终成过往，请不要失望，平凡是为了最美的荡气回肠。",
                "人若在面临抉择而无法取舍的时候,应该选择自己尚未经验过的那一个。",
                "蝴蝶变成了花,不用再以不停的飞翔表明自己自由,平淡安静的留守也是一种幸福的忧愁"
            };

            String[] neutral = new string[]
            {
                "人生就有许多这样的奇迹，看似比登天还难的事，有时轻而易举就可以做到，其中的差别就在于非凡的信念。",
                "在等待的日子里，刻苦读书，谦卑做人，养得深根，日后才能枝叶茂盛。",
                "你今天的努力，是幸运的伏笔。当下的付出，是明日的花开。",
                "人一生至少要有两次冲动，一次奋不顾身的爱情，一次说走就走的旅行。",
                "最幸福的事情就是无忧无虑陪着自己心爱的人与世无争。",
                "如果你懂得珍惜，你会发现你获得的越来越多。",
                "你不尝试着做些能力之外的事情，就永远无法成长。",
                "深谙世故却不世故，才是最善良的成熟。",
                "总是需要一些温暖。哪怕是一点点自以为是的纪念。",
                "想念只是一种仪式，真正的记忆与生俱来。",
                "在这个世上，只有真正快乐的男人、才能带给女人真正的快乐。",
                "当金钱站起来说话时，所有的真理都沉默了。",
                "幸福是比较级，要有东西垫底才感觉的到。",
                "你可以坚持自己的理想，但不需固执自己的想法。",
                "人犯错误，多半是在该用真情时太过动脑筋，而在该用脑筋时又太感情用事。",
                "生活不是等待风暴过去，而是学会在雨中慢舞。",
                "有时候你以为天要塌下来了，其实是自己站歪了。",
                "那些繁华哀伤终成过往，请不要失望，平凡是为了最美的荡气回肠。",
                "怜悯是一笔借款，为小心起见，还是不要滥用为好。",
                "智者顺时而谋,愚者逆时而动。",
                "人若在面临抉择而无法取舍的时候,应该选择自己尚未经验过的那一个。",
                "淡定是一种人生涵养,纯真是一种性格使然。",
                "蝴蝶变成了花,不用再以不停的飞翔表明自己自由,平淡安静的留守也是一种幸福的忧愁"
            };
            String[] feedback = null;
            if (emotion == "happiness")
            {
                feedback = positive;
            }
            else if (emotion == "sadness" || emotion == "fead")
            {
                feedback = passive;
            }
            else
            {
                feedback = neutral;
            }
            Random rand = new Random();
            int random = rand.Next(0, feedback.Length - 1);
            feedbackTextBlock.Text = feedback[random];
        }

        async void getWeather(string location)
        {

            string url = "http://v.juhe.cn/weather/index?cityname=" + location + "&dtype=xml&format=2&key=8f9462857e209325b1ffc655525ce3a5";
            Uri uri = new Uri(url);
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            var httpResponseMessage = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                httpResponseMessage = await httpClient.GetAsync(uri);
                httpResponseMessage.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception exception)
            {
                httpResponseBody = "ERROR " + exception.HResult.ToString("X") + "Message " + exception.Message;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(httpResponseBody);
            XmlNodeList list = xmlDocument.GetElementsByTagName("weather");
            IXmlNode node = list.Item(0);
            weather.Text = location + ":" + node.InnerText;

        }

        async void weatherInit()
        {
            var ip = "";

            var hosts = NetworkInformation.GetHostNames();
            // 筛选无线或以太网
            var host = hosts.FirstOrDefault(h =>
            {
                bool isIpaddr = (h.Type == Windows.Networking.HostNameType.Ipv4) || (h.Type == Windows.Networking.HostNameType.Ipv6);
                // 如果不是IP地址表示的名称，则忽略
                if (isIpaddr == false)
                {
                    return false;
                }
                IPInformation ipinfo = h.IPInformation;
                // 71表示无线，6表示以太网
                if (ipinfo.NetworkAdapter.IanaInterfaceType == 71 || ipinfo.NetworkAdapter.IanaInterfaceType == 6)
                {
                    return true;
                }
                return false;
            });
            if (host != null)
            {
                ip = host.DisplayName; //显示IP
            }


            var city = "";
            string url = "http://ip.taobao.com/service/getIpInfo.php?ip=" + ip;
            Uri uri = new Uri(url);
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            Windows.Web.Http.HttpResponseMessage httpResponseMessage = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                httpResponseMessage = await httpClient.GetAsync(uri);
                httpResponseMessage.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception exception)
            {
                httpResponseBody = "ERROR " + exception.HResult.ToString("X") + "Message " + exception.Message;
            }

            JsonReader jsonReader = new JsonTextReader(new StringReader(httpResponseBody));
            while (jsonReader.Read())
            {
                //   Console.WriteLine(jsonReader.TokenType + "\t\t" + jsonReader.ValueType + "\t\t" + jsonReader.Value);
                if (jsonReader.Path == "data.city")
                {
                    city = jsonReader.Value.ToString();
                }
            }
            getWeather(city);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            dynamic select = e.OriginalSource;
            ViewModel.SelectedItem = (Models.Model)select.DataContext;
            if (ViewModel.SelectedItem != null)
            {
                photoImageDetail.Source = new BitmapImage(ViewModel.SelectedItem.photoUri);
                moodTextBlockDetail.Text = ViewModel.SelectedItem.mood;
                dateDatePickerDetail.Date = ViewModel.SelectedItem.date;
                diaryTextBoxDetail.Text = ViewModel.SelectedItem.diary;
                feedbackTextBlock.Text = ViewModel.SelectedItem.feedback;
                mediaElement.Source = ViewModel.SelectedItem.songUri;
                musicNameTextBlock.Text = ViewModel.SelectedItem.musicName;
                var tempdate = ViewModel.SelectedItem.date;
                tempdate = tempdate.AddDays(2);
                if (DateTimeOffset.Compare(DateTimeOffset.Now,tempdate)>0)
                {
                    diaryTextBoxDetail.IsEnabled = false;
                }
                else
                {
                    diaryTextBoxDetail.IsEnabled = true;
                }
                createButton.Content = "Update";
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            dynamic select = e.OriginalSource;
            ViewModel.SelectedItem = (Models.Model)select.DataContext;
            var message = new MessageDialog("删除日记").ShowAsync();
            diaryTextBoxDetail.Text = "";
            if (ViewModel.SelectedItem != null)
            {
                if (ViewModel.SelectedItem.getId() != null)
                {
                    ViewModel.deleteItem(ViewModel.SelectedItem.getId());
                }
                ViewModel.SelectedItem = null;
            }
            BitmapImage temp = new BitmapImage(new Uri("ms-appx:///Assets/example.jpg"));
            photoImageDetail.Source = temp;
            moodTextBlockDetail.Text = "mood";
            dateDatePickerDetail.Date = DateTimeOffset.Now;
            diaryTextBoxDetail.Text = "";
            feedbackTextBlock.Text = "feedback";
            mediaElement.Source = null;
            musicNameTextBlock.Text = "";
            createButton.Content = "Create";
            diaryTextBoxDetail.IsEnabled = true;
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            dynamic select = e.OriginalSource;
            ViewModel.SelectedItem = (Model)select.DataContext;
            if (ViewModel.SelectedItem != null)
            {
                DataTransferManager.ShowShareUI();
            }
        }

        async void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var dp = args.Request.Data;
            var deferral = args.Request.GetDeferral();
            BitmapImage bitmapImage = new BitmapImage(ViewModel.SelectedItem.photoUri);
            
            var photoFile = await StorageFile.GetFileFromApplicationUriAsync(bitmapImage.UriSource);
            var musicFile = await StorageFile.GetFileFromApplicationUriAsync(mediaElement.Source);
            dp.Properties.Title = (string)ViewModel.SelectedItem.musicName;
            dp.Properties.Description = (string)ViewModel.SelectedItem.diary;
            dp.SetStorageItems(new List<StorageFile> { photoFile ,musicFile});
            deferral.Complete();
        }
    }
}
