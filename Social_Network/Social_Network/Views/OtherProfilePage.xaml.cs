using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class OtherProfilePage : ContentPage
{
    public OtherProfilePage(OtherProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
