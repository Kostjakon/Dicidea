using System;
using System.Globalization;
using System.Windows.Data;

namespace Dicidea.Core.Converters
{
    /// <summary>
    ///     Gibt vom übergebenen String (z.B. Name) den Anfangsbuchstaben als Großbuchstaben zurück.
    /// </summary>
    public class NameToInitialConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && s.Length > 0)
                return s.Substring(0, 1).ToUpper();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
