
namespace Social_Network.Constants
{
    public static class ApiConfig
    {
#if ANDROID
        public const string Host = "http://10.0.2.2:5043/";
#else
        private const string Host = "localhost:5043";
#endif

        public const string BaseUrl = $"http://{Host}/api";
        public const string HubUrl = $"http://{Host}/hubs/chat";
    }
}
