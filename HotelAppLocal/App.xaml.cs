using HotelAppLocal.Views;

namespace HotelAppLocal;

public partial class App : Application
{
    public App(LoginPage loginPage)
    {
        InitializeComponent();

        MainPage = new NavigationPage(loginPage);
    }
}
