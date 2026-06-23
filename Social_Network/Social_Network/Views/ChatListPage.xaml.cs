using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class ChatListPage : ContentPage
{
    private readonly ChatsListViewModel _vm;

    public ChatListPage(ChatsListViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadChatsCommand.ExecuteAsync(null);
    }
}
