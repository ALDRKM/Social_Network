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

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        try
        {
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
                await Shell.Current.Navigation.PopAsync(false);
            else
                await Shell.Current.GoToAsync("SettingsPage");
        }
        catch (InvalidOperationException)
        {
            await Shell.Current.GoToAsync("SettingsPage");
        }
    }
}
