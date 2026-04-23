using Application.DTOs;
using Application.Interfaces;
using Domain_layer.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ComplaintService : IComplaintService
    {
        public Task<ComplaintDto> CreateComplaintAsync(CreateComplaintDto dto)
        {
            int newId = MockDatabase.Complaints.Any() ? MockDatabase.Complaints.Max(c => c.Id) + 1 : 1;
            var complaint = new Complaint
            {
                Id = newId,
                Title = dto.Title,
                Content = dto.Content,
                CustomerId = dto.CustomerId,
                ServiceRequestId = dto.ServiceRequestId,
                CreatedAt = DateTime.UtcNow
            };
            MockDatabase.Complaints.Add(complaint);

            return Task.FromResult(new ComplaintDto
            {
                Id = complaint.Id,
                Title = complaint.Title,
                Content = complaint.Content,
                CustomerId = complaint.CustomerId,
                ServiceRequestId = complaint.ServiceRequestId,
                CreatedAt = complaint.CreatedAt
            });
        }

        public Task<IEnumerable<ComplaintDto>> GetCustomerComplaintsAsync(int customerId)
        {
            var items = MockDatabase.Complaints.Where(c => c.CustomerId == customerId).Select(c => new ComplaintDto
            {
                Id = c.Id,
                Title = c.Title,
                Content = c.Content,
                CustomerId = c.CustomerId,
                ServiceRequestId = c.ServiceRequestId,
                CreatedAt = c.CreatedAt
            }).ToList();

            return Task.FromResult<IEnumerable<ComplaintDto>>(items);
        }
    }
}
