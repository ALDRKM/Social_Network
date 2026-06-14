using Microsoft.Extensions.Logging;
using Social_Network.Service;
using Social_Network.ViewModels;

namespace Social_Network
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            //http client
            builder.Services.AddSingleton<HttpClient>();

            //services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IChatHubService, ChatHubService>();
            builder.Services.AddSingleton<IMessageService, MessageService>();
            builder.Services.AddSingleton<IChatService, ChatService>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<ISearchService, SearchService>();
            builder.Services.AddSingleton<ICommentService, CommentService>();
            builder.Services.AddSingleton<IPostService, PostService>();


            //viewmodels
            builder.Services.AddTransient<LoginViewModel>();



#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
