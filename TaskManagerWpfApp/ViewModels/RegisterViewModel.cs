using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace TaskManagerWpfApp.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public string Password { get; set; }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(Register);
        }

        private void Register()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Wypełnij wszystkie pola.");
                return;
            }

            // TODO: wyślij dane do API
            MessageBox.Show($"Zarejestrowano użytkownika: {Username}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
