using Labb_3_Information_system_i_ASP.Net.Models;
using Labb_3_Information_system_i_ASP.Net.Models.ViewModels;
using Labb_3_Information_system_i_ASP.Net.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Labb_3_Information_system_i_ASP.Net.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {        
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmployeeController(IAdminService adminService, UserManager<ApplicationUser> userManager)
        {
            _adminService = adminService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _adminService.GetAllEmployeesAsync();
            return View(employees); 
        }

        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser employee, string password)
        {
            if (ModelState.IsValid)
            {
                await _adminService.CreateEmployeeAsync(employee, password);
                return RedirectToAction("Index", "Admin");
            }
            return View(employee); // Återgå till formuläret om det är ogiltigt
        }
        
        public async Task<IActionResult> Details(string id)
        {
            var employee = await _adminService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound(); // Om anställd inte hittas, returnera 404
            }
            return View(employee); // Visa anställdens detaljer
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var employee = await _adminService.GetEmployeeById(id); // Hämta den anställde
            if (employee == null)
            {
                return NotFound(); // Om anställd inte hittas, returnera 404
            }

            var model = new EmployeeViewModel
            {
                Id = employee.Id,
                UserName = employee.UserName,
                Email = employee.Email,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
            };

            return View(model); // Skicka modellen till vyn för att redigera
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kontrollera om lösenorden matchar
                if (!string.IsNullOrEmpty(model.Password) && model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "Passwords do not match.");
                    return View(model); // Återgå till formuläret om lösenorden inte matchar
                }

                // Hämta användaren med ID
                var employee = await _adminService.GetEmployeeById(model.Id);
                if (employee == null)
                {
                    return NotFound(); // Om anställd inte hittas, returnera 404
                }

                // Uppdatera användarens detaljer
                employee.UserName = model.UserName;
                employee.Email = model.Email;
                employee.FirstName = model.FirstName;
                employee.LastName = model.LastName;

                // Om ett nytt lösenord anges, sätt det
                if (!string.IsNullOrEmpty(model.Password))
                {
                    // Använd UserManager för att uppdatera lösenordet (om nödvändigt)
                    var token = await _userManager.GeneratePasswordResetTokenAsync(employee);
                    await _userManager.ResetPasswordAsync(employee, token, model.Password);
                }

                await _adminService.UpdateEmployeeAsync(employee); // Uppdatera anställd
                return RedirectToAction("Index", "Admin");
            }

            return View(model); // Återgå till formuläret om modellen är ogiltig
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var employee = await _adminService.GetEmployeeById(id);
            if (employee != null)
            {
                await _adminService.DeleteEmployeeAsync(employee); // Anropa borttagningsmetoden
            }
            return RedirectToAction("Index", "Admin");
        }

        // Visa bekräftelseformulär för borttagning
        public async Task<IActionResult> Delete(string id)
        {
            var employee = await _adminService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound(); // Om anställd inte hittas, returnera 404
            }
            return View(employee); // Visa bekräftelse för att ta bort anställd
        }
    }
}
