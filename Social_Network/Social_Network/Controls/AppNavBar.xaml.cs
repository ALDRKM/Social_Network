namespace Social_Network.Controls
{
    public partial class AppNavBar : ContentView
    {
        // "Sidebar" — боковая панель (десктоп), "Bottom" — нижний бар (телефон)
        public static readonly BindableProperty ModeProperty =
            BindableProperty.Create(nameof(Mode), typeof(string), typeof(AppNavBar), "Bottom",
                propertyChanged: (b, o, n) => ((AppNavBar)b).ApplyMode());

        // Активный пункт: Feed | Search | Create | Chat | Profile
        public static readonly BindableProperty CurrentProperty =
            BindableProperty.Create(nameof(Current), typeof(string), typeof(AppNavBar), "Feed",
                propertyChanged: (b, o, n) => ((AppNavBar)b).ApplyHighlight());

        public AppNavBar()
        {
            InitializeComponent();
            ApplyMode();
            ApplyHighlight();
        }

        public string Mode
        {
            get => (string)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public string Current
        {
            get => (string)GetValue(CurrentProperty);
            set => SetValue(CurrentProperty, value);
        }

        private void ApplyMode()
        {
            bool sidebar = string.Equals(Mode, "Sidebar", StringComparison.OrdinalIgnoreCase);
            Sidebar.IsVisible = sidebar;
            BottomBar.IsVisible = !sidebar;
            ApplyHighlight();
        }

        private void ApplyHighlight()
        {
            var active = Color.FromArgb("#C8702A");      // оранжевая подсветка
            var inactive = Colors.Transparent;

            // Боковая панель
            SetBg(Sidebar.IsVisible, SFeed, Current == "Feed", active, inactive);
            SetBg(Sidebar.IsVisible, SSearch, Current == "Search", active, inactive);
            SetBg(Sidebar.IsVisible, SChat, Current == "Chat", active, inactive);
            SetBg(Sidebar.IsVisible, SCreate, Current == "Create", active, inactive);
            SetBg(Sidebar.IsVisible, SProfile, Current == "Profile", active, inactive);

            // Нижний бар
            SetBg(BottomBar.IsVisible, BFeed, Current == "Feed", active, inactive);
            SetBg(BottomBar.IsVisible, BSearch, Current == "Search", active, inactive);
            SetBg(BottomBar.IsVisible, BChat, Current == "Chat", active, inactive);
            SetBg(BottomBar.IsVisible, BCreate, Current == "Create", active, inactive);
            SetBg(BottomBar.IsVisible, BProfile, Current == "Profile", active, inactive);
        }

        private static void SetBg(bool layoutVisible, Border item, bool isActive, Color active, Color inactive)
        {
            item.BackgroundColor = isActive ? active : inactive;
        }

        private async void OnFeed(object? sender, EventArgs e)
        {
            if (Current == "Feed") return;
            await Shell.Current.GoToAsync("//FeedPage");
        }

        private async void OnSearch(object? sender, EventArgs e)
        {
            if (Current == "Search") return;
            await Shell.Current.GoToAsync("//SearchPage");
        }

        private async void OnCreate(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("CreatePostPage");

        private async void OnChat(object? sender, EventArgs e)
        {
            if (Current == "Chat") return;
            await Shell.Current.GoToAsync("//ChatListPage");
        }

        private async void OnProfile(object? sender, EventArgs e)
        {
            // Всегда ведёт на свой профиль (в т.ч. находясь на чужом)
            await Shell.Current.GoToAsync("//ProfilePage");
        }

        private async void OnSettings(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("SettingsPage");
    }
}
