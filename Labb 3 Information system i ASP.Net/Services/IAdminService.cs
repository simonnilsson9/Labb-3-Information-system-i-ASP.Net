using Labb_3_Information_system_i_ASP.Net.Models;
using Microsoft.AspNetCore.Identity;

namespace Labb_3_Information_system_i_ASP.Net.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<ApplicationUser>> GetAllEmployeesAsync();
        Task<ApplicationUser> GetEmployeeById(string id);
        Task CreateEmployeeAsync(ApplicationUser employee, string password);
        Task UpdateEmployeeAsync(ApplicationUser employee);
        Task DeleteEmployeeAsync(ApplicationUser employee);
    }
}
