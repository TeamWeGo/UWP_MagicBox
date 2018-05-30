using System;
using System.Collections.ObjectModel;
using System.Collections;
using GalaSoft.MvvmLight;

namespace MagicBox.ViewModels
{
    public class SignInViewModel : ViewModelBase
    {
        private Hashtable allUsers = new Hashtable();

        public SignInViewModel()
        {
        }

        public Boolean LogIn(String username,String password)
        {
            foreach(DictionaryEntry de in allUsers)
            {
                if((String)de.Key == username && (String)de.Value == password)
                {
                    MainViewModel.currentUser = username;
                    return true;
                }
            }
            return false;
        }

        public Boolean SignUp(String username, String password)
        {
            foreach (DictionaryEntry de in allUsers)
            {
                if ((String)de.Key == username)
                {
                    return false;
                }
            }
            allUsers.Add(username, password);
            return true;
        }
    }
}
