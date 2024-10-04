using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    [Table("Courses")]
    public class Course
    {

        [Key]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(80)]
        [Column(TypeName = "NVARCHAR")]
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR")]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal Price { get; set; }

        public virtual ICollection<StudentCourse> StudentsCourses { get; set; }

        public virtual ICollection<Homework> Homeworks { get; set; }

        public virtual ICollection<Resource> Resources { get; set; }
    }
}
