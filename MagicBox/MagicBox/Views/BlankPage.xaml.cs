using System;

using MagicBox.ViewModels;

using Windows.UI.Xaml.Controls;

namespace MagicBox.Views
{
    public sealed partial class BlankPage : Page
    {
        private BlankViewModel ViewModel
        {
            get { return DataContext as BlankViewModel; }
        }

        public BlankPage()
        {
            InitializeComponent();
        }
    }
}
