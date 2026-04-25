using FinalProject.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Web.ViewModels
{
    public class DashboardViewModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public int ActiveRequestsCount { get; set; }
        public int CompletedRequestsCount { get; set; }
        public int FavoritesCount { get; set; }
        public int UnreadNotifications { get; set; }
        public List<ServiceRequestDto> RecentRequests { get; set; } = new();
    }

    public class SelectServiceViewModel
    {
        public List<CategoryDto> Categories { get; set; } = new();
    }

    public class SelectProfessionalViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<WorkerDto> Workers { get; set; } = new();
        // Filtering
        public string? SortBy { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public float? MinRating { get; set; }
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 6;
        public int TotalCount { get; set; }
    }

    public class ServiceDetailsViewModel
    {
        public int WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal ServicePrice { get; set; }

        [Required(ErrorMessage = "Please describe the issue.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10-1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please provide your location.")]
        [StringLength(300, MinimumLength = 5, ErrorMessage = "Location must be between 5-300 characters.")]
        public string LocationDetails { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a date.")]
        [DataType(DataType.Date)]
        public DateTime ScheduledDate { get; set; } = DateTime.Today.AddDays(1);

        [Required]
        public TimeSpan ScheduledTime { get; set; }
    }

    public class ConfirmationViewModel
    {
        public ServiceRequestDto Request { get; set; } = new();
    }

    public class TrackRequestViewModel
    {
        public ServiceRequestDto Request { get; set; } = new();
        public WorkerDto? Worker { get; set; }
    }

    public class ReviewViewModel
    {
        public int RequestId { get; set; }
        public int WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a rating.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment must not exceed 500 characters.")]
        public string Comment { get; set; } = string.Empty;
    }

    public class ComplaintListViewModel
    {
        public List<ComplaintDto> Complaints { get; set; } = new();
    }

    public class CreateComplaintViewModel
    {
        public int WorkerId { get; set; }
        public int? RequestId { get; set; }

        [Required(ErrorMessage = "Please describe the complaint.")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Description must be between 20-2000 characters.")]
        public string Description { get; set; } = string.Empty;

        public List<ServiceRequestDto> CompletedRequests { get; set; } = new();
    }

    public class NotificationListViewModel
    {
        public List<NotificationDto> Notifications { get; set; } = new();
        public int UnreadCount { get; set; }
    }

    public class FavoriteListViewModel
    {
        public List<FavoriteDto> Favorites { get; set; } = new();
    }

    public class ProfileViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3-100 characters.")]
        [RegularExpression(@"^[\p{L}\s\-\.]+$", ErrorMessage = "Full name can only contain letters, spaces, hyphens, and dots.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^01[0125]\d{8}$", ErrorMessage = "Please enter a valid Egyptian phone number.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "Address must be between 10-300 characters.")]
        public string Address { get; set; } = string.Empty;

        public string? SuccessMessage { get; set; }
    }
}
