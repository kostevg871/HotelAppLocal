using HotelAppLocal.Data;
using HotelAppLocal.Models;
using HotelAppLocal.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace HotelAppLocal.Views;

public partial class RoomsPage : ContentPage
{
    private readonly RoomsViewModel _viewModel;
    private readonly AppDbContext _db;
    private readonly BookingsManagementPage _bookingsPage;
    private readonly AuthService _auth;
    private readonly IServiceProvider _services;

    public RoomsPage(
        RoomsViewModel vm,
        AppDbContext db,
        BookingsManagementPage bookingsPage,
        AuthService auth,
        IServiceProvider services)
    {
        InitializeComponent();

        _viewModel = vm;
        _db = db;
        _bookingsPage = bookingsPage;
        _auth = auth;
        _services = services;

        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.LoadCommand.CanExecute(null))
            _viewModel.LoadCommand.Execute(null);

        var user = _auth.CurrentUser;

        // "Брони" — доступна всем залогиненным пользователям
        var bookingsItem = this.ToolbarItems.FirstOrDefault(t => t.Text == "Брони");
        if (bookingsItem != null)
        {
            bookingsItem.IsEnabled = user != null;
        }

        // "Добавить" — только для админа
        var addItem = this.ToolbarItems.FirstOrDefault(t => t.Text == "Добавить");
        if (addItem != null)
        {
            addItem.IsEnabled = user?.Role == UserRole.Admin;
        }

        // "Выход" — если кто-то залогинен
        var logoutItem = this.ToolbarItems.FirstOrDefault(t => t.Text == "Выход");
        if (logoutItem != null)
        {
            logoutItem.IsEnabled = user != null;
        }
    }

    private async void OnBookClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button btn && btn.CommandParameter is Room room)
            {
                // Любой залогиненный пользователь может бронировать
                if (_auth.CurrentUser == null)
                {
                    await DisplayAlert("Авторизация",
                        "Для бронирования нужно войти в систему.",
                        "OK");
                    return;
                }

                await Navigation.PushAsync(new BookingPage(room, _db));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка",
                $"Не удалось открыть страницу бронирования:\n{ex.Message}",
                "OK");
        }
    }

    private async void OnBookingsClicked(object sender, EventArgs e)
    {
        try
        {
            var user = _auth.CurrentUser;
            if (user == null)
            {
                await DisplayAlert("Авторизация",
                    "Для просмотра бронирований нужно войти в систему.",
                    "OK");
                return;
            }

            if (user.Role == UserRole.Admin || user.Role == UserRole.Registrar)
            {
                // Страница управления всеми брони
                await Navigation.PushAsync(_bookingsPage);
            }
            else
            {
                // Обычный пользователь – "Мои бронирования"
                var myBookingsPage = _services.GetRequiredService<MyBookingsPage>();
                await Navigation.PushAsync(myBookingsPage);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка",
                $"Не удалось открыть бронирования:\n{ex.Message}",
                "OK");
        }
    }


    private async void OnAddRoomClicked(object sender, EventArgs e)
    {
        try
        {
            var user = _auth.CurrentUser;
            if (user?.Role != UserRole.Admin)
            {
                await DisplayAlert("Недостаточно прав",
                    "Только администратор может добавлять новые комнаты.",
                    "OK");
                return;
            }

            var newRoomPage = _services.GetRequiredService<NewRoomPage>();
            await Navigation.PushAsync(newRoomPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка",
                $"Не удалось открыть форму создания номера:\n{ex.Message}",
                "OK");
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        try
        {
            _auth.Logout();

            var loginPage = _services.GetRequiredService<LoginPage>();
            Application.Current.MainPage = new NavigationPage(loginPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка",
                $"Не удалось выйти из профиля:\n{ex.Message}",
                "OK");
        }
    }
}
