using System;
using System.Windows;
using TaskManagerWpfApp.ViewModels;
using TaskManagerWpfApp.Services;
using TaskManagerWpfApp.Helpers;

namespace TaskManagerWpfApp.Views
{
    public partial class AddTaskWindow : Window
    {
        public AddTaskViewModel ViewModel { get; }

        private readonly ApiClient _apiClient = new();

        public AddTaskWindow()
        {
            InitializeComponent();
            ViewModel = new AddTaskViewModel();
            DataContext = ViewModel;
            LoadCategories();
            LoadPriorities();
        }

        private async void LoadCategories()
        {
            try
            {
                var categoriesDto = await _apiClient.GetCategoriesAsync();
                var categories = categoriesDto.Select(c => new Category { CID = c.Id, Name = c.Name }).ToList();

                ViewModel.Categories.Clear();
                foreach (var cat in categories)
                {
                    ViewModel.Categories.Add(cat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania kategorii: {ex.Message}");
            }
        }
        private void LoadPriorities()
        {
            try
            {
                ViewModel.Priorities.Clear();
                ViewModel.Priorities.Add(PriorityLevel.High);
                ViewModel.Priorities.Add(PriorityLevel.Medium);
                ViewModel.Priorities.Add(PriorityLevel.Low);
            }
            catch (Exception ex)
            {
            }
        }
        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ViewModel.Title))
            {
                MessageBox.Show("Tytuł nie może być pusty.");
                return;
            }

            if (ViewModel.SelectedCategory == null)
            {
                MessageBox.Show("Wybierz kategorię.");
                return;
            }

            var newTaskDto = new Dto.TasksDto
            {
                Title = ViewModel.Title,
                Description = ViewModel.Description,
                CID = ViewModel.SelectedCategory.CID,
                UID = App.Session.LoggedUserId,
                Priority = ViewModel.SelectedPriority ?? PriorityLevel.Medium,
                Status = 0, // Incomplete
                StartDate = DateTime.Now,
                EndDate = ViewModel.EndDate,
                PID = ViewModel.ParentTaskId
            };

            try
            {
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania zadania: {ex.Message}");
            }
        }
        private async void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
