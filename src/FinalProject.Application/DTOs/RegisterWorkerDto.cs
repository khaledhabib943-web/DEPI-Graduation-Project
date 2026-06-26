namespace FinalProject.Application.DTOs
{
    public class RegisterWorkerDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Username { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string ProfilePicture { get; set; } = string.Empty;
        public string IdFrontImage { get; set; } = string.Empty;
        public string IdBackImage { get; set; } = string.Empty;
        public string? Portfolio { get; set; }
        public decimal ServicePrice { get; set; }
    }
}
