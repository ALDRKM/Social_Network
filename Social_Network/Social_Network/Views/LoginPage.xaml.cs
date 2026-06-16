namespace Social_Network.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(ViewModels.LoginViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}