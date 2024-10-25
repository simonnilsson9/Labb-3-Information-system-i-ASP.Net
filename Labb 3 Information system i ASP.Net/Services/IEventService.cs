using Labb_3_Information_system_i_ASP.Net.Models;

namespace Labb_3_Information_system_i_ASP.Net.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<IEnumerable<Event>> GetAllEventsFromDriver(int driverId);
        Task<Event> GetEventAsyncById(int id);
        Task CreateEventAsync(Event newEvent);
        Task UpdateEventAsync(Event updateEvent);
        Task DeleteEventAsync(Event deleteEvent);
        Task<IEnumerable<Event>> GetRecentEventsAsync12Hours();

    }
}
