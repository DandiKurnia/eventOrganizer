
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class VendorModel
    {
        public Guid VendorId { get; set; }

        [Required(ErrorMessage = "Vendor Name is required")]
        public string VendorName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone  { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;


    }
}
