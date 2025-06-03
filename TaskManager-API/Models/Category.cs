using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager_API.Models
{
    [Table("categories")]
    public class Category
    {
        [Column("CID")]
        [Key]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        //public ICollection<TaskItem>? Tasks { get; set; }
    }
}
