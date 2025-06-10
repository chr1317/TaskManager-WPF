using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager_API.Models
{
    [Table("tasks")]
    public class TaskItem
    {
        [Key]
        [Column("TID")]
        public int Id { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("Priority")]
        public PriorityLevel Priority { get; set; }

        [Column("Status")]
        public int Status { get; set; }

        [Column("CID")]
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        [Column("UID")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users User { get; set; } = null!;

        [Column("PID")]
        public int? PID { get; set; }

        [Column("StartDate")]
        public DateTime? StartDate { get; set; }

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [NotMapped]
        public List<TaskItem>? SubTasks { get; set; }
    }
}
