using HotelAppLocal.ViewModels;
using HotelAppLocal.Data;
using HotelAppLocal.Models;

namespace HotelAppLocal.Views;

public partial class RoomsPage : ContentPage
{
    private readonly RoomsViewModel _viewModel;
    private readonly AppDbContext _db;

    public RoomsPage(RoomsViewModel viewModel, AppDbContext db)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _db = db;

        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.LoadCommand.CanExecute(null))
            _viewModel.LoadCommand.Execute(null);
    }

    private async void OnBookClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Room room)
        {
            await Navigation.PushAsync(new BookingPage(room, _db));
        }
    }
}
