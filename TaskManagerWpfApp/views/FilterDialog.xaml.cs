using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TaskManagerWpfApp.Views
{
    public partial class FilterDialog : Window
    {
        public Category SelectedCategory { get; set; }
        public PriorityLevel? SelectedPriority { get; set; }
        public DateTime? SelectedEndDate { get; set; }

        public ObservableCollection<Category> Categories { get; }
        public ObservableCollection<PriorityLevel> Priorities { get; }

        public FilterDialog(ObservableCollection<Category> categories, ObservableCollection<PriorityLevel> priorities)
        {
            InitializeComponent();
            Categories = categories;
            Priorities = priorities;
            DataContext = this;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            SelectedCategory = null;
            SelectedPriority = null;
            SelectedEndDate = null;

            DialogResult = true;
            Close();
        }
    }

}

