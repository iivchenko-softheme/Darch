// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Globalization;
using System.Windows.Data;

namespace Darch.GUI.Wpf.Converters
{
    [ValueConversion(typeof(long), typeof(string))]
    public sealed class UnitConverter : IValueConverter
    {
        private static readonly string[] Units = { "bytes", "KB", "MB", "GB", "TB" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var i = 0;
            var tmp = (float)(ulong)value;
            for (; i < Units.Length - 1; ++i)
            {
                if (tmp < 1024)
                {
                    break;
                }

                tmp /= 1024;
            }

            return tmp - ((long)tmp) > 0
                ? string.Format(CultureInfo.InstalledUICulture, "{0:f2} {1}", tmp, Units[i])
                : string.Format(CultureInfo.InstalledUICulture, "{0} {1}", (long)tmp, Units[i]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
