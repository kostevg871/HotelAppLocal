using HotelAppLocal.Data;
using HotelAppLocal.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAppLocal.Views;

public partial class NewRoomPage : ContentPage
{
    private readonly AppDbContext _db;
    private readonly AuthService _auth;

    public NewRoomPage(AppDbContext db, AuthService auth)
    {
        InitializeComponent();
        _db = db;
        _auth = auth;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        ErrorLabel.Text = "";

        if (_auth.CurrentUser?.Role != UserRole.Admin)
        {
            await DisplayAlert("Недостаточно прав",
                "Только администратор может добавлять новые комнаты.",
                "OK");
            return;
        }

        var number = RoomNumberEntry.Text?.Trim();
        var priceText = PriceEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(number))
        {
            ErrorLabel.Text = "Укажите номер комнаты.";
            return;
        }

        if (!int.TryParse(priceText, out var price) || price <= 0)
        {
            ErrorLabel.Text = "Укажите корректную целую цену за ночь (больше 0).";
            return;
        }

        // Проверим, что такого номера ещё нет
        var exists = await _db.Rooms.AnyAsync(r => r.Number == number);
        if (exists)
        {
            ErrorLabel.Text = "Комната с таким номером уже существует.";
            return;
        }

        var room = new Room
        {
            Number = number,
            PricePerNight = price, // int
            IsAvailable = true
        };

        _db.Rooms.Add(room);
        await _db.SaveChangesAsync();

        await DisplayAlert("Готово", "Новый номер успешно создан.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
