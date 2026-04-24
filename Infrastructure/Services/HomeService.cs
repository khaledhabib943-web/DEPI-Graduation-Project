using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Persistence;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class HomeService : IHomeService
    {
        public Task<HomeDataDto> GetLandingPageDataAsync(int? customerId = null)
        {
            var categories = MockDatabase.Workers
                .Where(w => w.Category != null)
                .Select(w => w.Category!)
                .GroupBy(c => c.Id)
                .Select(g => g.First())
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IconUrl = c.IconUrl
                }).ToList();

            var featuredWorkers = MockDatabase.Workers.Take(4).Select(w => new WorkerDto
            {
                Id = w.Id,
                FullName = w.FullName,
                Bio = w.Bio,
                HourlyRate = w.HourlyRate,
                CategoryId = w.CategoryId,
                CategoryName = w.Category?.Name ?? string.Empty,
                Availability = w.Availability,
                IsVerified = w.IsVerified
            }).ToList();

            var recentReviews = MockDatabase.Reviews.OrderByDescending(r => r.CreatedAt).Take(3).Select(r => 
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

            var activeBookings = new System.Collections.Generic.List<ServiceRequestDto>();
            if (customerId.HasValue)
            {
                activeBookings = MockDatabase.ServiceRequests
                    .Where(sr => sr.CustomerId == customerId.Value && sr.Status != Domain_layer.Enums.RequestStatus.Completed)
                    .Select(sr => new ServiceRequestDto
                    {
                        Id = sr.Id,
                        Description = sr.Description,
                        Address = sr.Address,
                        Status = sr.Status,
                        CustomerId = sr.CustomerId,
                        WorkerId = sr.WorkerId,
                        ScheduledDate = sr.ScheduledDate
                    }).ToList();
            }

            var homeData = new HomeDataDto
            {
                Categories = categories,
                FeaturedWorkers = featuredWorkers,
                RecentReviews = recentReviews,
                ActiveBookings = activeBookings
            };

            return Task.FromResult(homeData);
        }
    }
}
