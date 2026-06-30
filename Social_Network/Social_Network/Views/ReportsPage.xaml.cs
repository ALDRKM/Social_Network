using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class ReportsPage : ContentPage
{
    private readonly SettingsViewModel _vm;
    private bool _isNavigatingBack;

    public ReportsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _isNavigatingBack = false;
        await _vm.LoadReportsCommand.ExecuteAsync(null);
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await NavigateBack();
    }

    protected override bool OnBackButtonPressed()
    {
        _ = NavigateBack();
        return true;
    }

    private async Task NavigateBack()
    {
        if (_isNavigatingBack) return;

        _isNavigatingBack = true;
        FromDatePicker.Unfocus();
        ToDatePicker.Unfocus();
        PeriodDates.IsVisible = false;

        await Task.Delay(50);
        await Shell.Current.GoToAsync("..", false);
    }
}
