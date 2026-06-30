using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class CreatePostPage : ContentPage
{
    public CreatePostPage(CreatePostViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
