using HotelAppLocal.Views;

namespace HotelAppLocal;

public partial class App : Application
{
    public App(StartupPage startupPage)
    {
        InitializeComponent();

        MainPage = new NavigationPage(startupPage);
    }
}
