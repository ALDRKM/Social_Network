using System.Globalization;

namespace Social_Network.Converters
{
    // Возвращает true, только если все переданные значения == true
    public class AllTrueConverter : IMultiValueConverter
    {
        public object Convert(object?[]? values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null) return false;
            foreach (var v in values)
                if (v is not bool b || !b)
                    return false;
            return true;
        }

        public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
