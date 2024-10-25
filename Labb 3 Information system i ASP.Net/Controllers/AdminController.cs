using Labb_3_Information_system_i_ASP.Net.Models;
using Labb_3_Information_system_i_ASP.Net.Models.ViewModels;
using Labb_3_Information_system_i_ASP.Net.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Labb_3_Information_system_i_ASP.Net.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IDriverService _driverService;
        private readonly IEventService _eventService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService, IDriverService driverService, IEventService eventService, UserManager<ApplicationUser> userManager)
        {
            _adminService = adminService;
            _driverService = driverService;
            _eventService = eventService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? driverId, string employeeId, int? hours)
        {
            // Hämta alla händelser
            var allEvents = await _eventService.GetAllEventsAsync();

            // Filter recent events based on selected hours
            IEnumerable<Event> recentEvents;

            if (hours.HasValue)
            {
                var cutOffTime = DateTime.Now.AddHours(-hours.Value);
                recentEvents = allEvents.Where(e => e.TimeOfEvent >= cutOffTime).ToList();
            }
            else
            {
                // Default to the last 24 hours if no hours are specified
                recentEvents = allEvents.Where(e => e.TimeOfEvent >= DateTime.Now.AddHours(-24)).ToList();
            }

            // Filtrera händelser baserat på angivna datum
            var filteredEvents = allEvents; // Start with all events for filtering
            if (startDate.HasValue)
            {
                filteredEvents = filteredEvents.Where(e => e.TimeOfEvent >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                filteredEvents = filteredEvents.Where(e => e.TimeOfEvent <= endDate.Value).ToList();
            }

            // Filtrera händelser baserat på vald förare (om angiven)
            if (driverId.HasValue && driverId.Value > 0)
            {
                filteredEvents = filteredEvents.Where(e => e.DriverId == driverId.Value).ToList();
            }

            // Hämta förare och anställda för filtreringsalternativ
            var drivers = await _driverService.GetAllDriversAsync();
            var employees = await _adminService.GetAllEmployeesAsync();

            // Skapa en SelectList för förare
            ViewBag.DriverSelectList = new SelectList(drivers, "DriverID", "DriverName", driverId);

            // Skapa en vymodell
            var viewModel = new HistoryViewModel
            {
                Events = filteredEvents,
                RecentEvents = recentEvents,
                Drivers = drivers,
                Employees = employees,
                StartDate = startDate,
                EndDate = endDate,
                SelectedDriverId = driverId,
                SelectedEmployeeId = employeeId
            };

            return View(viewModel); // Skicka vymodellen till vyn
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentEvents(int hours, int? driverId)
        {
            // Fetch all events from the database
            var allEvents = await _eventService.GetAllEventsAsync();

            // Filter recent events based on selected hours
            var cutOffTime = DateTime.Now.AddHours(-hours);
            var recentEvents = allEvents.Where(e => e.TimeOfEvent >= cutOffTime);

            // Further filter by selected driver if provided
            if (driverId.HasValue && driverId.Value > 0)
            {
                recentEvents = recentEvents.Where(e => e.DriverId == driverId.Value);
            }

            // Return the partial view with recent events
            return PartialView("_RecentEventsTable", recentEvents.ToList());
        }


    }
}
