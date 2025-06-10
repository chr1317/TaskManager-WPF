using System.Collections.Generic;
using System.Windows;
using TaskManagerWpfApp.ViewModels;

namespace TaskManagerWpfApp.views
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow(IEnumerable<ToDoTasks> allTasks)
        {
            InitializeComponent();
            DataContext = new StatisticsViewModel(allTasks);
        }
    }
}
