using Labb_3_Information_system_i_ASP.Net.Data;
using Labb_3_Information_system_i_ASP.Net.Models;
using Microsoft.EntityFrameworkCore;

namespace Labb_3_Information_system_i_ASP.Net.Services
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _appDbContext;
        public EventService(AppDbContext context)
        {
            _appDbContext = context;
        }
        public async Task CreateEventAsync(Event newEvent)
        {
            await _appDbContext.Events.AddAsync(newEvent);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteEventAsync(Event deleteEvent)
        {
            _appDbContext.Events.Remove(deleteEvent);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _appDbContext.Events
            .Include(e => e.Driver) 
            .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetAllEventsFromDriver(int driverId)
        {
            return await _appDbContext.Events
            .Where(e => e.DriverId == driverId)
            .ToListAsync();
        }

        public async Task<Event> GetEventAsyncById(int id)
        {
            return await _appDbContext.Events.FirstOrDefaultAsync(e => e.EventID == id);
        }

        public async Task UpdateEventAsync(Event updateEvent)
        {
            _appDbContext.Events.Update(updateEvent);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<Event>> GetRecentEventsAsync12Hours()
        {
            var twelveHoursAgo = DateTime.Now.AddHours(-12);
            return await _appDbContext.Events
                .Where(e => e.TimeOfEvent >= twelveHoursAgo)
                .ToListAsync();
        }
    }
}
