using FinalProject.Domain.Entities;

namespace FinalProject.Application.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerWithFavoritesAsync(int customerId);
        Task<Customer?> GetCustomerWithServiceRequestsAsync(int customerId);
    }
}
