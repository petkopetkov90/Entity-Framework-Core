using Microsoft.AspNetCore.Identity;

namespace BlogApp.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {

        }
        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
