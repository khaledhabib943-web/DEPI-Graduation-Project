using FinalProject.Application.DTOs;

namespace FinalProject.Application.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<bool> ValidateAccountAsync(int userId);
        Task<bool> SuspendAccountAsync(int userId);
        Task<bool> DeleteAccountAsync(int userId);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<bool> EditCategoryAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<ComplaintDto>> GetAllComplaintsAsync();
        Task<bool> ResolveComplaintAsync(int complaintId, string response);
        Task<IEnumerable<ServiceRequestDto>> GetAllServiceRequestsAsync();
    }
}
