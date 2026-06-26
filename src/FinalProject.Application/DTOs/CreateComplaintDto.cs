namespace FinalProject.Application.DTOs
{
    public class CreateComplaintDto
    {
        public int WorkerId { get; set; }
        public int? RequestId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
