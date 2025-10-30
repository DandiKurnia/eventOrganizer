using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class PackageEventModel
    {
        public Guid PackageEventId { get; set; }

        [Required(ErrorMessage = "Vendor Name is required")]
        public string PackageName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int BasePrice { get; set; }
        public string Status { get; set; } = string.Empty;

    }
}
