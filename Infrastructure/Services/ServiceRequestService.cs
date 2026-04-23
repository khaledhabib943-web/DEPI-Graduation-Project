using Application.DTOs;
using Application.Interfaces;
using Application.Wrappers;
using Domain_layer.Entities;
using Domain_layer.Enums;
using Infrastructure.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        public Task<ServiceRequestDto> CreateRequestAsync(CreateServiceRequestDto dto)
        {
            int newId = MockDatabase.ServiceRequests.Any() ? MockDatabase.ServiceRequests.Max(r => r.Id) + 1 : 1;
            
            var request = new ServiceRequest
            {
                Id = newId,
                Description = dto.Description,
                Address = dto.Address,
                ScheduledDate = dto.ScheduledDate,
                CustomerId = dto.CustomerId,
                WorkerId = dto.WorkerId,
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            MockDatabase.ServiceRequests.Add(request);

            var worker = MockDatabase.Workers.FirstOrDefault(w => w.Id == dto.WorkerId);

            return Task.FromResult(new ServiceRequestDto
            {
                Id = request.Id,
                Description = request.Description,
                Address = request.Address,
                ScheduledDate = request.ScheduledDate,
                CustomerId = request.CustomerId,
                WorkerId = request.WorkerId,
                WorkerName = worker?.FullName ?? string.Empty,
                Status = request.Status,
                CreatedAt = request.CreatedAt
            });
        }

        public Task<PagedResult<ServiceRequestDto>> GetCustomerRequestsAsync(int customerId, int pageNumber = 1, int pageSize = 10)
        {
            var query = MockDatabase.ServiceRequests.Where(r => r.CustomerId == customerId).OrderByDescending(r => r.CreatedAt);
            var totalCount = query.Count();

            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(r => 
            {
                var worker = MockDatabase.Workers.FirstOrDefault(w => w.Id == r.WorkerId);
                return new ServiceRequestDto
                {
                    Id = r.Id,
                    Description = r.Description,
                    Address = r.Address,
                    ScheduledDate = r.ScheduledDate,
                    CustomerId = r.CustomerId,
                    WorkerId = r.WorkerId,
                    WorkerName = worker?.FullName ?? string.Empty,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                };
            }).ToList();

            return Task.FromResult(new PagedResult<ServiceRequestDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        public Task<bool> UpdateRequestStatusAsync(int requestId, RequestStatus status)
        {
            var request = MockDatabase.ServiceRequests.FirstOrDefault(r => r.Id == requestId);
            if (request == null) return Task.FromResult(false);

            request.Status = status;
            return Task.FromResult(true);
        }
    }
}
