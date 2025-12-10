using HotelAppLocal.Views;

namespace HotelAppLocal;

public partial class App : Application
{
    public App(RoomsPage roomsPage)
    {
        InitializeComponent();

        MainPage = new NavigationPage(roomsPage);
    }
}
