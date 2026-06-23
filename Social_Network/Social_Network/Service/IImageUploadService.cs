namespace Social_Network.Service
{
    public interface IImageUploadService
    {
        // Загружает файл на сервер, возвращает относительный URL ("/uploads/..") или null
        Task<string?> UploadAsync(Stream stream, string fileName);
    }
}
