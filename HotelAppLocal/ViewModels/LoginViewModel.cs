using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HotelAppLocal.Data;
using HotelAppLocal.Views;
using Microsoft.Extensions.DependencyInjection;

namespace HotelAppLocal.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _auth;

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

        public LoginViewModel(AuthService auth)
        {
            _auth = auth;

            LoginCommand = new Command(async () => await LoginAsync());
            GoToRegisterCommand = new Command(async () => await GoToRegisterAsync());
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

            // Берём RoomsPage через DI только сейчас
            var services = App.Current?.Handler?.MauiContext?.Services;
            if (services is null)
            {
                Error = "Внутренняя ошибка: сервисы не доступны.";
                return;
            }

            var roomsPage = services.GetRequiredService<RoomsPage>();
            Application.Current.MainPage = new NavigationPage(roomsPage);
        }

        private async Task GoToRegisterAsync()
        {
            var services = App.Current?.Handler?.MauiContext?.Services;
            if (services is null)
            {
                Error = "Внутренняя ошибка: сервисы не доступны.";
                return;
            }

            var registrationPage = services.GetRequiredService<RegistrationPage>();
            await Navigation.PushAsync(registrationPage);
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
