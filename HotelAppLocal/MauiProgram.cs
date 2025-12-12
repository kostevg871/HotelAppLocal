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


        string dbHost;

        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            if (DeviceInfo.DeviceType == DeviceType.Virtual)
            {
                // ANDROID ЭМУЛЯТОР
                dbHost = "10.0.2.2";
            }
            else
            {
                // ФИЗИЧЕСКОЕ УСТРОЙСТВО ПО USB + adb reverse
                // adb reverse tcp:3306 tcp:3306
                dbHost = "127.0.0.1";
            }
        }
        else
        {
            // Windows / iOS / т.д. – просто localhost
            dbHost = "localhost";
        }

        // Можно захардкодить под .env docker-compose:
        var dbPort = "3306";          // то же, что MYSQL_PORT в .env
        var dbName = "hotelapp";      // MYSQL_DATABASE
        var dbUser = "hoteluser";     // MYSQL_USER
        var dbPass = "student";         // MYSQL_PASSWORD

        var connectionString =
            $"server={dbHost};port={dbPort};database={dbName};user={dbUser};password={dbPass};TreatTinyAsBoolean=true;";

        // Сам контекст БД — БЕЗ инициализации
        builder.Services.AddSingleton<AppDbContext>(_ =>
        {
            var ctx = new AppDbContext(connectionString);
            return ctx;
        });

        // Сервис проверки соединения и инициализации
        builder.Services.AddSingleton<DbHealthService>();

        builder.Services.AddSingleton<AuthService>();

        // ViewModels
        builder.Services.AddTransient<RoomsViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegistrationViewModel>();
        builder.Services.AddTransient<BookingsManagementViewModel>();

        // Pages
        builder.Services.AddTransient<RoomsPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegistrationPage>();
        builder.Services.AddTransient<BookingsManagementPage>();
        builder.Services.AddTransient<StartupPage>();

        return builder.Build();
    }
}
