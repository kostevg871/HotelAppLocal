using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HotelAppLocal.Data;
using HotelAppLocal.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAppLocal.ViewModels
{
    public class BookingsManagementViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private readonly AuthService _auth;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<BookingItem> Bookings { get; } = new();

        public ICommand LoadCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public BookingsManagementViewModel(AppDbContext db, AuthService auth)
        {
            _db = db;
            _auth = auth;

            LoadCommand = new Command(async () => await LoadAsync());
            ConfirmCommand = new Command<BookingItem>(async item => await ConfirmAsync(item));
            CancelCommand = new Command<BookingItem>(async item => await CancelAsync(item));
        }

        private async Task LoadAsync()
        {
            Bookings.Clear();

            var list = await _db.Bookings
                .Include(b => b.Room)
                .OrderByDescending(b => b.FromDate)
                .ToListAsync();

            foreach (var b in list)
            {
                Bookings.Add(new BookingItem(b, _auth));
            }
        }

        private async Task ConfirmAsync(BookingItem? item)
        {
            if (item == null || !item.CanConfirm)
                return;

            item.Booking.Status = BookingStatus.Confirmed;
            item.Booking.HandledByUserId = _auth.CurrentUser?.Id;

            // Можно пометить комнату недоступной
            var room = item.Booking.Room;
            room.IsAvailable = false;
            _db.Rooms.Update(room);

            _db.Bookings.Update(item.Booking);
            await _db.SaveChangesAsync();

            item.Refresh();
        }

        private async Task CancelAsync(BookingItem? item)
        {
            if (item == null || !item.CanCancel)
                return;

            item.Booking.Status = BookingStatus.Cancelled;
            item.Booking.HandledByUserId = _auth.CurrentUser?.Id;

            // При отмене бронь освобождаем комнату
            var room = item.Booking.Room;
            room.IsAvailable = true;
            _db.Rooms.Update(room);

            _db.Bookings.Update(item.Booking);
            await _db.SaveChangesAsync();

            item.Refresh();
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Обёртка над Booking для удобного биндинга в UI
    /// </summary>
    public class BookingItem : INotifyPropertyChanged
    {
        private readonly AuthService _auth;

        public Booking Booking { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public BookingItem(Booking booking, AuthService auth)
        {
            Booking = booking;
            _auth = auth;
        }

        public string RoomNumber => Booking.Room?.Number ?? "-";

        public string GuestName => Booking.GuestName;

        public string GuestPhone => Booking.GuestPhone;

        public string DateRange =>
            $"{Booking.FromDate:dd.MM.yyyy} — {Booking.ToDate:dd.MM.yyyy}";

        public string StatusText => Booking.Status switch
        {
            BookingStatus.Pending => "Ожидает",
            BookingStatus.Confirmed => "Подтверждена",
            BookingStatus.Cancelled => "Отменена",
            _ => Booking.Status.ToString()
        };

        public string StatusColor => Booking.Status switch
        {
            BookingStatus.Pending => "#F97316",   // оранжевый
            BookingStatus.Confirmed => "#16A34A", // зелёный
            BookingStatus.Cancelled => "#6B7280", // серый
            _ => "#6B7280"
        };

        // Кнопки доступны только админу/регистратору
        private bool IsStaff =>
            _auth.CurrentUser != null &&
            (_auth.CurrentUser.Role == UserRole.Admin ||
             _auth.CurrentUser.Role == UserRole.Registrar);

        public bool CanConfirm =>
            IsStaff && Booking.Status == BookingStatus.Pending;

        public bool CanCancel =>
            IsStaff && Booking.Status != BookingStatus.Cancelled;

        public void Refresh()
        {
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(StatusColor));
            OnPropertyChanged(nameof(CanConfirm));
            OnPropertyChanged(nameof(CanCancel));
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
