namespace Application.DTOs
{
    public class CreateReviewDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int WorkerId { get; set; }
        public int ServiceRequestId { get; set; }
    }
}
