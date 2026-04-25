using FinalProject.Domain.Enums;

namespace FinalProject.Application.DTOs
{
    public class SearchFilterDto
    {
        public string? Keyword { get; set; }
        public int? CategoryId { get; set; }
        public AvailabilityStatus? AvailabilityStatus { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public float? MinRating { get; set; }
        public bool? IsValidated { get; set; }
    }
}
