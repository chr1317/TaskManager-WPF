using System.Collections.ObjectModel;
using System.ComponentModel;
using TaskManagerWpfApp;
using System.Linq;
using TaskManagerWpfApp.Services;
using System.Windows;
using TaskManagerWpfApp.Helpers;
using TaskManagerWpfApp.Views;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace TaskManagerWpfApp
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ToDoTasks> Tasks { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<User> Users { get; set; } = new();

        public ObservableCollection<string> Priorities { get; } = new()
{
    "High",
    "Medium",
    "Low"
};

        private string _selectedPriority;
        public string SelectedPriority
        {
            get => _selectedPriority;
            set { _selectedPriority = value; OnPropertyChanged(nameof(SelectedPriority)); }
        }
        public ICommand AddTaskCommand { get; }

        private ToDoTasks _selectedTask;
        public ToDoTasks SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
                OnPropertyChanged(nameof(IsDescriptionEmpty));
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
                //Tasks.Clear();
                foreach (var dto in dtos)
                    Tasks.Add(TaskMapper.ToModel(dto));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wczytywania zadań: {ex.Message}");
            }
        }

        public ICommand SelectTaskCommand { get; }
        private SortField _selectedSortField = SortField.Title;
        public SortField SelectedSortField
        {
            get => _selectedSortField;
            set { _selectedSortField = value; OnPropertyChanged(nameof(SelectedSortField)); }
        }

        private bool _isAscending = true;
        public string SortDirectionIcon => _isAscending ? "⬇" : "⬆";

        public ICommand SortTasksCommand { get; }
        public enum SortField
        {
            Title,
            Priority,
            EndDate
        }
        public TaskViewModel()
        {
            AddTaskCommand = new RelayCommand(OpenAddTaskDialog);
            SelectTaskCommand = new RelayCommand<ToDoTasks>(SelectTask);
            SortTasksCommand = new RelayCommand(SortAllTasks);

            // DANE TESTOWE widoczne od razu po uruchomieniu
            var zadanieGlowne = new ToDoTasks
            {
                TID = 1,
                Title = "Zadanie główne testowe",
                Description = "Opis zadania głównego",
                Priority = "High",
                Status = TaskStatus.Incomplete,
                Subtasks = new ObservableCollection<ToDoTasks>()
            };

            var subtask1 = new ToDoTasks
            {
                TID = 2,
                PID = 1,
                Title = "Podzadanie 1",
                Description = "Opis podzadania",
                Priority = "Medium",
                Status = TaskStatus.Incomplete
            };

            var subtask2 = new ToDoTasks
            {
                TID = 3,
                PID = 1,
                Title = "Podzadanie 2",
                Description = "Inne podzadanie testowe",
                Priority = "Low",
                Status = TaskStatus.Complete
            };

            zadanieGlowne.Subtasks.Add(subtask1);
            zadanieGlowne.Subtasks.Add(subtask2);

            Tasks.Add(zadanieGlowne);
        }
        private void SortAllTasks()
        {
            SortTaskTree(Tasks);
            _isAscending = !_isAscending;
        }

        private void SortTaskTree(ObservableCollection<ToDoTasks> taskList)
        {
            // Kopia posortowana
            var sorted = _isAscending
        ? taskList.OrderBy(t => t.Title).ToList()
        : taskList.OrderByDescending(t => t.Title).ToList();
            // Czyścimy oryginał i dodajemy w kolejności
            taskList.Clear();
            foreach (var task in sorted)
            {
                // Sortuj rekurencyjnie podzadania
                SortTaskTree(task.Subtasks);
                taskList.Add(task);
            }
        }

        private void SelectTask(ToDoTasks task)
        {
            SelectedTask = null; // jeśli potrzebujesz odświeżyć UI
            SelectedTask = task;


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
