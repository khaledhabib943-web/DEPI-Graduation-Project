using FinalProject.Domain.Entities;

namespace FinalProject.Application.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<Category?> GetCategoryWithWorkersAsync(int categoryId);
    }
}
