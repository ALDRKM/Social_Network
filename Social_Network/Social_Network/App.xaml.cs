using Microsoft.Extensions.DependencyInjection;

namespace Social_Network
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Глобальные обработчики — логируем ошибки в файл, чтобы приложение не падало молча
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                Log("UnhandledException", e.ExceptionObject as Exception);
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Log("UnobservedTaskException", e.Exception);
                e.SetObserved();
            };

            MainPage = new AppShell();
        }

        public static void Log(string source, Exception? ex)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {source}: {ex}{Environment.NewLine}{Environment.NewLine}";
            try { File.AppendAllText(Path.Combine(FileSystem.AppDataDirectory, "crash.log"), line); } catch { }
            try { File.AppendAllText(Path.Combine(Path.GetTempPath(), "socialnet_crash.log"), line); } catch { }
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
