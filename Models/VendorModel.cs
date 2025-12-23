
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class VendorModel
    {
        public Guid VendorId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Status { get; set; } = "available";
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }

    public class EditVendorStatusViewModel
    {
        public Guid VendorId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;
    }

}
