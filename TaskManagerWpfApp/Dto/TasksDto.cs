using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerWpfApp.Dto
{
    class TasksDto
    {
        public int TID { get; set; }
        public int UID { get; set; }
        public int CID { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string Priority { get; set; } // np. "Low", "Medium", "High"
        public int Status { get; set; } // np. 0 = Incomplete, 1 = Complete

        public int? PID { get; set; } // Parent Task ID (dla podzadań)

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
