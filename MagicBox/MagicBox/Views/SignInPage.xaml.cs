using System;

using MagicBox.ViewModels;
using SQLitePCL;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MagicBox.Views
{
    public sealed partial class SignInPage : Page
    {
        private SignInViewModel ViewModel
        {
            get { return DataContext as SignInViewModel; }
        }

        public SignInPage()
        {
            InitializeComponent();
        }

        private void LogIn(object sender, RoutedEventArgs e)
        {
            if (userNameTextBox.Text.Trim() == String.Empty )
            {
                var message = new MessageDialog("用户名不能为空！").ShowAsync();
                return;
            }

            //用户输入的用户名和密码
            String userName = userNameTextBox.Text.Trim();
            String password = passwordBox.Password;

            //数据库返回的用户名和密码
            String userResult = String.Empty;
            String pwResult = String.Empty;

            //在数据库中查找该用户
            var db = App.connection;
            using (var statement = db.Prepare(App.SEARCHUSER))
            {
                statement.Bind(1, userName);
                while (SQLiteResult.ROW == statement.Step())
                {
                    userResult += statement[0].ToString();
                    pwResult += statement[1].ToString();
                }
            }

            if(userResult == String.Empty)
            {
                var message = new MessageDialog("该用户不存在！").ShowAsync();
            }
            else
            {
                if(password == pwResult)
                {
                    App.userName = userName;
                    Frame.Navigate(typeof(MainPage));
                    
                }
                else
                {
                    var message = new MessageDialog("密码错误！").ShowAsync();
                }
            }
        }

        private void SignUp(object sender, RoutedEventArgs e)
        {
            if (userNameTextBox.Text.Trim() == String.Empty)
            {
                var message = new MessageDialog("用户名不能为空！").ShowAsync();
                return;
            }
            if (passwordBox.Password == String.Empty)
            {
                var message = new MessageDialog("密码不能为空！").ShowAsync();
                return;
            }

            //用户输入的用户名和密码
            String userName = userNameTextBox.Text.Trim();
            String password = passwordBox.Password;

            //数据库返回的用户名和密码
            String userResult = String.Empty;
            String pwResult = String.Empty;

            //在数据库中查找该用户
            var db = App.connection;
            using (var statement = db.Prepare(App.SEARCHUSER))
            {
                statement.Bind(1, userName);
                while (SQLiteResult.ROW == statement.Step())
                {
                    userResult += statement[0].ToString();
                    pwResult += statement[1].ToString();
                }
            }

            //不存在则允许注册
            if(userResult == String.Empty)
            {
                try
                {
                    using (var statement = db.Prepare(App.INSERTUSER))
                    {
                        statement.Bind(1, userName);
                        statement.Bind(2, password);
                        statement.Step();
                    }
                    var message = new MessageDialog("注册成功！").ShowAsync();
                    userNameTextBox.Text = "";
                    passwordBox.Password = "";
                }
                catch(Exception ex)
                {
                    throw (ex);
                }    
            }
            else
            {
                var message = new MessageDialog("该用户已存在！").ShowAsync();
            }
            
        }
    }
}
