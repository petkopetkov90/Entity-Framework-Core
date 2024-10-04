using BlogApp.Services.Models;

namespace BlogApp.Services.Services.Interfaces
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleViewModel>> GetAllAsync();
        Task<ArticleViewModel> FindByIdAsync(int id);
        Task AddAsync(ArticleInputModel article);
        Task UpdateAsync(ArticleViewModel article, int id);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
