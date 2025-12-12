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

        // Текущий пользователь
        var user = _auth.CurrentUser;

        // Кнопка "Брони" — только для админа и регистратора
        var bookingsItem = this.ToolbarItems.FirstOrDefault(t => t.Text == "Брони");
        if (bookingsItem != null)
        {
            var isStaff = user != null &&
                          (user.Role == UserRole.Admin || user.Role == UserRole.Registrar);
            bookingsItem.IsEnabled = isStaff;
        }

        // Кнопка "Добавить" — только для админа
        var addItem = this.ToolbarItems.FirstOrDefault(t => t.Text == "Добавить");
        if (addItem != null)
        {
            var isAdmin = user?.Role == UserRole.Admin;
            addItem.IsEnabled = isAdmin;
        }

        // "Выход" — всегда доступен, если кто-то залогинен
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
            if (user == null ||
                !(user.Role == UserRole.Admin || user.Role == UserRole.Registrar))
            {
                await DisplayAlert("Доступ запрещён",
                    "Только администратор или регистратор могут управлять бронированиями.",
                    "OK");
                return;
            }

            await Navigation.PushAsync(_bookingsPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка",
                $"Не удалось открыть список бронирований:\n{ex.Message}",
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
