

namespace Models
{
    public class PackagePhoto
    {
        public Guid PhotoId { get; set; } = Guid.NewGuid();
        public Guid PackageEventId { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public byte[]? Foto { get; set; }
        
        public string? FotoContentType { get; set; }
    }
}
