using System.Globalization;

namespace Social_Network.Converters
{
    // Время сообщения: ≤1 мин — «только что», <1 ч — минуты назад,
    // <24 ч — часы назад, иначе — дата и время без секунд
    public class MessageTimeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not DateTime dt) return string.Empty;
            var local = dt.ToLocalTime();
            var diff = DateTime.Now - local;

            if (diff.TotalMinutes < 1) return "только что";
            if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes} мин. назад";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} ч. назад";
            return local.ToString("dd.MM.yyyy HH:mm", new CultureInfo("ru-RU"));
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
