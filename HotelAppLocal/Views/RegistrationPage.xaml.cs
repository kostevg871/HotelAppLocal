using HotelAppLocal.ViewModels;

namespace HotelAppLocal.Views;

public partial class RegistrationPage : ContentPage
{
    public RegistrationPage(RegistrationViewModel vm)
    {
        InitializeComponent();
        vm.Navigation = Navigation;
        BindingContext = vm;
    }
}
