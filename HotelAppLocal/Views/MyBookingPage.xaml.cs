using System.Collections.ObjectModel;
using HotelAppLocal.Data;
using HotelAppLocal.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAppLocal.Views;

public partial class MyBookingsPage : ContentPage
{
    private readonly AppDbContext _db;
    private readonly AuthService _auth;

    public ObservableCollection<MyBookingItem> Bookings { get; } = new();

    public MyBookingsPage(AppDbContext db, AuthService auth)
    {
        InitializeComponent();
        _db = db;
        _auth = auth;

        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Bookings.Clear();

        var user = _auth.CurrentUser;
        if (user == null)
        {
            EmptyLabel.Text = "Для просмотра бронирований войдите в систему.";
            EmptyLabel.IsVisible = true;
            BookingsView.IsVisible = false;
            return;
        }

        var bookings = await _db.Bookings
            .Include(b => b.Room)
            .Where(b => b.CreatedByUserId == user.Id)
            .OrderByDescending(b => b.FromDate)
            .ToListAsync();

        if (!bookings.Any())
        {
            EmptyLabel.Text = "У вас пока нет бронирований.";
            EmptyLabel.IsVisible = true;
            BookingsView.IsVisible = false;
            return;
        }

        foreach (var b in bookings)
        {
            var (statusText, statusColor) = MapStatus(b.Status);

            Bookings.Add(new MyBookingItem
            {
                RoomNumber = $"Номер {b.Room.Number}",
                DatesText = $"{b.FromDate:dd.MM.yyyy} – {b.ToDate:dd.MM.yyyy}",
                StatusText = statusText,
                StatusColor = statusColor
            });
        }

        EmptyLabel.IsVisible = false;
        BookingsView.IsVisible = true;
    }

    private static (string text, Color color) MapStatus(BookingStatus status)
    {
        return status switch
        {
            BookingStatus.Pending => ("Ожидает подтверждения", Color.FromArgb("#F97316")), // оранжевый
            BookingStatus.Confirmed => ("Подтверждена", Color.FromArgb("#16A34A")), // зелёный
            BookingStatus.Cancelled => ("Отменена", Color.FromArgb("#6B7280")), // серый
            _ => ("Неизвестный статус", Color.FromArgb("#DC2626")),
        };
    }

    public class MyBookingItem
    {
        public string RoomNumber { get; set; } = "";
        public string DatesText { get; set; } = "";
        public string StatusText { get; set; } = "";
        public Color StatusColor { get; set; }
    }
}
