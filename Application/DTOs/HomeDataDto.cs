using System.Collections.Generic;

namespace Application.DTOs
{
    public class HomeDataDto
    {
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public IEnumerable<WorkerDto> FeaturedWorkers { get; set; } = new List<WorkerDto>();
        public IEnumerable<ReviewDto> RecentReviews { get; set; } = new List<ReviewDto>();
        public IEnumerable<ServiceRequestDto> ActiveBookings { get; set; } = new List<ServiceRequestDto>();
    }
}
