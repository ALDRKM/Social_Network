using Microsoft.Extensions.DependencyInjection;

namespace Social_Network
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            int userId = Preferences.Default.Get(Constants.AppSettings.UserIdKey, 0);

            if (userId > 0)
                await Shell.Current.GoToAsync("//FeedPage");
            else
                await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}