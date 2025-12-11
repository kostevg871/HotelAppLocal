using HotelAppLocal.Data;
using HotelAppLocal.Models;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var rooms = new[]
        {
            new Room { Number = "101", IsAvailable = true,  PricePerNight = 4500 },
            new Room { Number = "102", IsAvailable = true,  PricePerNight = 4500 },
            new Room { Number = "103", IsAvailable = false,  PricePerNight = 4500 },
            new Room { Number = "104", IsAvailable = true,  PricePerNight = 4500 },
            // ...
        };

        context.Rooms.AddRange(rooms);
        context.SaveChanges();
    }
}
