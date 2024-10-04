using P01_StudentSystem.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    [Table("Resources")]
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "NVARCHAR")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR")]
        public string Url { get; set; }

        [Required]
        public ResourceType ResourceType { get; set; }

        [Required]
        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
    }
}
