using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class ReportsPage : ContentPage
{
    private readonly SettingsViewModel _vm;

    public ReportsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadReportsCommand.ExecuteAsync(null);
    }
}
