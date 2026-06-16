using System.Globalization;

namespace Social_Network.Converters
{
    public class DateTimeCnverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not DateTime dt) return string.Empty;

            var diff = DateTime.UtcNow - dt.ToUniversalTime();

            if (diff.TotalMinutes < 1) return "только что";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} мин. назад";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} ч. назад";
            if (diff.TotalDays < 7) return $"{(int)diff.TotalDays} д. назад";

            return dt.ToString("d MMM yyyy", new CultureInfo("ru-RU"));
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}