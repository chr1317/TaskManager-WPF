using System.Collections.ObjectModel;
using System.ComponentModel;
using TaskManagerWpfApp;
using System.Linq;
using TaskManagerWpfApp.Services;
using System.Windows;
using TaskManagerWpfApp.Helpers;
using TaskManagerWpfApp.Views;
using System.Windows.Input;

namespace TaskManagerWpfApp
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ToDoTasks> Tasks { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<User> Users { get; set; } = new();
        public ICommand AddTaskCommand { get; }

        private ToDoTasks _selectedTask;
        public ToDoTasks SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask != value)
                {
                    _selectedTask = value;
                    OnPropertyChanged(nameof(SelectedTask));
                    OnPropertyChanged(nameof(IsDescriptionEmpty));
                }
            }
        }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(nameof(SelectedCategory)); }
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(nameof(SelectedUser)); }
        }
        private readonly ApiClient _apiClient = new();

        public async Task LoadUserTasks()
        {
            var userId = App.Session.LoggedUserId;

            if (userId == 0)
                return;

            try
            {
                var dtos = await _apiClient.GetTasksByUserAsync(userId);
                Tasks.Clear();
                foreach (var dto in dtos)
                    Tasks.Add(TaskMapper.ToModel(dto));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wczytywania zadań: {ex.Message}");
            }
        }

        public TaskViewModel()
        {
            AddTaskCommand = new RelayCommand(OpenAddTaskDialog);
        }
        private void OpenAddTaskDialog()
        {
            ShowAddTaskDialog();
        }
        public void AddTask(ToDoTasks task)
        {
            Tasks.Add(task);
        }
        public void ShowAddTaskDialog()
        {
            var window = new AddTaskWindow
            {
                Owner = Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
            {
                var vm = window.ViewModel;

                var newTask = new ToDoTasks
                {
                    Title = vm.Title,
                    Description = vm.Description,
                    CID = vm.SelectedCategory.CID,
                    UID = App.Session.LoggedUserId,
                    Priority = "Medium",
                    Status = TaskStatus.Incomplete,
                    StartDate = DateTime.Now,
                    EndDate = vm.EndDate
                };

                Tasks.Add(newTask);
            }
        }
        public void RemoveTask(ToDoTasks task)
        {
            Tasks.Remove(task);
        }

        public void ToggleTaskCompletion(ToDoTasks task)
        {
            task.Status = task.Status == TaskStatus.Complete ? TaskStatus.Incomplete : TaskStatus.Complete;
        }

        public void AddSubtask(ToDoTasks parent, ToDoTasks subtask)
        {
            subtask.PID = parent.TID;
            parent.Subtasks.Add(subtask);
        }
        public async Task LoadCategoriesAsync()
        {
            try
            {
                var categoriesDto = await _apiClient.GetCategoriesAsync();
                var categories = categoriesDto.Select(c => new Category { CID = c.Id, Name = c.Name }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania kategorii: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public bool IsDescriptionEmpty => string.IsNullOrWhiteSpace(SelectedTask?.Description);

    }
}
