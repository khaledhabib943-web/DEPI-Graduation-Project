namespace FinalProject.Application.DTOs
{
    public class FavoriteDto
    {
        public int FavoriteId { get; set; }
        public int CustomerId { get; set; }
        public int WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public float AverageRating { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
