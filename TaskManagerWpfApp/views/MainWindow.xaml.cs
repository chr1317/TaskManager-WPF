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
        private readonly TaskViewModel _viewModel = new();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadCategoriesAsync();
            await _viewModel.LoadUserTasks();
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
        /*private void ExpanderHeader_Click(object sender, MouseButtonEventArgs e)
        {
            
            if (sender is TextBlock tb && tb.DataContext is ToDoTasks clickedTask)
            {
                var vm = DataContext as TaskViewModel;
                if (vm != null)
                {
                    vm.SelectedTask = clickedTask;
                }
            }
        }*/
        private void ExpanderHeader_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is ToDoTasks clickedTask)
            {
                if (DataContext is TaskViewModel vm)
                {
                    // Zawsze ustawiamy nowy obiekt – nawet jeśli to ten sam
                    vm.SelectedTask = null;
                    vm.SelectedTask = clickedTask;
                }
            }
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is TaskViewModel vm && e.NewValue is ToDoTasks selected)
            {
                vm.SelectedTask = selected;
            }
        }




        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is Expander expander && expander.DataContext is ToDoTasks task)
            {
                var vm = DataContext as TaskViewModel;
                if (vm != null)
                {
                    vm.SelectedTask = task;
                }
            }
        }

    }
}