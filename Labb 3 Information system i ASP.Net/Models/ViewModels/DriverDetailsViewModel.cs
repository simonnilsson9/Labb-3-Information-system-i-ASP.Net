namespace Labb_3_Information_system_i_ASP.Net.Models.ViewModels
{
    public class DriverDetailsViewModel
    {
        public Driver Driver { get; set; }
        public IEnumerable<Event> Events { get; set; }
    }
}
