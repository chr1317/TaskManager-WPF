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

namespace TaskManagerWpfApp
{
    /// <summary>
    /// Logika interakcji dla klasy Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
            DataContext = new TaskManagerWpfApp.ViewModels.RegisterViewModel();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is RegisterViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}
