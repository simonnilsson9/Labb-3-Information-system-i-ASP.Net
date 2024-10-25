using Microsoft.AspNetCore.Identity;

namespace Labb_3_Information_system_i_ASP.Net.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
