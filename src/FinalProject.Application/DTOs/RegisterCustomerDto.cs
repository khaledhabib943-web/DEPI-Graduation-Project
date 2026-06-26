namespace FinalProject.Application.DTOs
{
    public class RegisterCustomerDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
