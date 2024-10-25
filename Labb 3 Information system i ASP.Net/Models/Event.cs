using System.ComponentModel.DataAnnotations;

namespace Labb_3_Information_system_i_ASP.Net.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Driver ID is required.")]
        public int DriverId { get; set; }

        public Driver Driver { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Time of event is required.")]
        public DateTime TimeOfEvent { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Amount In is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount In must be a positive value.")]
        public decimal AmountIn { get; set; }

        [Required(ErrorMessage = "Amount Out is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount Out must be a positive value.")]
        public decimal AmountOut { get; set; }

        // Calculated total for the event
        public decimal NetAmount => AmountIn - AmountOut;
    }
}
