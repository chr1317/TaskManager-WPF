using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerWpfApp.Dto;

namespace TaskManagerWpfApp.Helpers
{
    public static class TaskMapper
    {
        public static ToDoTasks ToModel(TasksDto dto)
        {
            return new ToDoTasks
            {
                TID = dto.TID,
                UID = dto.UID,
                CID = dto.CID,
                PID = dto.PID,
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = (TaskStatus)dto.Status,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
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
    }
}
