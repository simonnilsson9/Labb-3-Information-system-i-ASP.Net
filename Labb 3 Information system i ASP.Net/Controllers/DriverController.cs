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
    [Authorize(Roles = "Employee, Admin")]
    public class DriverController : Controller
    {
        private readonly IDriverService _driverService;
        private readonly IAdminService _adminService;
        private readonly IEventService _eventService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DriverController(IDriverService driverService, UserManager<ApplicationUser> userManager, IEventService eventService, IAdminService adminService)
        {
            _driverService = driverService;
            _userManager = userManager;
            _eventService = eventService;
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = null)
        {
            var drivers = string.IsNullOrWhiteSpace(searchTerm)
                ? await _driverService.GetAllDriversAsync() // Hämta alla förare om inget sökord anges
                : await _driverService.SearchDriversByNameAsync(searchTerm); // Hämta förare som matchar sökordet

            ViewBag.SearchTerm = searchTerm;
            return View(drivers); // Skicka förare till vyn
        }

        [HttpGet]
        public async Task<IActionResult> CreateDriver()
        {
            var employees = await _adminService.GetAllEmployeesAsync();
            
            var employeeList = employees.Select(e => new
            {
                Id = e.Id,
                FullName = $"{e.FirstName} {e.LastName}",
            }).ToList();
            ViewBag.Employees = new SelectList(employeeList, "Id", "FullName");
            
            return View(); // Returnera vyn
        }

        [HttpPost]
        public async Task<IActionResult> CreateDriver(Driver driver)
        {
            if (ModelState.IsValid)
            {
                // Om dropdown för ansvarig anställd har valts, sätt ResponsibleEmployeeId
                if (!string.IsNullOrEmpty(driver.ResponsibleEmployeeId))
                {
                    
                }

                await _driverService.CreateDriverAsync(driver);
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
                    return RedirectToAction("Index", "Driver");
                }
            }
            return View(driver);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsOfDriver(int driverId)
        {
            var driver = await _driverService.GetDriverByIdAsync(driverId);
            if (driver == null)
            {
                return NotFound(); // Returnera 404 om föraren inte hittas
            }

            // Hämta händelser kopplade till föraren
            var events = await _eventService.GetAllEventsFromDriver(driverId);

            // Skapa en vymodell som innehåller både föraren och händelserna
            var viewModel = new DriverDetailsViewModel
            {
                Driver = driver,
                Events = events
            };

            return View(viewModel); // Skicka vymodellen till vyn
        }

        [HttpGet]
        public async Task<IActionResult> UpdateDriver(int id)
        {
            var driver = await _driverService.GetDriverByIdAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            var employees = await _adminService.GetAllEmployeesAsync();
            var employeeList = employees.Select(e => new
            {
                Id = e.Id,
                FullName = $"{e.FirstName} {e.LastName}",
            }).ToList();
            ViewBag.Employees = new SelectList(employeeList, "Id", "FullName", driver.ResponsibleEmployeeId); 

            return View(driver); 
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDriver(Driver driver)
        {
            if (ModelState.IsValid)
            {
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
                    return RedirectToAction("Index", "Driver");
                }
            }
            
            var employees = await _userManager.Users.ToListAsync();
            ViewBag.Employees = new SelectList(employees, "Id", "UserName");

            return View(driver); 
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int driverId)
        {            
            var driver = await _driverService.GetDriverByIdAsync(driverId);
            if (driver == null)
            {
                return NotFound(); 
            }
                        
            await _driverService.DeleteDriverAsync(driver);

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
                return RedirectToAction("Index", "Driver"); 
            }

            return RedirectToAction("Index"); 
        }

        public async Task<IActionResult> DetailsByName(string name)
        {
            var driver = await _driverService.GetDriverByName(name);
            return View(driver);
        }

        [HttpGet]
        public async Task<IActionResult> FilteredEvents(int driverId, DateTime? startDate, DateTime? endDate)
        {
            // Hämta alla händelser kopplade till föraren
            var events = await _eventService.GetAllEventsFromDriver(driverId);

            // Filtrera händelser baserat på start- och slutdatum
            if (startDate.HasValue)
            {
                events = events.Where(e => e.TimeOfEvent >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                events = events.Where(e => e.TimeOfEvent <= endDate.Value).ToList();
            }

            // Hämta information om föraren
            var driver = await _driverService.GetDriverByIdAsync(driverId);

            // Spara datumen i ViewBag för att använda i vyn
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            // Skapa en vymodell som innehåller föraren och filtrerade händelser
            var viewModel = new DriverDetailsViewModel
            {
                Driver = driver,
                Events = events
            };

            // Skicka vymodellen till vyn
            return View("DetailsOfDriver", viewModel);
        }
    }
}
