using BlogApp.Data.Models;
using BlogApp.Data.Repositories.Interfaces;
using BlogApp.Services.Models;
using BlogApp.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlogApp.Services.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IBlogRepository<Article> repository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<ApplicationUser> userManager;

        public ArticleService(IBlogRepository<Article> repository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            this.repository = repository;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<ArticleViewModel>> GetAllAsync()
        {
            var articles = await repository.GetAllAsync();

            var viewArticles = articles.Select(a => new ArticleViewModel
            {
                Id = a.Id,
                Author = a.Author,
                Genre = a.Genre,
                CreatedOn = a.CreatedOn.ToString("dd-MM-yyyy hh:mm"),
                Content = a.Content,
                Title = a.Title
            }
            ).ToList();

            return viewArticles;
        }

        public async Task<ArticleViewModel> FindByIdAsync(int id)
        {
            var article = await repository.FindByIdAsync(id);

            if (article is null)
            {
                throw new KeyNotFoundException("Article not found.");
            }

            var articleView = new ArticleViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                CreatedOn = article.CreatedOn.ToString("dd-MM-yyyy hh:mm"),
                Author = article.Author,
                Genre = article.Genre
            };

            return articleView;
        }

        public async Task AddAsync(ArticleInputModel inputModel)
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await userManager.FindByIdAsync(userId);

            if (userId is null)
            {
                throw new UnauthorizedAccessException("User is not logged in or does not have a valid claim.");
            }

            if (user is not null)
            {
                var article = new Article()
                {
                    Title = inputModel.Title,
                    Content = inputModel.Content,
                    CreatedOn = DateTime.Now,
                    Author = user.UserName,
                    Genre = inputModel.Genre,
                    UserId = user.Id,
                    ApplicationUser = user
                };

                await repository.AddAsync(article);
                await repository.SaveAsync();
            }
        }

        public async Task UpdateAsync(ArticleViewModel inputModel, int id)
        {

            var article = await repository.FindByIdAsync(id);

            if (article is null)
            {
                throw new KeyNotFoundException("Article not found.");
            }

            if (inputModel.Id != id)
            {
                return;
            }

            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                throw new UnauthorizedAccessException("User is not logged in or does not have a valid claim.");
            }

            if (userId != article.UserId)
            {
                throw new UnauthorizedAccessException("User is not authorized to update this article.");
            }

            article.Content = inputModel.Content;
            article.Title = inputModel.Title;
            article.Genre = inputModel.Genre;

            repository.Update(article);
            await repository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var article = await repository.FindByIdAsync(id);

            if (article is null)
            {
                throw new KeyNotFoundException("Article not found.");
            }

            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                throw new UnauthorizedAccessException("User is not logged in or does not have a valid claim.");
            }

            if (userId != article.UserId)
            {
                throw new UnauthorizedAccessException("User is not authorized to delete this article.");
            }

            await repository.DeleteAsync(id);
            await repository.SaveAsync();
        }

        public async Task SaveAsync()
        {
            await repository.SaveAsync();
        }
    }
}
