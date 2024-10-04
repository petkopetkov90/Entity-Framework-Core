namespace BlogApp.Data.Repositories.Interfaces
{
    public interface IBlogRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindByIdAsync(int id);
        Task AddAsync(T article);
        void Update(T article);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
