using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskManagerWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TaskInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}