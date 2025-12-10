using HotelAppLocal.ViewModels;

namespace HotelAppLocal.Views;

public partial class RoomsPage : ContentPage
{
    private readonly RoomsViewModel _viewModel;

    public RoomsPage(RoomsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.LoadCommand.CanExecute(null))
            _viewModel.LoadCommand.Execute(null);
    }
}
