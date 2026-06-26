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

        // ── Customers ────────────────────────────────────────────────────
        public async Task<IEnumerable<CustomerDto>> GetCustomersAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            return customers.Select(c => new CustomerDto
            {
                UserId = c.UserId, FullName = c.FullName, Email = c.Email ?? string.Empty,
                PhoneNumber = c.PhoneNumber ?? string.Empty, NationalId = c.NationalId, Age = c.Age,
                Username = c.Username, Role = c.Role, IsActive = c.IsActive,
                CreatedAt = c.CreatedAt, Address = c.Address, ProfilePicture = c.ProfilePicture
            });
        }

        // ── Workers ──────────────────────────────────────────────────────
        public async Task<IEnumerable<WorkerDto>> GetWorkersAsync()
        {
            var workers = await _unitOfWork.Workers.GetAllWithCategoryAsync();
            return workers.Select(w => MapWorkerDto(w));
        }

        public async Task<IEnumerable<WorkerDto>> GetPendingWorkersAsync()
        {
            var workers = await _unitOfWork.Workers.GetAllWithCategoryAsync();
            return workers.Where(w => !w.IsValidated).Select(w => MapWorkerDto(w));
        }

        // ── All Users ────────────────────────────────────────────────────
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            var workers = await _unitOfWork.Workers.GetAllAsync();
            var admins = await _unitOfWork.Admins.GetAllAsync();

            var users = new List<UserDto>();

            foreach (var c in customers)
                users.Add(new CustomerDto
                {
                    UserId = c.UserId, FullName = c.FullName, Email = c.Email ?? string.Empty,
                    PhoneNumber = c.PhoneNumber ?? string.Empty, NationalId = c.NationalId, Age = c.Age,
                    Username = c.Username, Role = c.Role, IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt, Address = c.Address, ProfilePicture = c.ProfilePicture
                });

            foreach (var w in workers)
                users.Add(MapWorkerDto(w));

            foreach (var a in admins)
                users.Add(new AdminDto
                {
                    UserId = a.UserId, FullName = a.FullName, Email = a.Email ?? string.Empty,
                    PhoneNumber = a.PhoneNumber ?? string.Empty, NationalId = a.NationalId, Age = a.Age,
                    Username = a.Username, Role = a.Role, IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt
                });

            return users;
        }

        // ── Single User ──────────────────────────────────────────────────
        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(userId);
            if (customer != null)
                return new CustomerDto
                {
                    UserId = customer.UserId, FullName = customer.FullName, Email = customer.Email ?? string.Empty,
                    PhoneNumber = customer.PhoneNumber ?? string.Empty, NationalId = customer.NationalId, Age = customer.Age,
                    Username = customer.Username, Role = customer.Role, IsActive = customer.IsActive,
                    CreatedAt = customer.CreatedAt, Address = customer.Address, ProfilePicture = customer.ProfilePicture
                };

            var worker = await _unitOfWork.Workers.GetByIdAsync(userId);
            if (worker != null)
                return MapWorkerDto(worker);

            return null;
        }

        // ── Update User (Admin edits customer/worker basic info) ─────────
        public async Task<bool> UpdateUserAsync(int userId, string fullName, string email, string phoneNumber, int age, string? address)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(userId);
            if (customer != null)
            {
                customer.FullName = fullName;
                customer.Email = email;
                customer.PhoneNumber = phoneNumber;
                customer.Age = age;
                if (address != null) customer.Address = address;
                _unitOfWork.Customers.Update(customer);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            var worker = await _unitOfWork.Workers.GetByIdAsync(userId);
            if (worker != null)
            {
                worker.FullName = fullName;
                worker.Email = email;
                worker.PhoneNumber = phoneNumber;
                worker.Age = age;
                _unitOfWork.Workers.Update(worker);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            return false;
        }

        // ── Account Actions ──────────────────────────────────────────────
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

        public async Task<bool> ActivateAccountAsync(int userId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(userId);
            if (customer != null)
            {
                customer.IsActive = true;
                _unitOfWork.Customers.Update(customer);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            var worker = await _unitOfWork.Workers.GetByIdAsync(userId);
            if (worker != null)
            {
                worker.IsActive = true;
                _unitOfWork.Workers.Update(worker);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAccountAsync(int userId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(userId);
            if (customer != null)
            {
                // Delete Favorites
                var favorites = await _unitOfWork.Favorites.FindAsync(f => f.CustomerId == userId);
                foreach (var favorite in favorites)
                {
                    _unitOfWork.Favorites.Delete(favorite);
                }

                // Delete Complaints
                var complaints = await _unitOfWork.Complaints.FindAsync(c => c.CustomerId == userId);
                foreach (var complaint in complaints)
                {
                    _unitOfWork.Complaints.Delete(complaint);
                }

                // Delete Reviews
                var reviews = await _unitOfWork.Reviews.FindAsync(r => r.CustomerId == userId);
                foreach (var review in reviews)
                {
                    _unitOfWork.Reviews.Delete(review);
                }

                // Delete ServiceRequests
                var requests = await _unitOfWork.ServiceRequests.FindAsync(sr => sr.CustomerId == userId);
                foreach (var request in requests)
                {
                    var statusHistories = await _unitOfWork.StatusHistories.FindAsync(sh => sh.RequestId == request.RequestId);
                    foreach (var sh in statusHistories)
                    {
                        _unitOfWork.StatusHistories.Delete(sh);
                    }

                    var reqReviews = await _unitOfWork.Reviews.FindAsync(r => r.RequestId == request.RequestId);
                    foreach (var rr in reqReviews)
                    {
                        _unitOfWork.Reviews.Delete(rr);
                    }

                    var reqNotifications = await _unitOfWork.Notifications.FindAsync(n => n.RelatedRequestId == request.RequestId);
                    foreach (var rn in reqNotifications)
                    {
                        _unitOfWork.Notifications.Delete(rn);
                    }

                    _unitOfWork.ServiceRequests.Delete(request);
                }

                // Delete Notifications
                var notifications = await _unitOfWork.Notifications.FindAsync(n => n.UserId == userId);
                foreach (var notification in notifications)
                {
                    _unitOfWork.Notifications.Delete(notification);
                }

                _unitOfWork.Customers.Delete(customer);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            var worker = await _unitOfWork.Workers.GetByIdAsync(userId);
            if (worker != null)
            {
                // Delete Favorites
                var favorites = await _unitOfWork.Favorites.FindAsync(f => f.WorkerId == userId);
                foreach (var favorite in favorites)
                {
                    _unitOfWork.Favorites.Delete(favorite);
                }

                // Delete Complaints
                var complaints = await _unitOfWork.Complaints.FindAsync(c => c.WorkerId == userId);
                foreach (var complaint in complaints)
                {
                    _unitOfWork.Complaints.Delete(complaint);
                }

                // Delete Reviews
                var reviews = await _unitOfWork.Reviews.FindAsync(r => r.WorkerId == userId);
                foreach (var review in reviews)
                {
                    _unitOfWork.Reviews.Delete(review);
                }

                // Delete ServiceRequests
                var requests = await _unitOfWork.ServiceRequests.FindAsync(sr => sr.WorkerId == userId);
                foreach (var request in requests)
                {
                    var statusHistories = await _unitOfWork.StatusHistories.FindAsync(sh => sh.RequestId == request.RequestId);
                    foreach (var sh in statusHistories)
                    {
                        _unitOfWork.StatusHistories.Delete(sh);
                    }

                    var reqReviews = await _unitOfWork.Reviews.FindAsync(r => r.RequestId == request.RequestId);
                    foreach (var rr in reqReviews)
                    {
                        _unitOfWork.Reviews.Delete(rr);
                    }

                    var reqNotifications = await _unitOfWork.Notifications.FindAsync(n => n.RelatedRequestId == request.RequestId);
                    foreach (var rn in reqNotifications)
                    {
                        _unitOfWork.Notifications.Delete(rn);
                    }

                    _unitOfWork.ServiceRequests.Delete(request);
                }

                // Delete Notifications
                var notifications = await _unitOfWork.Notifications.FindAsync(n => n.UserId == userId);
                foreach (var notification in notifications)
                {
                    _unitOfWork.Notifications.Delete(notification);
                }

                _unitOfWork.Workers.Delete(worker);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> RejectWorkerAsync(int workerId)
        {
            var worker = await _unitOfWork.Workers.GetByIdAsync(workerId);
            if (worker == null) return false;
            _unitOfWork.Workers.Delete(worker);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ── Categories ───────────────────────────────────────────────────
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

        // ── Complaints ───────────────────────────────────────────────────
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

        public async Task<ComplaintDto?> GetComplaintByIdAsync(int complaintId)
        {
            var c = await _unitOfWork.Complaints.GetByIdAsync(complaintId);
            if (c == null) return null;
            var cust = await _unitOfWork.Customers.GetByIdAsync(c.CustomerId);
            var wkr = await _unitOfWork.Workers.GetByIdAsync(c.WorkerId);
            return new ComplaintDto
            {
                ComplaintId = c.ComplaintId, CustomerId = c.CustomerId, CustomerName = cust?.FullName ?? "",
                WorkerId = c.WorkerId, WorkerName = wkr?.FullName ?? "", RequestId = c.RequestId,
                Description = c.Description, Status = c.Status, AdminResponse = c.AdminResponse,
                CreatedAt = c.CreatedAt, ResolvedAt = c.ResolvedAt
            };
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

        public async Task<bool> DismissComplaintAsync(int complaintId, string response)
        {
            var complaint = await _unitOfWork.Complaints.GetByIdAsync(complaintId);
            if (complaint == null) return false;
            complaint.Status = ComplaintStatus.Dismissed;
            complaint.AdminResponse = response;
            complaint.ResolvedAt = DateTime.UtcNow;
            _unitOfWork.Complaints.Update(complaint);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ── Service Requests ─────────────────────────────────────────────
        public async Task<IEnumerable<ServiceRequestDto>> GetAllServiceRequestsAsync()
        {
            var requests = await _unitOfWork.ServiceRequests.GetAllWithDetailsAsync();
            return requests.Select(sr => new ServiceRequestDto
            {
                RequestId       = sr.RequestId,
                CustomerId      = sr.CustomerId,
                CustomerName    = sr.Customer?.FullName ?? string.Empty,
                WorkerId        = sr.WorkerId,
                WorkerName      = sr.Worker?.FullName ?? string.Empty,
                CategoryId      = sr.CategoryId,
                CategoryName    = sr.Category?.Name ?? string.Empty,
                LocationDetails = sr.LocationDetails,
                ScheduledDate   = sr.ScheduledDate,
                ScheduledTime   = sr.ScheduledTime,
                Status          = sr.Status,
                Description     = sr.Description,
                CreatedAt       = sr.CreatedAt,
                UpdatedAt       = sr.UpdatedAt,
                PriceAtBooking  = sr.PriceAtBooking,
                // Arrival / completion workflow flags
                IsWorkerArrived                    = sr.IsWorkerArrived,
                WorkerArrivedAt                    = sr.WorkerArrivedAt,
                IsArrivalConfirmedByCustomer       = sr.IsArrivalConfirmedByCustomer,
                ArrivalConfirmedAt                 = sr.ArrivalConfirmedAt,
                IsWorkCompletedConfirmedByCustomer = sr.IsWorkCompletedConfirmedByCustomer,
                WorkCompletionConfirmedAt          = sr.WorkCompletionConfirmedAt,
                // Review data
                ReviewRating    = sr.Review?.Rating,
                ReviewComment   = sr.Review?.Comment,
                ReviewCreatedAt = sr.Review?.CreatedAt,
                // Status timeline
                StatusHistory = sr.StatusHistory
                    .OrderBy(h => h.ChangedAt)
                    .Select(h => new StatusHistoryEntryDto
                    {
                        Status    = h.Status,
                        ChangedAt = h.ChangedAt,
                        Note      = h.Note
                    }).ToList()
            });
        }


        // ── Private Helpers ──────────────────────────────────────────────
        private static WorkerDto MapWorkerDto(Worker w)
        {
            return new WorkerDto
            {
                UserId = w.UserId, FullName = w.FullName, Email = w.Email ?? string.Empty,
                PhoneNumber = w.PhoneNumber ?? string.Empty, NationalId = w.NationalId, Age = w.Age,
                Username = w.Username, Role = w.Role, IsActive = w.IsActive,
                CreatedAt = w.CreatedAt, CategoryId = w.CategoryId,
                CategoryName = w.Category?.Name ?? "", ProfilePicture = w.ProfilePicture,
                IdFrontImage = w.IdFrontImage, IdBackImage = w.IdBackImage,
                ServicePrice = w.ServicePrice, AvailabilityStatus = w.AvailabilityStatus,
                AverageRating = w.AverageRating, IsValidated = w.IsValidated
            };
        }
    }
}
