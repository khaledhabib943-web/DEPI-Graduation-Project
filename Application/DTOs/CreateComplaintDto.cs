namespace Application.DTOs
{
    public class CreateComplaintDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int? ServiceRequestId { get; set; }
    }
}
