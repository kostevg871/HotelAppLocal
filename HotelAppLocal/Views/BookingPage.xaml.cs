using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HotelAppLocal.Data;
using HotelAppLocal.Models;

namespace HotelAppLocal.Views;

public partial class BookingPage : ContentPage
{
    private readonly AppDbContext _db;
    private readonly Room _room;
    private readonly BookingViewModel _viewModel;

    public BookingPage(Room room, AppDbContext db)
    {
        InitializeComponent();

        _room = room;
        _db = db;

        _viewModel = new BookingViewModel(_db, _room);
        BindingContext = _viewModel;
    }

    // Простейшая VM для этой страницы
    private class BookingViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private readonly Room _room;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string RoomNumber => $"Номер {_room.Number}";
        public string PriceText => $"{_room.PricePerNight:N0} ₽ / ночь";

        private string _guestName = string.Empty;
        public string GuestName
        {
            get => _guestName;
            set { _guestName = value; OnPropertyChanged(); }
        }

        private DateTime _fromDate = DateTime.Today;
        public DateTime FromDate
        {
            get => _fromDate;
            set { _fromDate = value; OnPropertyChanged(); }
        }

        private DateTime _toDate = DateTime.Today.AddDays(1);
        public DateTime ToDate
        {
            get => _toDate;
            set { _toDate = value; OnPropertyChanged(); }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }

        public BookingViewModel(AppDbContext db, Room room)
        {
            _db = db;
            _room = room;
            SaveCommand = new Command(async () => await SaveAsync());
        }

        private async Task SaveAsync()
        {
            StatusMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(GuestName))
            {
                StatusMessage = "Введите имя гостя.";
                return;
            }

            if (ToDate <= FromDate)
            {
                StatusMessage = "Дата выезда должна быть позже даты заезда.";
                return;
            }

            // Проверка пересечения бронирований для этой комнаты
            bool hasOverlap = _db.Bookings.Any(b =>
                b.RoomId == _room.Id &&
                (
                    (FromDate >= b.FromDate && FromDate < b.ToDate) ||
                    (ToDate > b.FromDate && ToDate <= b.ToDate) ||
                    (FromDate <= b.FromDate && ToDate >= b.ToDate)
                ));

            if (hasOverlap)
            {
                StatusMessage = "На выбранные даты номер уже забронирован.";
                return;
            }

            var booking = new Booking
            {
                RoomId = _room.Id,
                GuestName = GuestName.Trim(),
                FromDate = FromDate,
                ToDate = ToDate
            };

            _db.Bookings.Add(booking);

            // Можем пометить комнату как недоступную
            _room.IsAvailable = false;
            _db.Rooms.Update(_room);

            await _db.SaveChangesAsync();

            StatusMessage = "Бронь успешно создана!";

            // TODO: можно сделать навигацию назад через пару секунд
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
