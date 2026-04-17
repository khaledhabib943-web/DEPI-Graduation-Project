namespace ProjectRowadd.Models
{
    public enum ComplaintStatus
    {
        Pending,
        InProgress,
        Resolved,
        Rejected
    }
    public class Complaint
    {
        public int ComplaintId { get; private set; }
        public int CustomerId { get; private set; }
        public int WorkerId { get; private set; }
        public int? RequestId { get; private set; }
        public string Description { get; private set; }
        public ComplaintStatus Status { get; private set; }
        public string? AdminResponse { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ResolvedAt { get; private set; }

        // For ORM
        private Complaint() { }

        // submit → implemented as constructor
        public Complaint(
            int customerId,
            int workerId,
            string description,
            int? requestId = null)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.");

            CustomerId = customerId;
            WorkerId = workerId;
            Description = description;
            RequestId = requestId;
            Status = ComplaintStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        // updateStatus
        public void UpdateStatus(ComplaintStatus status)
        {
            Status = status;

            if (status == ComplaintStatus.Resolved ||
                status == ComplaintStatus.Rejected)
            {
                ResolvedAt = DateTime.UtcNow;
            }
        }

        // respond
        public void Respond(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be empty.");

            AdminResponse = response;
        }
    }
}
