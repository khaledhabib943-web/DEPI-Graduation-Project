namespace FinalProject.Application.DTOs
{
    public class CreateReviewDto
    {
        public int WorkerId { get; set; }
        public int RequestId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
