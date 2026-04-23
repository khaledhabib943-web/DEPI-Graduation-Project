using Application.DTOs;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IHomeService
    {
        Task<HomeDataDto> GetLandingPageDataAsync();
    }
}
