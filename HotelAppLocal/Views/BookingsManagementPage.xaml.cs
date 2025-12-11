using HotelAppLocal.ViewModels;

namespace HotelAppLocal.Views;

public partial class BookingsManagementPage : ContentPage
{
    private readonly BookingsManagementViewModel _vm;

    public BookingsManagementPage(BookingsManagementViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_vm.LoadCommand.CanExecute(null))
            _vm.LoadCommand.Execute(null);
    }
}
