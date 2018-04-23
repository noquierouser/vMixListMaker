using System;
using Windows.UI.Xaml.Data;

namespace vMixListMaker
{
    class StringFormatConverter : IValueConverter
    {
        //https://stackoverflow.com/questions/44756983/how-to-pass-static-resource-string-to-converterparameter-in-uwp
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            if (parameter == null)
                return value;

            return string.Format((string)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
