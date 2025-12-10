using HotelAppLocal.Data;
using HotelAppLocal.Models;

namespace HotelAppLocal.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // создаёт базу и таблицы, если их ещё нет
            context.Database.EnsureCreated();

            // если в базе уже есть комнаты – ничего не делаем
            if (context.Rooms.Any())
                return;

            var rooms = new[]
            {
                new Room { Number = "101", IsAvailable = true },
                new Room { Number = "102", IsAvailable = true },
                new Room { Number = "201", IsAvailable = false }, // для примера одна недоступна
                new Room { Number = "202", IsAvailable = true },
            };

            context.Rooms.AddRange(rooms);
            context.SaveChanges();
        }
    }
}
