using System;
using Windows.UI.Xaml.Data;

namespace GDD
{
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isDirectory = (bool)value;
            if (isDirectory)
            {
                return "Assets/icons/folder.png";
            }
            else
            {
                return "Assets/icons/file.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


    public class IsInteractiveImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isInteractive = (bool)value;
            if (isInteractive)
            {
                return "Assets/icons/folder.png";
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
    

    public class SizeSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double size = (long)value;
            string units = "bytes";
            while(size > 1024 || units == "GB")
            {
                size = size / 1024;
                units = bumpUnit(units);
            }
            return String.Format("{0:0.##} {1}", size, units);
        }

        private string bumpUnit(string units)
        {
            if(units == "bytes")
            {
                return "KB";
            }
            else if(units == "KB")
            {
                return "MB";
            }
            else if(units =="MB")
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
