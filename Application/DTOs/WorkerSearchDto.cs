namespace Application.DTOs
{
    public class WorkerSearchDto
    {
        public int? CategoryId { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public string? SortBy { get; set; } // e.g., "PriceAsc", "PriceDesc", "Rating"
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
    }
}
