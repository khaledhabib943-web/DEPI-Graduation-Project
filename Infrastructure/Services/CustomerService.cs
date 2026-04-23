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
        private static readonly List<Worker> _workers = new List<Worker>
        {
            new Worker { Id = 1, FullName = "Ahmed Ali", Bio = "Expert plumber", HourlyRate = 150, CategoryId = 1, Category = new Category { Id = 1, Name = "Plumbing" }, Availability = AvailabilityStatus.Available, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 4 }, new Review { Rating = 5 } } },
            new Worker { Id = 2, FullName = "Mohamed Salah", Bio = "Professional electrician", HourlyRate = 200, CategoryId = 2, Category = new Category { Id = 2, Name = "Electrical" }, Availability = AvailabilityStatus.Busy, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 5 } } },
            new Worker { Id = 3, FullName = "Omar Hassan", Bio = "Experienced carpenter", HourlyRate = 120, CategoryId = 3, Category = new Category { Id = 3, Name = "Carpentry" }, Availability = AvailabilityStatus.Available, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 4 } } },
            new Worker { Id = 4, FullName = "Khaled Youssef", Bio = "HVAC specialist", HourlyRate = 250, CategoryId = 4, Category = new Category { Id = 4, Name = "HVAC" }, Availability = AvailabilityStatus.Offline, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 5 }, new Review { Rating = 4 } } },
            new Worker { Id = 5, FullName = "Hassan Mostafa", Bio = "Painter with 10 years experience", HourlyRate = 100, CategoryId = 5, Category = new Category { Id = 5, Name = "Painting" }, Availability = AvailabilityStatus.Available, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 4 }, new Review { Rating = 3 } } },
            new Worker { Id = 6, FullName = "Mahmoud Ezzat", Bio = "Quick plumbing fixes", HourlyRate = 130, CategoryId = 1, Category = new Category { Id = 1, Name = "Plumbing" }, Availability = AvailabilityStatus.Available, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 3 }, new Review { Rating = 4 } } },
            new Worker { Id = 7, FullName = "Ibrahim Nabil", Bio = "Smart home installations", HourlyRate = 300, CategoryId = 2, Category = new Category { Id = 2, Name = "Electrical" }, Availability = AvailabilityStatus.Busy, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 5 }, new Review { Rating = 5 } } },
            new Worker { Id = 8, FullName = "Tariq Ziad", Bio = "Furniture assembly expert", HourlyRate = 110, CategoryId = 3, Category = new Category { Id = 3, Name = "Carpentry" }, Availability = AvailabilityStatus.Available, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 4 } } },
            new Worker { Id = 9, FullName = "Amr Diab", Bio = "AC cleaning and repair", HourlyRate = 180, CategoryId = 4, Category = new Category { Id = 4, Name = "HVAC" }, Availability = AvailabilityStatus.Offline, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 5 }, new Review { Rating = 4 } } },
            new Worker { Id = 10, FullName = "Sayed Makkawy", Bio = "Decorative painting", HourlyRate = 160, CategoryId = 5, Category = new Category { Id = 5, Name = "Painting" }, Availability = AvailabilityStatus.Available, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 5 } } },
            new Worker { Id = 11, FullName = "Adel Imam", Bio = "Pipe leak detection", HourlyRate = 140, CategoryId = 1, Category = new Category { Id = 1, Name = "Plumbing" }, Availability = AvailabilityStatus.Available, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 4 } } },
            new Worker { Id = 12, FullName = "Yasser Galal", Bio = "Rewiring and electrical safety", HourlyRate = 220, CategoryId = 2, Category = new Category { Id = 2, Name = "Electrical" }, Availability = AvailabilityStatus.Available, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 5 }, new Review { Rating = 4 } } },
            new Worker { Id = 13, FullName = "Wael Gomaa", Bio = "Custom wooden doors", HourlyRate = 200, CategoryId = 3, Category = new Category { Id = 3, Name = "Carpentry" }, Availability = AvailabilityStatus.Busy, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 4 }, new Review { Rating = 5 } } },
            new Worker { Id = 14, FullName = "Nour El Sherif", Bio = "Split AC maintenance", HourlyRate = 210, CategoryId = 4, Category = new Category { Id = 4, Name = "HVAC" }, Availability = AvailabilityStatus.Available, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 4 } } },
            new Worker { Id = 15, FullName = "Ayman Zidan", Bio = "Exterior house painting", HourlyRate = 120, CategoryId = 5, Category = new Category { Id = 5, Name = "Painting" }, Availability = AvailabilityStatus.Available, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 4 } } },
            new Worker { Id = 16, FullName = "Tamer Hosny", Bio = "Emergency plumbing 24/7", HourlyRate = 350, CategoryId = 1, Category = new Category { Id = 1, Name = "Plumbing" }, Availability = AvailabilityStatus.Available, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 5 } } },
            new Worker { Id = 17, FullName = "Maged El Kedwany", Bio = "Industrial electrical panels", HourlyRate = 400, CategoryId = 2, Category = new Category { Id = 2, Name = "Electrical" }, Availability = AvailabilityStatus.Offline, IsVerified = true, Reviews = new List<Review> { new Review { Rating = 5 } } }
        };

        public Task<PagedResult<WorkerDto>> GetFilteredWorkersAsync(WorkerSearchDto searchDto)
        {
            var query = _workers.AsQueryable();

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
    }
}
