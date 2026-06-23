using System.Globalization;
using Social_Network.Helpers;

namespace Social_Network.Converters
{
    // Для привязок Image.Source: возвращает полный URL картинки
    public class MediaUrlConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => MediaHelper.Resolve(value as string);

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
