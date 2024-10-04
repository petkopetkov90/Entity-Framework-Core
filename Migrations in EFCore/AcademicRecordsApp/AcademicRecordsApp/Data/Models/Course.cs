using System.ComponentModel.DataAnnotations;

namespace AcademicRecordsApp.Data.Models
{
    public partial class Course
    {
        public Course()
        {
            Students = new HashSet<Student>();
            Exams = new HashSet<Exam>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
    }
}
