using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Social_Network.Service;
using Social_Network.ViewModels;
using Social_Network.Views;

namespace Social_Network
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
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
            builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>();
            builder.Services.AddSingleton<ISearchService, SearchService>();
            builder.Services.AddSingleton<ICommentService, CommentService>();
            builder.Services.AddSingleton<IPostService, PostService>();
            builder.Services.AddSingleton<ILikeService,LikeService>();
            builder.Services.AddSingleton<ISavedPostService, SavedPostService>();
            builder.Services.AddSingleton<IReportService, ReportService>();
            builder.Services.AddSingleton<IUserSettingsService, UserSettingsService>();
            builder.Services.AddSingleton<IImageUploadService, ImageUploadService>();


            //viewmodels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<FeedViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<OtherProfileViewModel>();
            builder.Services.AddTransient<PostDetailViewModel>();
            builder.Services.AddTransient<ChatsListViewModel>();
            builder.Services.AddTransient<ChatViewModel>();
            builder.Services.AddTransient<SearchViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<MyActionsViewModel>();
            builder.Services.AddTransient<CreatePostViewModel>();

            //pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<FeedPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<OtherProfilePage>();
            builder.Services.AddTransient<PostDetailPage>();
            builder.Services.AddTransient<ChatListPage>();
            builder.Services.AddTransient<ChatPage>();
            builder.Services.AddTransient<SearchPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<MyActionsPage>();
            builder.Services.AddTransient<CreatePostPage>();
            builder.Services.AddTransient<PrivacySettingsPage>();
            builder.Services.AddTransient<AccountSettingsPage>();
            builder.Services.AddTransient<ProfileSettingsPage>();
            builder.Services.AddTransient<ReportsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
