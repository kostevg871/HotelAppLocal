using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HotelAppLocal.Data;
using HotelAppLocal.Views;

namespace HotelAppLocal.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _auth;
        private readonly RoomsPage _roomsPage;
        private readonly RegistrationPage _registrationPage;

        public event PropertyChangedEventHandler? PropertyChanged;

        public INavigation Navigation { get; set; } = null!;

        private string _username = "";
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _error = "";
        public string Error
        {
            get => _error;
            set { _error = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        public LoginViewModel(AuthService auth, RoomsPage roomsPage, RegistrationPage registrationPage)
        {
            _auth = auth;
            _roomsPage = roomsPage;
            _registrationPage = registrationPage;

            LoginCommand = new Command(async () => await LoginAsync());
            GoToRegisterCommand = new Command(async () => await Navigation.PushAsync(_registrationPage));
        }

        private async Task LoginAsync()
        {
            Error = "";

            var ok = await _auth.LoginAsync(Username, Password);
            if (!ok)
            {
                Error = "Неверный логин или пароль";
                return;
            }

            // заменяем навигационный стек
            Application.Current.MainPage = new NavigationPage(_roomsPage);
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
