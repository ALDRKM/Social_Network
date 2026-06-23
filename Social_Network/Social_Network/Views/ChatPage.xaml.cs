using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class ChatPage : ContentPage
{
    private readonly ChatViewModel _vm;

    public ChatPage(ChatViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _vm.CleanupAsync();
    }
}
