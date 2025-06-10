using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerWpfApp.Dto;

namespace TaskManagerWpfApp.Helpers
{
    public static class TaskMapper
    {
        public static ToDoTasks ToModel(TasksDto dto, IEnumerable<Category> categories)
        {
            return new ToDoTasks
            {
                TID = dto.TID,
                UID = dto.UID,
                CID = dto.CID,
                PID = dto.PID,
                Title = dto.Title,
                Description = dto.Description,
                Priority = Enum.TryParse(dto.Priority.ToString(), out PriorityLevel prio) ? prio : PriorityLevel.Medium,
                Status = (TaskStatus)dto.Status,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Category = categories.FirstOrDefault(c => c.CID == dto.CID)
            };
        }

        public static TasksDto ToDto(ToDoTasks model)
        {
            return new TasksDto
            {
                TID = model.TID,
                UID = model.UID,
                CID = model.CID,
                PID = model.PID,
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                Status = (int)model.Status,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };
        }
        public static ToDoTasks Clone(ToDoTasks original)
        {
            return new ToDoTasks
            {
                TID = original.TID,
                Title = original.Title,
                Description = original.Description,
                UID = original.UID,
                CID = original.CID,
                PID = original.PID,
                Priority = original.Priority,
                Status = original.Status,
                StartDate = original.StartDate,
                EndDate = original.EndDate,
                Category = original.Category,
                Subtasks = new ObservableCollection<ToDoTasks>(
                    original.Subtasks.Select(Clone))
            };
        }
    }
}
