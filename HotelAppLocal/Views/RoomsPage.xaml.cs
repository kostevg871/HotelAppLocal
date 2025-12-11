using HotelAppLocal.Data;
using HotelAppLocal.Models;
using HotelAppLocal.ViewModels;

namespace HotelAppLocal.Views;

public partial class RoomsPage : ContentPage
{
    private readonly RoomsViewModel _viewModel;
    private readonly AppDbContext _db;
    private readonly BookingsManagementPage _bookingsPage;
    private readonly AuthService _auth;

    public RoomsPage(RoomsViewModel vm, AppDbContext db, BookingsManagementPage bookingsPage, AuthService auth)
    {
        InitializeComponent();
        _viewModel = vm;
        _db = db;
        _bookingsPage = bookingsPage;
        _auth = auth;

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
            await Navigation.PushAsync(new BookingPage(room, _db)); // твой существующий BookingPage
        }
    }

    private async void OnBookingsClicked(object sender, EventArgs e)
    {
        if (_auth.CurrentUser == null ||
            !(_auth.CurrentUser.Role == UserRole.Admin ||
              _auth.CurrentUser.Role == UserRole.Registrar))
        {
            await DisplayAlert("Доступ запрещён",
                "Только администратор или регистратор могут управлять бронированиями.",
                "OK");
            return;
        }

        await Navigation.PushAsync(_bookingsPage);
    }
}
