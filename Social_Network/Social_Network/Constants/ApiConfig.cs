
namespace Social_Network.Constants
{
    public static class ApiConfig
    {
#if ANDROID
        // В эмуляторе Android хост-машина доступна по адресу 10.0.2.2
        public const string Host = "10.0.2.2:5043";
#else
        public const string Host = "localhost:5043";
#endif

        public const string BaseUrl = $"http://{Host}/api";
        public const string HubUrl = $"http://{Host}/hubs/chat";
    }
}
