using System;
using System.Xml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace MagicBox.Converters
{
    public sealed class UriToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new BitmapImage(value as Uri);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value as BitmapImage).UriSource;
        }
    }
}
