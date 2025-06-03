namespace TaskManager_API.Dto
{
    public class TaskDto
    {
        public int TID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Priority { get; set; }
        public int Status { get; set; }
        public int? CID { get; set; }
        public int UID { get; set; }
        public int? PID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}