using P01_StudentSystem.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    [Table("Homeworks")]
    public class Homework
    {
        [Key]
        public int HomeworkId { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR")]
        public string Content { get; set; }

        [Required]
        public ContentType ContentType { get; set; }

        [Required]
        public DateTime SubmissionTime { get; set; }

        [Required]
        public int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }

        [Required]
        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
    }
}
