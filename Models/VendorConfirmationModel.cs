namespace Models
{
    public class VendorConfirmationModel
    {
        public Guid VendorConfirmationId { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Guid VendorId { get; set; }
        public decimal ActualPrice { get; set; }
        public string? Notes { get; set; }

        public string VendorStatus { get; set; } = "pending_vendor"; // ⭐ Updated

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // JOIN
        public string? VendorName { get; set; }
    }
}
