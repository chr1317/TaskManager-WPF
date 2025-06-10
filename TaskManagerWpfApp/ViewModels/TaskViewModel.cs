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
using System.Threading.Tasks;
using TaskManagerWpfApp.views;

namespace TaskManagerWpfApp
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ToDoTasks> Tasks { get; set; } = new();
        private List<ToDoTasks> _allTasks = new();
        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<User> Users { get; set; } = new();

        public ObservableCollection<PriorityLevel> Priorities { get; } =
            new(Enum.GetValues(typeof(PriorityLevel)).Cast<PriorityLevel>());

        private string _selectedPriority;
        public string SelectedPriority
        {
            get => _selectedPriority;
            set { _selectedPriority = value; OnPropertyChanged(nameof(SelectedPriority)); }
        }
        public ICommand AddTaskCommand { get; }
        public ICommand AddSubtask { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand SaveTaskCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ToggleFilterPanelCommand { get; }
        public ICommand ShowStatisticsCommand { get; }
        public ICommand LogoutCommand { get; }

        private ToDoTasks _taskBackup;

        private ToDoTasks _selectedTask;
        public ToDoTasks SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
                OnPropertyChanged(nameof(IsDescriptionEmpty));
                OnPropertyChanged(nameof(CanAddSubtask));
                OnPropertyChanged(nameof(IsTaskCompleted));
            }
        }
        public bool CanAddSubtask => SelectedTask != null && SelectedTask.PID == null;
        public bool IsReadOnlyMode => !IsEditing;
        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
                OnPropertyChanged(nameof(CanSelectOtherTasks));
                OnPropertyChanged(nameof(IsReadOnlyMode));
            }
        }
        public bool CanSelectOtherTasks => !IsEditing;

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

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
                ApplySearchFilter();
            }
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

                _allTasks = dtos.Select(dto => TaskMapper.ToModel(dto, Categories)).ToList();
                ApplyFilters();
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

        public ICommand SortTasksCommand { get; }
        public enum SortField
        {
            Title,
            Priority,
            EndDate,
            Category
        }
        public TaskViewModel()
        {
            AddSubtask = new RelayCommand(OpenAddSubtaskDialog);
            AddTaskCommand = new RelayCommand(OpenAddTaskDialog);
            DeleteTaskCommand = new RelayCommand<ToDoTasks>(async task => await DeleteTask(task));
            SelectTaskCommand = new RelayCommand<ToDoTasks>(SelectTask);
            SortTasksCommand = new RelayCommand(SortAllTasks);
            EditTaskCommand = new RelayCommand(BeginEdit);
            SaveTaskCommand = new RelayCommand(async () => await SaveTask());
            CancelEditCommand = new RelayCommand(CancelEdit);
            SearchCommand = new RelayCommand(ApplySearchFilter);
            ToggleFilterPanelCommand = new RelayCommand(OpenFilterDialog);
            ShowStatisticsCommand = new RelayCommand(ShowStatistics);
            LogoutCommand = new RelayCommand(Logout);

        }
        private void Logout()
        {
            App.Session.LoggedUserId = 0;

            var loginWindow = new Login();
            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();

            foreach (Window w in Application.Current.Windows)
            {
                if (w != loginWindow)
                {
                    w.Close();
                }
            }
        }
        private void ShowStatistics()
        {
            var window = new StatisticsWindow(_allTasks);
            window.ShowDialog();
        }
        private void ApplySearchFilter()
        {
            var filtered = _allTasks.Where(task =>
                string.IsNullOrWhiteSpace(SearchQuery) ||
                task.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            var taskMap = filtered.ToDictionary(t => t.TID);
            Tasks.Clear();

            foreach (var task in filtered)
            {
                if (task.PID.HasValue && taskMap.ContainsKey(task.PID.Value))
                {
                    taskMap[task.PID.Value].Subtasks.Add(task);
                }
                else
                {
                    Tasks.Add(task);
                }
            }
        }
        private Category _filterCategory;
        private PriorityLevel? _filterPriority;
        private DateTime? _filterEndDate;

        private void OpenFilterDialog()
        {
            var dialog = new FilterDialog(Categories, Priorities)
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                _filterCategory = dialog.SelectedCategory;
                _filterPriority = dialog.SelectedPriority;
                _filterEndDate = dialog.SelectedEndDate;

                ApplyFilters();
            }
        }
        private void ApplyFilters()
        {
            var filtered = _allTasks.Where(task =>
                (_filterCategory == null || task.Category?.CID == _filterCategory.CID) &&
                (_filterPriority == null || task.Priority == _filterPriority) &&
                (_filterEndDate == null || (task.EndDate.HasValue && task.EndDate.Value.Date <= _filterEndDate.Value.Date)) &&
                (string.IsNullOrWhiteSpace(SearchQuery) || task.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            var taskMap = filtered.ToDictionary(t => t.TID);
            Tasks.Clear();

            foreach (var task in filtered)
            {
                task.Subtasks.Clear();
            }

            foreach (var task in filtered)
            {
                if (task.PID.HasValue && taskMap.ContainsKey(task.PID.Value))
                {
                    taskMap[task.PID.Value].Subtasks.Add(task);
                }
                else
                {
                    Tasks.Add(task);
                }
            }
        }


        private void BeginEdit()
        {
            if (SelectedTask != null)
            {
                _taskBackup = TaskMapper.Clone(SelectedTask);
                IsEditing = true;
            }
        }

        private async Task SaveTask()
        {
            if (SelectedTask != null)
            {
                try
                {
                    var dto = TaskMapper.ToDto(SelectedTask);
                    await _apiClient.UpdateTaskAsync(dto.TID, dto);
                    if (SelectedTask.PID != null)
                    {
                        var siblings = _allTasks.Where(t => t.PID == SelectedTask.PID);
                        var parent = _allTasks.FirstOrDefault(t => t.TID == SelectedTask.PID);

                        if (parent != null && siblings.All(s => s.Status == TaskStatus.Complete))
                        {
                            parent.Status = TaskStatus.Complete;
                            var parentDto = TaskMapper.ToDto(parent);
                            await _apiClient.UpdateTaskAsync(parentDto.TID, parentDto);
                        }
                        else if (parent != null && parent.Status == TaskStatus.Complete && siblings.Any(s => s.Status != TaskStatus.Complete))
                        {
                            parent.Status = TaskStatus.Incomplete;
                            var parentDto = TaskMapper.ToDto(parent);
                            await _apiClient.UpdateTaskAsync(parentDto.TID, parentDto);
                        }
                    }

                    IsEditing = false;
                    await LoadUserTasks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd zapisu: {ex.Message}");
                }
            }
        }

        private void CancelEdit()
        {
            if (_taskBackup != null && SelectedTask != null)
            {
                var restored = TaskMapper.Clone(_taskBackup);
                restored.Subtasks = SelectedTask.Subtasks;
                SelectedTask.Title = restored.Title;
                SelectedTask.Description = restored.Description;
                SelectedTask.Priority = restored.Priority;
                SelectedTask.EndDate = restored.EndDate;
                SelectedTask.Category = restored.Category;
                // inne pola...

                IsEditing = false;
            }
        }
        private void SortAllTasks()
        {
            SortTaskTree(Tasks);
            _isAscending = !_isAscending;
        }

        private void SortTaskTree(ObservableCollection<ToDoTasks> taskList)
        {
            var sorted = _isAscending
        ? taskList.OrderBy(t => t.Title).ToList()
        : taskList.OrderByDescending(t => t.Title).ToList();
            taskList.Clear();
            foreach (var task in sorted)
            {
                SortTaskTree(task.Subtasks);
                taskList.Add(task);
            }
        }

        private void SelectTask(ToDoTasks task)
        {
            if (!CanSelectOtherTasks) return;

            SelectedTask = null;
            SelectedTask = task;


        }

        private void OpenAddTaskDialog()
        {
            ShowAddTaskDialog();
        }
        public async Task ShowAddTaskDialog()
        {
            var window = new AddTaskWindow
            {
                Owner = Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
            {
                var vm = window.ViewModel;

                var newTaskDto = new Dto.TasksDto
                {
                    Title = vm.Title,
                    Description = vm.Description,
                    CID = vm.SelectedCategory.CID,
                    UID = App.Session.LoggedUserId,
                    Priority = vm.SelectedPriority ?? PriorityLevel.Medium,
                    Status = 0, // Incomplete
                    StartDate = DateTime.Now,
                    EndDate = vm.EndDate,
                    PID = vm.ParentTaskId
                };
                await _apiClient.CreateTaskAsync(newTaskDto);
                await LoadUserTasks();
            }
        }

        public async void OpenAddSubtaskDialog()
        {
            if (SelectedTask == null)
            {
                MessageBox.Show("Nie wybrano zadania, do którego chcesz dodać podzadanie.");
                return;
            }

            var window = new AddTaskWindow
            {
                Owner = Application.Current.MainWindow
            };
            window.ViewModel.ParentTaskId = SelectedTask.TID;
            if (window.ShowDialog() == true)
            {
                var vm = window.ViewModel;


                var newSubtask = new ToDoTasks
                {
                    Title = vm.Title,
                    Description = vm.Description,
                    CID = vm.SelectedCategory.CID,
                    UID = App.Session.LoggedUserId,
                    Priority = vm.SelectedPriority ?? PriorityLevel.Medium,
                    Status = TaskStatus.Incomplete,
                    StartDate = DateTime.Now,
                    EndDate = vm.EndDate,
                    PID = SelectedTask.TID
                };

                try
                {
                    var dto = TaskMapper.ToDto(newSubtask);
                    await _apiClient.CreateTaskAsync(dto);
                    await LoadUserTasks(); // odświeżenie
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd przy dodawaniu podzadania: {ex.Message}");
                }
            }
        }

        public bool IsTaskCompleted
        {
            get => SelectedTask?.Status == TaskStatus.Complete;
            set
            {
                if (SelectedTask == null) return;

                // Blokada ręczna: nie pozwalaj ustawić statusu na Complete, jeśli to zadanie główne i ma nieukończone podzadania
                if (value && SelectedTask.PID == null)
                {
                    var anyUnfinished = SelectedTask.Subtasks.Any(s => s.Status != TaskStatus.Complete);
                    if (anyUnfinished)
                    {
                        MessageBox.Show("Nie można oznaczyć zadania jako ukończone, dopóki wszystkie podzadania nie zostaną zakończone.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                        OnPropertyChanged(nameof(IsTaskCompleted));
                        return;
                    }
                }

                // Po prostu zmień status w pamięci
                SelectedTask.Status = value ? TaskStatus.Complete : TaskStatus.Incomplete;
                OnPropertyChanged(nameof(IsTaskCompleted));
            }
        }




        public async Task LoadCategoriesAsync()
        {
            try
            {
                var categoriesDto = await _apiClient.GetCategoriesAsync();
                var categories = categoriesDto.Select(c => new Category { CID = c.Id, Name = c.Name }).ToList();

                Categories.Clear();
                foreach (var cat in categories)
                {
                    Categories.Add(cat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania kategorii: {ex.Message}");
            }
        }
        public async Task DeleteTask(ToDoTasks task)
        {
            if (task == null)
            {
                MessageBox.Show("Nie wybrano zadania do usunięcia");
                return;
            }

            var result = MessageBox.Show($"Czy na pewno chcesz usunąć zadanie: \"{task.Title}\"?",
                                          "Potwierdzenie usunięcia",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _apiClient.DeleteTaskAsync(task.TID);
                    if (success)
                    {
                        LoadUserTasks();
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się usunąć zadania.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd usuwania zadania: {ex.Message}");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public bool IsDescriptionEmpty => string.IsNullOrWhiteSpace(SelectedTask?.Description);

    }
}
