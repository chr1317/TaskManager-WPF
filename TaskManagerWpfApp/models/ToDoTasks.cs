using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TaskManagerWpfApp
{
    public enum TaskStatus
    {
        Incomplete = 0,
        Complete = 1
    }
    public enum PriorityLevel
    {
        Low,
        Medium, 
        High
    }

    public class ToDoTasks : INotifyPropertyChanged
    {
        public int TID { get; set; }
        public int UID { get; set; }
        public int CID { get; set; }
        public int? PID { get; set; }

        private string _title;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public PriorityLevel Priority { get; set; }
        public TaskStatus Status { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        private Category _category;
        public Category Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(nameof(Category)); }
        }

        public ObservableCollection<ToDoTasks> Subtasks { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
