using BlogApp.Common.Enumerations;
using System.ComponentModel.DataAnnotations;
using static BlogApp.Common.Validations.ArticleValidations;

namespace BlogApp.Services.Models
{
    public class ArticleViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(ArticleTitleMinLength)]
        [MaxLength(ArticleTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(ArticleContentMinLength)]
        [MaxLength(ArticleContentMaxLength)]
        public string Content { get; set; } = null!;

        [Required]
        public string CreatedOn { get; set; } = null!;

        [Required]
        [MinLength(ArticleAuthorMinLength)]
        [MaxLength(ArticleAuthorMaxLength)]
        public string Author { get; set; } = null!;

        [Required]
        public Genre Genre { get; set; }
    }
}
