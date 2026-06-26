using FinalProject.Domain.Enums;

namespace FinalProject.Application.DTOs
{
    public class ServiceRequestDto
    {
        public int RequestId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string LocationDetails { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public RequestStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Arrival & completion workflow
        public bool IsWorkerArrived { get; set; }
        public DateTime? WorkerArrivedAt { get; set; }
        public bool IsArrivalConfirmedByCustomer { get; set; }
        public DateTime? ArrivalConfirmedAt { get; set; }
        public bool IsWorkCompletedConfirmedByCustomer { get; set; }
        public DateTime? WorkCompletionConfirmedAt { get; set; }

        // Price captured at booking time
        public decimal PriceAtBooking { get; set; }

        // Review info (populated by admin queries)
        public int? ReviewRating { get; set; }
        public string? ReviewComment { get; set; }
        public DateTime? ReviewCreatedAt { get; set; }

        // Status timeline (populated by admin queries)
        public List<StatusHistoryEntryDto> StatusHistory { get; set; } = new();
    }

    public class StatusHistoryEntryDto
    {
        public RequestStatus Status { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Note { get; set; }
    }
}
