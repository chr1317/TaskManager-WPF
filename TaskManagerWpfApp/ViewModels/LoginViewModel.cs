using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;
using TaskManagerWpfApp.Dto;

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
            LoginCommand = new RelayCommand(async () => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5272");

                var loginRequest = new Dto.LoginRequestDto
                {
                    Username = Username,
                    Password = Password
                };

                var response = await client.PostAsJsonAsync("api/Users/login", loginRequest);

                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var userDto = await response.Content.ReadFromJsonAsync<Dto.UserDto>();
                    if (userDto != null)
                    {
                        App.Session.LoggedUserId = userDto.Id;
                        MessageBox.Show("Zalogowano pomyślnie!");
                        LoginSucceeded?.Invoke();
                    }
                    else
                    {
                        MessageBox.Show("Błąd podczas pobierania danych użytkownika.");
                    }
                }
                else
                {
                    MessageBox.Show($"Błąd logowania. Status: {response.StatusCode}\nTreść: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd połączenia z serwerem: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event Action? LoginSucceeded;

    }
}
