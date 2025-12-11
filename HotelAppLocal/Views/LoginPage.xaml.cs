using HotelAppLocal.ViewModels;

namespace HotelAppLocal.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        vm.Navigation = Navigation;
        BindingContext = vm;
    }
}
