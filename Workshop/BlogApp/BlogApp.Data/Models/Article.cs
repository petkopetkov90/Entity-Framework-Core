using BlogApp.Common.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BlogApp.Common.Validations.ArticleValidations;

namespace BlogApp.Data.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ArticleTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(ArticleContentMaxLength)]
        public string Content { get; set; } = null!;

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        [MaxLength(ArticleAuthorMaxLength)]
        public string Author { get; set; } = null!;

        [Required]
        public Genre Genre { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
