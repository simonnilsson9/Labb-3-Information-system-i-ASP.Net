namespace Labb_3_Information_system_i_ASP.Net.Models.ViewModels
{
    public class EventsIndexViewModel
    {
        public IEnumerable<Event> AllEvents { get; set; }
        public IEnumerable<Event> RecentEvents { get; set; }
    }
}
