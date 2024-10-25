using Labb_3_Information_system_i_ASP.Net.Models;
using Labb_3_Information_system_i_ASP.Net.Models.ViewModels;
using Labb_3_Information_system_i_ASP.Net.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Labb_3_Information_system_i_ASP.Net.Controllers
{
    [Authorize(Roles = "Employee, Admin")]
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IDriverService _driverService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(IEventService eventService, IDriverService driverService, UserManager<ApplicationUser> userManager)
        {
            _eventService = eventService;
            _driverService = driverService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> CreateEvent()
        {
            var drivers = await _driverService.GetAllDriversAsync(); 
            ViewBag.Drivers = new SelectList(drivers, "DriverID", "DriverName"); 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(Event newEvent)
        {
            if (ModelState.IsValid)
            {                
                await _eventService.CreateEventAsync(newEvent);
                
                var driver = await _driverService.GetDriverByIdAsync(newEvent.DriverId);
                
                var eventsList = driver.Events.ToList();
                eventsList.Add(newEvent);
                driver.Events = eventsList;
                
                await _driverService.UpdateDriverAsync(driver);
                                
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToAction("Index");
                }

                var roles = await _userManager.GetRolesAsync(currentUser);
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (roles.Contains("Employee"))
                {
                    return RedirectToAction("Index", "Events");
                }
            }

            // Om modellen är ogiltig, hämta förare igen för att fylla i dropdown
            var drivers = await _driverService.GetAllDriversAsync();
            ViewBag.Drivers = new SelectList(drivers, "DriverID", "DriverName");

            return View(newEvent);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var eventDetails = await _eventService.GetEventAsyncById(id);
            if (eventDetails == null)
            {
                return NotFound(); 
            }
            return View(eventDetails);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var eventToEdit = await _eventService.GetEventAsyncById(id);
            if (eventToEdit == null)
            {
                return NotFound(); // Returnera 404 om händelsen inte hittas
            }

            var drivers = await _driverService.GetAllDriversAsync(); // Hämta alla förare
            ViewBag.Drivers = new SelectList(drivers, "DriverID", "DriverName", eventToEdit.DriverId); // Skapa en SelectList och förvalda förare

            return View(eventToEdit); // Skicka händelsen till vyn för att redigera
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Event updateEvent)
        {
            if (ModelState.IsValid)
            {
                await _eventService.UpdateEventAsync(updateEvent);
                return RedirectToAction("Index"); // Omdirigera till listan över händelser
            }

            // Om modellen är ogiltig, hämta förare igen för att fylla i dropdown
            var drivers = await _driverService.GetAllDriversAsync();
            ViewBag.Drivers = new SelectList(drivers, "DriverID", "DriverName", updateEvent.DriverId);

            return View(updateEvent); // Visa formuläret igen med felmeddelanden
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var eventToDelete = await _eventService.GetEventAsyncById(id);
            if (eventToDelete == null)
            {
                return NotFound(); 
            }

            await _eventService.DeleteEventAsync(eventToDelete);
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            // Hämta alla händelser
            var events = await _eventService.GetAllEventsAsync();

            // Filtrera händelser baserat på angivna datum
            if (startDate.HasValue)
            {
                events = events.Where(e => e.TimeOfEvent >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                events = events.Where(e => e.TimeOfEvent <= endDate.Value).ToList();
            }

            // Hämta händelser registrerade under de senaste 12 timmarna
            var recentEvents = await _eventService.GetRecentEventsAsync12Hours();

            // Skicka datumen tillbaka till vyn
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            // Skapa en vymodell för att skicka både alla händelser och de senaste händelserna
            var viewModel = new EventsIndexViewModel
            {
                AllEvents = events,
                RecentEvents = recentEvents
            };

            return View(viewModel); // Skicka vymodellen till vyn
        }

    }
}
