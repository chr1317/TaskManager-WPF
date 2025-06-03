using System.Configuration;
using System.Data;
using System.Windows;

namespace TaskManagerWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var loginWindow = new Login(); // albo: new Views.Login()
            loginWindow.Show();
        }
        public static class Session
        {
            public static int LoggedUserId { get; set; }
        }
    }

}
