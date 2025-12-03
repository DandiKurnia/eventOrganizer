using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class PackageEventModel
    {
        public Guid PackageEventId { get; set; }
        public Guid MainPhotoId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int BasePrice { get; set; }
        public string Status { get; set; } = string.Empty;

        public List<PackagePhoto> Photos { get; set; } = new();

    }
}
