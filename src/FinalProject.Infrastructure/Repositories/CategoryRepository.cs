using FinalProject.Application.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryWithWorkersAsync(int categoryId)
        {
            return await _context.Categories
                .Include(c => c.Workers)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }
    }
}
