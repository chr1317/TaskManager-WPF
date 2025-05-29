using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager_API.Models
{
    [Table("users")]
    public class Users
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Username")]
        public string Username { get; set; }

        [Column("PasswordHash")]
        public string PasswordHash { get; set; }

        public ICollection<TaskItem>? Tasks { get; set; }
    }
}
