using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    [Table("Students")]
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string Name { get; set; }

        [MaxLength(10)]
        [MinLength(10)]
        [Column(TypeName = "VARCHAR")]
        public string? PhoneNumber { get; set; }

        [Required]
        public DateTime RegisteredOn { get; set; }

        public DateTime? Birthday { get; set; }

        public virtual ICollection<StudentCourse> StudentsCourses { get; set; }

        public virtual ICollection<Homework> Homeworks { get; set; }
    }
}
