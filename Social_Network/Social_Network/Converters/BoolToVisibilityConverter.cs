using System.Globalization;

namespace Social_Network.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool b = value is bool v && v;
            bool invert = parameter is string s && s.Equals("invert", StringComparison.OrdinalIgnoreCase);
            return invert ? !b : b;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}