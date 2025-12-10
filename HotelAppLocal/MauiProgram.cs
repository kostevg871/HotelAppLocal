using HotelAppLocal.Data;
using HotelAppLocal.ViewModels;
using HotelAppLocal.Views;

namespace HotelAppLocal;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rooms.db");

        // DbContext
        builder.Services.AddSingleton<AppDbContext>(_ =>
        {
            var ctx = new AppDbContext(dbPath);
            DbInitializer.Initialize(ctx);
            return ctx;
        });

        // ViewModel + Page
        builder.Services.AddTransient<RoomsViewModel>();
        builder.Services.AddTransient<RoomsPage>();

        return builder.Build();
    }
}
