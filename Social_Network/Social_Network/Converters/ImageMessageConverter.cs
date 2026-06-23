using System.Globalization;
using Social_Network.Helpers;

namespace Social_Network.Converters
{
    // Определяет, является ли текст сообщения картинкой (по расширению пути/URL).
    // ConverterParameter: "source" -> путь или null; "isimage" -> bool; "istext" -> bool (инверсия)
    public class ImageMessageConverter : IValueConverter
    {
        private static readonly string[] Ext = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };

        private static bool LooksLikeImage(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var lower = s.Trim().ToLowerInvariant();
            foreach (var e in Ext)
                if (lower.EndsWith(e)) return true;
            return false;
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var s = value as string;
            bool isImg = LooksLikeImage(s);
            var mode = (parameter as string)?.ToLowerInvariant() ?? "isimage";

            return mode switch
            {
                "source" => isImg ? MediaHelper.Resolve(s) : null,
                "istext" => !isImg,
                _ => isImg,
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
