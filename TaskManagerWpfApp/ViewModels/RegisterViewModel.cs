using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Net.Http.Json;

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
            RegisterCommand = new RelayCommand(async () => await RegisterAsync());
        }

        public async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Wypełnij wszystkie pola.");
                return;
            }

            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5272");

                var registerRequest = new Dto.RegisterRequestDto
                {
                    Username = Username,
                    Password = Password
                };

                var response = await client.PostAsJsonAsync("api/Users/register", registerRequest);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Rejestracja przebiegła pomyślnie!");
                    RegisterSucceeded?.Invoke();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Błąd podczas rejestracji: " + error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd połączenia z serwerem: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event Action? RegisterSucceeded;
    }
}
