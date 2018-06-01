using System;

using MagicBox.Services;
using SQLitePCL;
using Windows.ApplicationModel.Activation;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace MagicBox
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        //创建SLQiteConnection
        public static SQLiteConnection connection;
        public App()
        {
            InitializeComponent();

            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
            this.LoadDatabase();

            //每次启动程序都先清空通知队列
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            //对所有磁贴启动通知队列
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
        }

        //全局的用户名
        public static String userName = "";

        //将mysql语句保存在字符串中
        //Users
        public static String CREATEUSER = @"CREATE TABLE IF NOT EXISTS Users(userName VARCHAR(140) PRIMARY KEY NOT NULL, password VARCHAR(140) );";
        public static String INSERTUSER = @"INSERT INTO Users(userName, password) VALUES(?,?);";
        public static String SEARCHUSER = @"SELECT userName, password FROM Users WHERE userName = ?";
        //Items
        public static String CREATE = @"CREATE TABLE IF NOT EXISTS Items(Id VARCHAR(140) PRIMARY KEY NOT NULL, Date VARCHAR(140), songUri VARCHAR(140), photoUri VARCHAR(140), mood VARCHAR(140), feedback VARCHAR(140), diary VARCHAR(140), musicName VARCHAR(140), userName VARCHAR(140));";
        public static String INSERT = @"INSERT INTO Items(Id, Date, songUri, photoUri, mood, feedback, diary, musicName, userName) VALUES(?,?,?,?,?,?,?,?,?);";
        public static String SELECT = @"SELECT Id, Date, songUri, photoUri, mood, feedback, diary, musicName FROM Items WHERE userName = ?;";
        public static String UPDATE = @"UPDATE Items SET songUri = ?, photoUri = ?, mood = ?, feedback = ?, diary = ?, musicName = ? WHERE Id=?;";
        public static String DELETE = @"DELETE FROM Items WHERE Id=?";
        public static String SEARCH = @"SELECT userName, Date, musicName, mood FROM Items WHERE musicName LIKE ? OR Date LIKE ? OR mood LIKE ? ";

        //加载数据库
        private void LoadDatabase()
        {
            connection = new SQLiteConnection("MagicBox.db");
            //创建User的表
            using (var statement = connection.Prepare(CREATEUSER))
            {
                statement.Step();
            }
            //创建Item的表
            using (var statement = connection.Prepare(CREATE))
            {
                statement.Step();
            }
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(ViewModels.SignInViewModel), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }
    }
}
