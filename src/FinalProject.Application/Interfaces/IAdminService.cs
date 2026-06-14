using FinalProject.Application.DTOs;

namespace FinalProject.Application.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<CustomerDto>> GetCustomersAsync();
        Task<IEnumerable<WorkerDto>> GetWorkersAsync();
        Task<IEnumerable<WorkerDto>> GetPendingWorkersAsync();
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserAsync(int userId, string fullName, string email, string phoneNumber, int age, string? address);
        Task<bool> ValidateAccountAsync(int userId);
        Task<bool> SuspendAccountAsync(int userId);
        Task<bool> ActivateAccountAsync(int userId);
        Task<bool> DeleteAccountAsync(int userId);
        Task<bool> RejectWorkerAsync(int workerId);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<bool> EditCategoryAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<ComplaintDto>> GetAllComplaintsAsync();
        Task<ComplaintDto?> GetComplaintByIdAsync(int complaintId);
        Task<bool> ResolveComplaintAsync(int complaintId, string response);
        Task<bool> DismissComplaintAsync(int complaintId, string response);
        Task<IEnumerable<ServiceRequestDto>> GetAllServiceRequestsAsync();
    }
}
