using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskManagerWpfApp.ViewModels;
using TaskManagerWpfApp;

namespace TaskManagerWpfApp
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            var vm = new LoginViewModel();
            vm.LoginSucceeded += OnLoginSuccess;
            DataContext = vm;
        }
        private void OnLoginSuccess()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new Register();
            registerWindow.Show();
            this.Close();
        }

    }
}
