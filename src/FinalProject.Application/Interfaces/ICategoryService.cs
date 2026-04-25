using FinalProject.Application.DTOs;

namespace FinalProject.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int categoryId);
        Task<IEnumerable<WorkerDto>> GetWorkersByCategoryAsync(int categoryId);
    }
}
