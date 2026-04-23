using Application.DTOs;
using Application.Interfaces;
using Application.Wrappers;
using Domain_layer.Entities;
using Infrastructure.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ReviewService : IReviewService
    {
        public Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto)
        {
            int newId = MockDatabase.Reviews.Any() ? MockDatabase.Reviews.Max(r => r.Id) + 1 : 1;
            var review = new Review
            {
                Id = newId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CustomerId = dto.CustomerId,
                WorkerId = dto.WorkerId,
                ServiceRequestId = dto.ServiceRequestId,
                CreatedAt = DateTime.UtcNow
            };
            MockDatabase.Reviews.Add(review);
            
            var worker = MockDatabase.Workers.FirstOrDefault(w => w.Id == dto.WorkerId);
            if(worker != null) worker.Reviews.Add(review);

            var customer = MockDatabase.Customers.FirstOrDefault(c => c.Id == dto.CustomerId);

            return Task.FromResult(new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                CustomerId = review.CustomerId,
                CustomerName = customer?.FullName ?? string.Empty,
                WorkerId = review.WorkerId,
                CreatedAt = review.CreatedAt
            });
        }

        public Task<PagedResult<ReviewDto>> GetWorkerReviewsAsync(int workerId, int pageNumber = 1, int pageSize = 10)
        {
            var query = MockDatabase.Reviews.Where(r => r.WorkerId == workerId).OrderByDescending(r => r.CreatedAt);
            var totalCount = query.Count();

            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(r => 
            {
                var customer = MockDatabase.Customers.FirstOrDefault(c => c.Id == r.CustomerId);
                return new ReviewDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CustomerId = r.CustomerId,
                    CustomerName = customer?.FullName ?? string.Empty,
                    WorkerId = r.WorkerId,
                    CreatedAt = r.CreatedAt
                };
            }).ToList();

            return Task.FromResult(new PagedResult<ReviewDto> { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize });
        }
    }
}
