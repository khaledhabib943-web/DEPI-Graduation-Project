using FinalProject.Application.DTOs;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;

namespace FinalProject.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public CustomerService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null) return null;

            return MapToDto(customer);
        }

        public async Task<IEnumerable<WorkerDto>> SearchWorkersAsync(SearchFilterDto filters)
        {
            var workers = await _unitOfWork.Workers.GetAllAsync();
            var query = workers.Where(w => w.IsActive && w.IsValidated);

            if (filters.CategoryId.HasValue)
                query = query.Where(w => w.CategoryId == filters.CategoryId.Value);

            if (filters.AvailabilityStatus.HasValue)
                query = query.Where(w => w.AvailabilityStatus == filters.AvailabilityStatus.Value);

            if (filters.MinPrice.HasValue)
                query = query.Where(w => w.ServicePrice >= filters.MinPrice.Value);

            if (filters.MaxPrice.HasValue)
                query = query.Where(w => w.ServicePrice <= filters.MaxPrice.Value);

            if (filters.MinRating.HasValue)
                query = query.Where(w => w.AverageRating >= filters.MinRating.Value);

            if (!string.IsNullOrWhiteSpace(filters.Keyword))
                query = query.Where(w => w.FullName.Contains(filters.Keyword, StringComparison.OrdinalIgnoreCase));

            return query.Select(w => MapWorkerToDto(w));
        }

        public async Task<ServiceRequestDto> CreateServiceRequestAsync(int customerId, CreateServiceRequestDto dto)
        {
            var serviceRequest = new ServiceRequest
            {
                CustomerId = customerId,
                WorkerId = dto.WorkerId,
                CategoryId = dto.CategoryId,
                LocationDetails = dto.LocationDetails,
                ScheduledDate = dto.ScheduledDate,
                ScheduledTime = dto.ScheduledTime,
                Description = dto.Description,
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ServiceRequests.AddAsync(serviceRequest);
            await _unitOfWork.SaveChangesAsync();

            // Send notification to the worker
            await _notificationService.SendNotificationAsync(
                dto.WorkerId,
                "New Service Request",
                $"You have a new service request.",
                NotificationType.NewRequest,
                serviceRequest.RequestId);

            return await MapServiceRequestToDto(serviceRequest);
        }

        public async Task<RequestStatus?> TrackRequestStatusAsync(int customerId, int requestId)
        {
            var request = await _unitOfWork.ServiceRequests.GetByIdAsync(requestId);
            if (request == null || request.CustomerId != customerId)
                return null;

            return request.Status;
        }

        public async Task<ReviewDto> RateWorkerAsync(int customerId, CreateReviewDto dto)
        {
            var review = new Review
            {
                CustomerId = customerId,
                WorkerId = dto.WorkerId,
                RequestId = dto.RequestId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Update worker's average rating
            var avgRating = await _unitOfWork.Reviews.GetAverageRatingForWorkerAsync(dto.WorkerId);
            var worker = await _unitOfWork.Workers.GetByIdAsync(dto.WorkerId);
            if (worker != null)
            {
                worker.AverageRating = (float)avgRating;
                _unitOfWork.Workers.Update(worker);
                await _unitOfWork.SaveChangesAsync();
            }

            return MapReviewToDto(review);
        }

        public async Task<ComplaintDto> SubmitComplaintAsync(int customerId, CreateComplaintDto dto)
        {
            var complaint = new Complaint
            {
                CustomerId = customerId,
                WorkerId = dto.WorkerId,
                RequestId = dto.RequestId,
                Description = dto.Description,
                Status = ComplaintStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Complaints.AddAsync(complaint);
            await _unitOfWork.SaveChangesAsync();

            return MapComplaintToDto(complaint);
        }

        public async Task<bool> AddToFavoritesAsync(int customerId, int workerId)
        {
            var exists = await _unitOfWork.Favorites.IsFavoriteAsync(customerId, workerId);
            if (exists) return false;

            var favorite = new Favorite
            {
                CustomerId = customerId,
                WorkerId = workerId,
                AddedAt = DateTime.UtcNow
            };

            await _unitOfWork.Favorites.AddAsync(favorite);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(int customerId, int workerId)
        {
            var favorite = await _unitOfWork.Favorites.GetFavoriteAsync(customerId, workerId);
            if (favorite == null) return false;

            _unitOfWork.Favorites.Delete(favorite);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<FavoriteDto>> GetFavoritesAsync(int customerId)
        {
            var favorites = await _unitOfWork.Favorites.GetFavoritesByCustomerAsync(customerId);

            return favorites.Select(f => new FavoriteDto
            {
                FavoriteId = f.FavoriteId,
                CustomerId = f.CustomerId,
                WorkerId = f.WorkerId,
                WorkerName = f.Worker?.FullName ?? string.Empty,
                CategoryName = f.Worker?.Category?.Name ?? string.Empty,
                ProfilePicture = f.Worker?.ProfilePicture ?? string.Empty,
                AverageRating = f.Worker?.AverageRating ?? 0,
                AddedAt = f.AddedAt
            });
        }

        public async Task<bool> CancelRequestAsync(int customerId, int requestId)
        {
            var request = await _unitOfWork.ServiceRequests.GetByIdAsync(requestId);
            if (request == null || request.CustomerId != customerId)
                return false;

            if (request.Status != RequestStatus.Pending)
                return false;

            request.Status = RequestStatus.Cancelled;
            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.ServiceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsAsync(int customerId)
        {
            var requests = await _unitOfWork.ServiceRequests.GetRequestsByCustomerAsync(customerId);
            var result = new List<ServiceRequestDto>();
            foreach (var r in requests)
                result.Add(await MapServiceRequestToDto(r));
            return result;
        }

        public async Task<IEnumerable<ComplaintDto>> GetComplaintsAsync(int customerId)
        {
            var complaints = await _unitOfWork.Complaints.GetComplaintsByCustomerAsync(customerId);
            return complaints.Select(c => MapComplaintToDto(c));
        }

        // ---- Private Mapping Methods ----

        private static CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto
            {
                UserId = customer.UserId,
                FullName = customer.FullName,
                Email = customer.Email ?? string.Empty,
                PhoneNumber = customer.PhoneNumber ?? string.Empty,
                NationalId = customer.NationalId,
                Age = customer.Age,
                Username = customer.Username,
                Role = customer.Role,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                Address = customer.Address
            };
        }

        private static WorkerDto MapWorkerToDto(Worker worker)
        {
            return new WorkerDto
            {
                UserId = worker.UserId,
                FullName = worker.FullName,
                Email = worker.Email ?? string.Empty,
                PhoneNumber = worker.PhoneNumber ?? string.Empty,
                NationalId = worker.NationalId,
                Age = worker.Age,
                Username = worker.Username,
                Role = worker.Role,
                IsActive = worker.IsActive,
                CreatedAt = worker.CreatedAt,
                CategoryId = worker.CategoryId,
                CategoryName = worker.Category?.Name ?? string.Empty,
                ProfilePicture = worker.ProfilePicture,
                Portfolio = worker.Portfolio,
                ServicePrice = worker.ServicePrice,
                AvailabilityStatus = worker.AvailabilityStatus,
                AverageRating = worker.AverageRating,
                IsValidated = worker.IsValidated
            };
        }

        private async Task<ServiceRequestDto> MapServiceRequestToDto(ServiceRequest sr)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(sr.CustomerId);
            var worker = await _unitOfWork.Workers.GetByIdAsync(sr.WorkerId);
            var category = await _unitOfWork.Categories.GetByIdAsync(sr.CategoryId);

            return new ServiceRequestDto
            {
                RequestId = sr.RequestId,
                CustomerId = sr.CustomerId,
                CustomerName = customer?.FullName ?? string.Empty,
                WorkerId = sr.WorkerId,
                WorkerName = worker?.FullName ?? string.Empty,
                CategoryId = sr.CategoryId,
                CategoryName = category?.Name ?? string.Empty,
                LocationDetails = sr.LocationDetails,
                ScheduledDate = sr.ScheduledDate,
                ScheduledTime = sr.ScheduledTime,
                Status = sr.Status,
                Description = sr.Description,
                CreatedAt = sr.CreatedAt,
                UpdatedAt = sr.UpdatedAt
            };
        }

        private static ReviewDto MapReviewToDto(Review review)
        {
            return new ReviewDto
            {
                ReviewId = review.ReviewId,
                CustomerId = review.CustomerId,
                CustomerName = review.Customer?.FullName ?? string.Empty,
                WorkerId = review.WorkerId,
                WorkerName = review.Worker?.FullName ?? string.Empty,
                RequestId = review.RequestId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }

        private static ComplaintDto MapComplaintToDto(Complaint complaint)
        {
            return new ComplaintDto
            {
                ComplaintId = complaint.ComplaintId,
                CustomerId = complaint.CustomerId,
                CustomerName = complaint.Customer?.FullName ?? string.Empty,
                WorkerId = complaint.WorkerId,
                WorkerName = complaint.Worker?.FullName ?? string.Empty,
                RequestId = complaint.RequestId,
                Description = complaint.Description,
                Status = complaint.Status,
                AdminResponse = complaint.AdminResponse,
                CreatedAt = complaint.CreatedAt,
                ResolvedAt = complaint.ResolvedAt
            };
        }
    }
}
