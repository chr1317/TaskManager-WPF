using System.ComponentModel;
using System.Windows.Input;

namespace TaskManagerWpfApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            // Tu dodaj logikę logowania (np. zapytanie do API)
            if (Username == "admin" && Password == "1234")
            {
                // Logowanie OK – można otworzyć główne okno
                System.Windows.MessageBox.Show("Zalogowano pomyślnie!");
                LoginSucceeded?.Invoke();
            }
            else
            {
                System.Windows.MessageBox.Show("Niepoprawne dane logowania.");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event Action? LoginSucceeded;

    }
}
