using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HotelAppLocal.Data;

namespace HotelAppLocal.ViewModels
{
    public class RegistrationViewModel : INotifyPropertyChanged
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

        private string _confirmPassword = "";
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        private string _error = "";
        public string Error
        {
            get => _error;
            set { _error = value; OnPropertyChanged(); }
        }

        public ICommand RegisterCommand { get; }

        public RegistrationViewModel(AuthService auth)
        {
            _auth = auth;
            RegisterCommand = new Command(async () => await RegisterAsync());
        }

        private async Task RegisterAsync()
        {
            Error = "";

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                Error = "Заполните логин и пароль";
                return;
            }

            if (Password != ConfirmPassword)
            {
                Error = "Пароли не совпадают";
                return;
            }

            var ok = await _auth.RegisterAsync(Username, Password);
            if (!ok)
            {
                Error = "Пользователь с таким логином уже существует";
                return;
            }

            await Navigation.PopAsync(); // вернуться на экран логина
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
