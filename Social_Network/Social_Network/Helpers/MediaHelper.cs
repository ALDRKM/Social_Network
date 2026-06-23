using Social_Network.Constants;

namespace Social_Network.Helpers
{
    // Превращает сохранённый путь картинки в полный URL.
    // - http(s) — внешние ссылки (picsum/pravatar) — как есть
    // - "/uploads/..." — наши загруженные файлы — добавляем хост API (работает с любого устройства)
    // - локальный путь — как есть (легаси/тот же девайс)
    public static class MediaHelper
    {
        public static string? Resolve(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return url;
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return url;
            if (url.StartsWith("/")) return $"http://{ApiConfig.Host}{url}";
            return url;
        }
    }
}
