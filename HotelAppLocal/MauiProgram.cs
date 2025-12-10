using HotelAppLocal;
using HotelAppLocal.Data;

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

        // путь к файлу базы данных
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rooms.db");

        // регистрируем DbContext как Singleton
        builder.Services.AddSingleton<AppDbContext>(_ =>
        {
            var ctx = new AppDbContext(dbPath);
            DbInitializer.Initialize(ctx); // один раз создаём БД и тестовые данные
            return ctx;
        });

        // ViewModel и Page для списка комнат добавим ниже:
        builder.Services.AddTransient<ViewModels.RoomsViewModel>();
        builder.Services.AddTransient<Views.RoomsPage>();

        return builder.Build();
    }
}
