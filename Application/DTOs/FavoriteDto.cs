using System;

namespace Application.DTOs
{
    public class FavoriteDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; }
    }
}
