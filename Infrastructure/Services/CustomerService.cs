using Application.DTOs;
using Application.Interfaces;
using Application.Wrappers;
using Domain_layer.Entities;
using Domain_layer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        public Task<PagedResult<WorkerDto>> GetFilteredWorkersAsync(WorkerSearchDto searchDto)
        {
            var query = Infrastructure.Persistence.MockDatabase.Workers.AsQueryable();

            // Filtering
            if (searchDto.CategoryId.HasValue)
                query = query.Where(w => w.CategoryId == searchDto.CategoryId.Value);

            if (searchDto.MinPrice.HasValue)
                query = query.Where(w => w.HourlyRate >= searchDto.MinPrice.Value);

            if (searchDto.MaxPrice.HasValue)
                query = query.Where(w => w.HourlyRate <= searchDto.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var term = searchDto.SearchTerm.ToLower();
                query = query.Where(w => (w.FullName != null && w.FullName.ToLower().Contains(term)) || (w.Bio != null && w.Bio.ToLower().Contains(term)));
            }

            // Sorting
            switch (searchDto.SortBy?.ToLower())
            {
                case "priceasc":
                    query = query.OrderBy(w => w.HourlyRate);
                    break;
                case "pricedesc":
                    query = query.OrderByDescending(w => w.HourlyRate);
                    break;
                case "rating":
                    query = query.OrderByDescending(w => w.Reviews.Any() ? w.Reviews.Average(r => r.Rating) : 0);
                    break;
                default:
                    // default sorting
                    query = query.OrderByDescending(w => w.Reviews.Any() ? w.Reviews.Average(r => r.Rating) : 0);
                    break;
            }

            var totalCount = query.Count();

            // Pagination
            var pageNumber = searchDto.PageNumber > 0 ? searchDto.PageNumber : 1;
            var pageSize = searchDto.PageSize > 0 ? searchDto.PageSize : 10;

            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new WorkerDto
                {
                    Id = w.Id,
                    FullName = w.FullName,
                    Bio = w.Bio,
                    HourlyRate = w.HourlyRate,
                    IsVerified = w.IsVerified,
                    CategoryId = w.CategoryId,
                    CategoryName = w.Category != null ? w.Category.Name : string.Empty,
                    Availability = w.Availability,
                    AverageRating = w.Reviews.Any() ? w.Reviews.Average(r => r.Rating) : 0
                }).ToList();

            var result = new PagedResult<WorkerDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Task.FromResult(result);
        }

        public Task<WorkerDto?> GetWorkerByIdAsync(int id)
        {
            var worker = Infrastructure.Persistence.MockDatabase.Workers.FirstOrDefault(w => w.Id == id);
            if (worker == null) return Task.FromResult<WorkerDto?>(null);

            var dto = new WorkerDto
            {
                Id = worker.Id,
                FullName = worker.FullName ?? "",
                Bio = worker.Bio ?? "",
                HourlyRate = worker.HourlyRate,
                IsVerified = worker.IsVerified,
                CategoryId = worker.CategoryId,
                CategoryName = worker.Category?.Name ?? "",
                Availability = worker.Availability,
                AverageRating = worker.Reviews?.Any() == true ? worker.Reviews.Average(r => r.Rating) : 0
            };

            return Task.FromResult<WorkerDto?>(dto);
        }
    }
}
