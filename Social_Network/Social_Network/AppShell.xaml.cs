namespace Social_Network
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("OtherProfilePage", typeof(Views.OtherProfilePage));
            Routing.RegisterRoute("PostDetailPage", typeof(Views.PostDetailPage));
            Routing.RegisterRoute("ChatPage", typeof(Views.ChatPage));
            Routing.RegisterRoute("SettingsPage", typeof(Views.SettingsPage));
            Routing.RegisterRoute("CreatePostPage", typeof(Views.CreatePostPage));
            Routing.RegisterRoute("MyActionsPage", typeof(Views.MyActionsPage));
            Routing.RegisterRoute("PrivacySettingsPage", typeof(Views.PrivacySettingsPage));
            Routing.RegisterRoute("AccountSettingsPage", typeof(Views.AccountSettingsPage));
            Routing.RegisterRoute("ProfileSettingsPage", typeof(Views.ProfileSettingsPage));
            Routing.RegisterRoute("ReportsPage", typeof(Views.ReportsPage));
        }
    }
}