using FinalProject.Application.DTOs;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;

namespace FinalProject.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            var workers = await _unitOfWork.Workers.GetAllAsync();
            var admins = await _unitOfWork.Admins.GetAllAsync();

            var users = new List<UserDto>();

            foreach (var c in customers)
                users.Add(new CustomerDto
                {
                    UserId = c.UserId, FullName = c.FullName, Email = c.Email,
                    PhoneNumber = c.PhoneNumber, NationalId = c.NationalId, Age = c.Age,
                    Username = c.Username, Role = c.Role, IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt, Address = c.Address
                });

            foreach (var w in workers)
                users.Add(new WorkerDto
                {
                    UserId = w.UserId, FullName = w.FullName, Email = w.Email,
                    PhoneNumber = w.PhoneNumber, NationalId = w.NationalId, Age = w.Age,
                    Username = w.Username, Role = w.Role, IsActive = w.IsActive,
                    CreatedAt = w.CreatedAt, CategoryId = w.CategoryId,
                    CategoryName = w.Category?.Name ?? "", ProfilePicture = w.ProfilePicture,
                    ServicePrice = w.ServicePrice, AvailabilityStatus = w.AvailabilityStatus,
                    AverageRating = w.AverageRating, IsValidated = w.IsValidated
                });

            foreach (var a in admins)
                users.Add(new AdminDto
                {
                    UserId = a.UserId, FullName = a.FullName, Email = a.Email,
                    PhoneNumber = a.PhoneNumber, NationalId = a.NationalId, Age = a.Age,
                    Username = a.Username, Role = a.Role, IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt
                });

            return users;
        }

        public async Task<bool> ValidateAccountAsync(int userId)
        {
            var worker = await _unitOfWork.Workers.GetByIdAsync(userId);
            if (worker == null) return false;
            worker.IsValidated = true;
            _unitOfWork.Workers.Update(worker);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SuspendAccountAsync(int userId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(userId);
            if (customer != null)
            {
                customer.IsActive = false;
                _unitOfWork.Customers.Update(customer);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            var worker = await _unitOfWork.Workers.GetByIdAsync(userId);
            if (worker != null)
            {
                worker.IsActive = false;
                _unitOfWork.Workers.Update(worker);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAccountAsync(int userId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(userId);
            if (customer != null) { _unitOfWork.Customers.Delete(customer); await _unitOfWork.SaveChangesAsync(); return true; }
            var worker = await _unitOfWork.Workers.GetByIdAsync(userId);
            if (worker != null) { _unitOfWork.Workers.Delete(worker); await _unitOfWork.SaveChangesAsync(); return true; }
            return false;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = new Category { Name = dto.Name, Description = dto.Description, IconUrl = dto.IconUrl, IsActive = true, CreatedAt = DateTime.UtcNow };
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return new CategoryDto { CategoryId = category.CategoryId, Name = category.Name, Description = category.Description, IconUrl = category.IconUrl, IsActive = true, CreatedAt = category.CreatedAt, WorkerCount = 0 };
        }

        public async Task<bool> EditCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return false;
            category.Name = dto.Name;
            category.Description = dto.Description;
            category.IconUrl = dto.IconUrl;
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return false;
            category.IsActive = false;
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ComplaintDto>> GetAllComplaintsAsync()
        {
            var complaints = await _unitOfWork.Complaints.GetAllAsync();
            var result = new List<ComplaintDto>();
            foreach (var c in complaints)
            {
                var cust = await _unitOfWork.Customers.GetByIdAsync(c.CustomerId);
                var wkr = await _unitOfWork.Workers.GetByIdAsync(c.WorkerId);
                result.Add(new ComplaintDto { ComplaintId = c.ComplaintId, CustomerId = c.CustomerId, CustomerName = cust?.FullName ?? "", WorkerId = c.WorkerId, WorkerName = wkr?.FullName ?? "", RequestId = c.RequestId, Description = c.Description, Status = c.Status, AdminResponse = c.AdminResponse, CreatedAt = c.CreatedAt, ResolvedAt = c.ResolvedAt });
            }
            return result;
        }

        public async Task<bool> ResolveComplaintAsync(int complaintId, string response)
        {
            var complaint = await _unitOfWork.Complaints.GetByIdAsync(complaintId);
            if (complaint == null) return false;
            complaint.Status = ComplaintStatus.Resolved;
            complaint.AdminResponse = response;
            complaint.ResolvedAt = DateTime.UtcNow;
            _unitOfWork.Complaints.Update(complaint);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetAllServiceRequestsAsync()
        {
            var requests = await _unitOfWork.ServiceRequests.GetAllAsync();
            var result = new List<ServiceRequestDto>();
            foreach (var sr in requests)
            {
                var cust = await _unitOfWork.Customers.GetByIdAsync(sr.CustomerId);
                var wkr = await _unitOfWork.Workers.GetByIdAsync(sr.WorkerId);
                var cat = await _unitOfWork.Categories.GetByIdAsync(sr.CategoryId);
                result.Add(new ServiceRequestDto { RequestId = sr.RequestId, CustomerId = sr.CustomerId, CustomerName = cust?.FullName ?? "", WorkerId = sr.WorkerId, WorkerName = wkr?.FullName ?? "", CategoryId = sr.CategoryId, CategoryName = cat?.Name ?? "", LocationDetails = sr.LocationDetails, ScheduledDate = sr.ScheduledDate, ScheduledTime = sr.ScheduledTime, Status = sr.Status, Description = sr.Description, CreatedAt = sr.CreatedAt, UpdatedAt = sr.UpdatedAt });
            }
            return result;
        }
    }
}
