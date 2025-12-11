using HotelAppLocal.Models;

namespace HotelAppLocal.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Для разработки: каждый запуск пересоздаём схему
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // --- Комнаты ---
            var rooms = new[]
            {
                new Room { Number = "101", IsAvailable = true,  PricePerNight = 4500 },
                new Room { Number = "102", IsAvailable = true,  PricePerNight = 5200 },
                new Room { Number = "201", IsAvailable = false, PricePerNight = 6000 },
                new Room { Number = "202", IsAvailable = true,  PricePerNight = 5800 },
            };

            context.Rooms.AddRange(rooms);

            // --- Пользователи системы ---
            var admin = new User
            {
                Username = "admin",
                PasswordHash = "admin123",      // для демо
                Role = UserRole.Admin
            };

            var registrar = new User
            {
                Username = "reg",
                PasswordHash = "reg123",
                Role = UserRole.Registrar
            };

            context.Users.AddRange(admin, registrar);

            context.SaveChanges();
        }
    }
}
