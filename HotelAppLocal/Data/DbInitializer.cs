using HotelAppLocal.Models;

namespace HotelAppLocal.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Каждый запуск: пересоздаём схему
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var rooms = new[]
            {
                new Room { Number = "101", IsAvailable = true,  PricePerNight = 4500 },
                new Room { Number = "102", IsAvailable = true,  PricePerNight = 5200 },
                new Room { Number = "201", IsAvailable = false, PricePerNight = 6000 },
                new Room { Number = "202", IsAvailable = true,  PricePerNight = 5800 },
            };

            context.Rooms.AddRange(rooms);

            // пока бронирования не добавляем, будут создаваться из приложения
            context.SaveChanges();
        }
    }
}
