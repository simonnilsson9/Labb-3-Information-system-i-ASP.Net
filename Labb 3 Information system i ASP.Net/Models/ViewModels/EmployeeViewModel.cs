namespace Labb_3_Information_system_i_ASP.Net.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public string Id { get; set; } // Användarens ID        
        public string UserName { get; set; } // Användarnamn
        public string Email { get; set; } // E-post
        public string Password { get; set; } // Nytt lösenord
        public string ConfirmPassword { get; set; } // Bekräfta lösenord
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
