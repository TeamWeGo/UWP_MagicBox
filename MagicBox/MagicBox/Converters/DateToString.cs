using System;
using System.Xml;
using Windows.UI.Xaml.Data;

namespace MagicBox.Converters
{
    public sealed class DateToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((DateTimeOffset)value).Date.ToString("yyyy-MM-dd");
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return XmlConvert.ToDateTimeOffset(value as string);
        }
    }
}
