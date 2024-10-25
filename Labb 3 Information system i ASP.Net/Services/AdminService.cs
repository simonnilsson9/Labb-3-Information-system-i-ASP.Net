using Labb_3_Information_system_i_ASP.Net.Data;
using Labb_3_Information_system_i_ASP.Net.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Labb_3_Information_system_i_ASP.Net.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _appDbContext = context;
            _userManager = userManager;
        }       

        public async Task CreateEmployeeAsync(ApplicationUser employee, string password)
        {
            // Skapa användaren
            var result = await _userManager.CreateAsync(employee, password);

            if (result.Succeeded)
            {
                // Tilldela rollen "Employee" till den nya användaren
                await _userManager.AddToRoleAsync(employee, "Employee");
            }
            else
            {
                // Hantera eventuella fel som kan ha inträffat vid skapandet av användaren
                foreach (var error in result.Errors)
                {
                    throw new Exception(error.Description); // Du kan anpassa felhanteringen här
                }
            }
        }

        public async Task DeleteEmployeeAsync(ApplicationUser employee)
        {
            // Kontrollera om användaren är null
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null");
            }

            // Ta bort användaren
            var result = await _userManager.DeleteAsync(employee);

            // Kontrollera om borttagningen lyckades
            if (!result.Succeeded)
            {
                // Hantera eventuella fel som kan ha inträffat vid borttagningen
                foreach (var error in result.Errors)
                {
                    throw new Exception(error.Description); // Du kan anpassa felhanteringen här
                }
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllEmployeesAsync()
        {
            var allUsers = await _appDbContext.Users.ToListAsync();

            // Hämta alla roller som matchar "Employee"
            var employeeRole = await _appDbContext.Roles
                .Where(r => r.Name == "Employee")
                .Select(r => r.Id)
                .FirstOrDefaultAsync(); // Hämta ID för rollen

            // Hämta användare med rollen "Employee"
            var employees = allUsers.Where(user =>
            {
                // Hämta roller för varje användare
                var roles = _userManager.GetRolesAsync(user).Result; // Hämta roller synkront (kan förbättras)
                return roles.Contains("Employee");
            }).ToList();

            return employees;
        }

        public async Task<ApplicationUser> GetEmployeeById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task UpdateEmployeeAsync(ApplicationUser employee)
        {
            _appDbContext.Users.Update(employee);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
