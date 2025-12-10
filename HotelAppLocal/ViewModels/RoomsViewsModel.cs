using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using HotelAppLocal.Data;
using HotelAppLocal.Models;

namespace HotelAppLocal.ViewModels
{
    public class RoomsViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Room> Rooms { get; } = new();

        public ICommand LoadCommand { get; }

        public RoomsViewModel(AppDbContext db)
        {
            _db = db;
            LoadCommand = new Command(async () => await LoadAsync());
        }

        private async Task LoadAsync()
        {
            Rooms.Clear();

            var rooms = await _db.Rooms
                .Where(r => r.IsAvailable)
                .OrderBy(r => r.Number)
                .ToListAsync();

            foreach (var room in rooms)
                Rooms.Add(room);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
