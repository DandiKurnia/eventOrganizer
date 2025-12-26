
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class VendorModel
    {
        public Guid VendorId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        // 🔥 dari Users (READ ONLY)
        public string? Email { get; set; }

        // 🔥 dari Users (BOLEH EDIT)
        public string? PhoneNumber { get; set; }
        public string Status { get; set; } = "available";
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<Guid> SelectedCategoryIds { get; set; } = new();
        public List<VendorCategoryModel>? Categories { get; set; }

    }

    public class EditVendorStatusViewModel
    {
        public Guid VendorId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;
    }

}
