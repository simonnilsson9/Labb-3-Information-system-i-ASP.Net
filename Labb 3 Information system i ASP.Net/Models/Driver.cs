using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Labb_3_Information_system_i_ASP.Net.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Driver
    {
        [Key]
        public int DriverID { get; set; }

        [Required(ErrorMessage = "Driver name is required.")]
        [StringLength(25, ErrorMessage = "Driver name cannot exceed 25 characters.")]
        public string DriverName { get; set; }

        [Required(ErrorMessage = "Car registration number is required.")]
        [StringLength(6, ErrorMessage = "Car registration number cannot exceed 6 characters.")]
        public string CarReg { get; set; }

        public IEnumerable<Event> Events { get; set; } = new List<Event>();

        public ApplicationUser ResponsibleEmployee { get; set; }

        [Required(ErrorMessage = "Responsible employee ID is required.")]
        public string ResponsibleEmployeeId { get; set; }

        public decimal TotalAmountIn
        {
            get
            {
                return Events.Where(e => e.DriverId == DriverID).Sum(e => e.AmountIn);
            }
        }

        public decimal TotalAmountOut
        {
            get
            {
                return Events.Where(e => e.DriverId == DriverID).Sum(e => e.AmountOut);
            }
        }

        public decimal NetAmount
        {
            get
            {
                return Events.Any() ? Events.Sum(e => e.AmountIn) - Events.Sum(e => e.AmountOut) : 0;
            }
        }
    }

}
