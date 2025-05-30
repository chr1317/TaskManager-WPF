using System.Collections.ObjectModel;
using System.ComponentModel;
using TaskManagerWpfApp;
using System.Linq;

namespace TaskManagerWpfApp
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ToDoTasks> Tasks { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<User> Users { get; set; } = new();

        private ToDoTasks _selectedTask;
        public ToDoTasks SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(nameof(SelectedTask)); }
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

        public TaskViewModel()
        {
            // Przykładowe dane testowe
            Categories.Add(new Category { CID = 1, Name = "Dom" });
            Categories.Add(new Category { CID = 2, Name = "Praca" });

            Users.Add(new User { Id = 1, Username = "admin" });

            Tasks.Add(new ToDoTasks
            {
                TID = 1,
                Title = "Przykładowe zadanie",
                Description = "Zrobić coś ważnego",
                CID = 1,
                UID = 1,
                Priority = "High",
                Status = TaskStatus.Incomplete,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
            });
        }

        public void AddTask(ToDoTasks task)
        {
            Tasks.Add(task);
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public bool IsDescriptionEmpty => string.IsNullOrWhiteSpace(SelectedTask?.Description);

    }
}
