using Labb_3_Information_system_i_ASP.Net.Models;

namespace Labb_3_Information_system_i_ASP.Net.Services
{
    public interface IDriverService
    {
        Task<IEnumerable<Driver>> GetAllDriversAsync();
        Task<Driver> GetDriverByIdAsync(int driverId);
        Task CreateDriverAsync(Driver driver);
        Task UpdateDriverAsync(Driver driver);
        Task DeleteDriverAsync(Driver driver);
        Task<Driver> GetDriverByName(string name);
        Task<IEnumerable<Driver>> SearchDriversByNameAsync(string searchTerm);
        
    }
}
