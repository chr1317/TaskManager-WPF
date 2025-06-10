using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerWpfApp.Dto
{
    public class TasksDto
    {
        public int TID { get; set; }
        public int UID { get; set; }
        public int CID { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public PriorityLevel Priority { get; set; }
        public int Status { get; set; }

        public int? PID { get; set; } 

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
