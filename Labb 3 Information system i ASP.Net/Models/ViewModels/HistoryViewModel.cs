using Microsoft.AspNetCore.Identity;

namespace Labb_3_Information_system_i_ASP.Net.Models.ViewModels
{
    public class HistoryViewModel
    {
        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<Event> RecentEvents { get; set; }
        public IEnumerable<Driver> Drivers { get; set; }
        public IEnumerable<ApplicationUser> Employees { get; set; }        
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }        
        public int? SelectedDriverId { get; set; }
        public string SelectedEmployeeId { get; set; }
    }
}
