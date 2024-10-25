using Labb_3_Information_system_i_ASP.Net.Data;
using Labb_3_Information_system_i_ASP.Net.Models;
using Microsoft.EntityFrameworkCore;

namespace Labb_3_Information_system_i_ASP.Net.Services
{    
    public class DriverService : IDriverService
    {
        private readonly AppDbContext _appDbContext;
        public DriverService(AppDbContext context)
        {
            _appDbContext = context;
        }
        public async Task CreateDriverAsync(Driver driver)
        {
            await _appDbContext.Drivers.AddAsync(driver);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteDriverAsync(Driver driver)
        {
            _appDbContext.Drivers.Remove(driver);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Driver>> GetAllDriversAsync()
        {
            return await _appDbContext.Drivers
            .Include(d => d.ResponsibleEmployee) 
            .ToListAsync();
        }

        public async Task<Driver> GetDriverByIdAsync(int driverId)
        {
            return await _appDbContext.Drivers.Include(d => d.ResponsibleEmployee).FirstOrDefaultAsync(d => d.DriverID == driverId);
        }

        public async Task<Driver> GetDriverByName(string name)
        {
            return await _appDbContext.Drivers.FirstOrDefaultAsync(d => d.DriverName.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Driver>> SearchDriversByNameAsync(string searchTerm)
        {
            return await _appDbContext.Drivers
                .Where(d => d.DriverName.Contains(searchTerm)) 
                .Include(d => d.ResponsibleEmployee) 
                .ToListAsync();
        }

        public async Task UpdateDriverAsync(Driver driver)
        {
            _appDbContext.Drivers.Update(driver);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
