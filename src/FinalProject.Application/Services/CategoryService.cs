using FinalProject.Application.DTOs;
using FinalProject.Application.Interfaces;

namespace FinalProject.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                IconUrl = c.IconUrl,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                WorkerCount = c.Workers?.Count ?? 0
            });
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _unitOfWork.Categories.GetCategoryWithWorkersAsync(categoryId);
            if (category == null) return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                IconUrl = category.IconUrl,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                WorkerCount = category.Workers?.Count ?? 0
            };
        }

        public async Task<IEnumerable<WorkerDto>> GetWorkersByCategoryAsync(int categoryId)
        {
            var workers = await _unitOfWork.Workers.GetWorkersByCategoryAsync(categoryId);
            return workers.Select(w => new WorkerDto
            {
                UserId = w.UserId, FullName = w.FullName, Email = w.Email,
                PhoneNumber = w.PhoneNumber, NationalId = w.NationalId, Age = w.Age,
                Username = w.Username, Role = w.Role, IsActive = w.IsActive,
                CreatedAt = w.CreatedAt, CategoryId = w.CategoryId,
                CategoryName = w.Category?.Name ?? "", ProfilePicture = w.ProfilePicture,
                Portfolio = w.Portfolio, ServicePrice = w.ServicePrice,
                AvailabilityStatus = w.AvailabilityStatus,
                AverageRating = w.AverageRating, IsValidated = w.IsValidated
            });
        }
    }
}
