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

        // Можно захардкодить под .env docker-compose:
        var dbHost = "localhost";
        var dbPort = "3306";          // то же, что MYSQL_PORT в .env
        var dbName = "hotelapp";      // MYSQL_DATABASE
        var dbUser = "hoteluser";     // MYSQL_USER
        var dbPass = "student"; // MYSQL_PASSWORD

        var connectionString =
            $"server={dbHost};port={dbPort};database={dbName};user={dbUser};password={dbPass};TreatTinyAsBoolean=true;";

        // Регистрация контекста с MySQL
        builder.Services.AddSingleton<AppDbContext>(_ =>
        {
            var ctx = new AppDbContext(connectionString);
            DbInitializer.Initialize(ctx);
            return ctx;
        });

        builder.Services.AddTransient<ViewModels.RoomsViewModel>();
        builder.Services.AddTransient<Views.RoomsPage>();

        return builder.Build();
    }
}
