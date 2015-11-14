using System;
using Windows.UI.Xaml.Data;

namespace GDD
{
    public class SizeSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double size = (long)value;
            string units = "bytes";

            if (size == 0)
                return "";

            while (size > 1024 || units == "GB")
            {
                size = size / 1024;
                units = bumpUnit(units);
            }
            return String.Format("{0:0.##} {1}", size, units);
        }

        private string bumpUnit(string units)
        {
            if (units == "bytes")
            {
                return "KB";
            }
            else if (units == "KB")
            {
                return "MB";
            }
            else if (units == "MB")
            {
                return "GB";
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
