using Microsoft.EntityFrameworkCore;
using HotelAppLocal.Models;

namespace HotelAppLocal.Data
{
    public class AppDbContext : DbContext
    {
        private readonly string _connectionString;

        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Booking> Bookings => Set<Booking>();   // ← добавили

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var serverVersion = ServerVersion.AutoDetect(_connectionString);
            optionsBuilder.UseMySql(_connectionString, serverVersion);
        }
    }
}
