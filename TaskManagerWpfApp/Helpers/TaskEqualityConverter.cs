using System;
using System.Globalization;
using System.Windows.Data;

namespace TaskManagerWpfApp.Helpers
{
    public class TaskEqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is ToDoTasks selectedTask && values[1] is ToDoTasks currentTask)
            {
                return selectedTask.TID == currentTask.TID;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}