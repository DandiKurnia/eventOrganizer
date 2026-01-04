namespace Models
{
    public class VendorConfirmationModel
    {
        public Guid VendorConfirmationId { get; set; }
        public Guid OrderId { get; set; }
        public Guid VendorId { get; set; }

        public decimal ActualPrice { get; set; }
        public string? Notes { get; set; }
        public string VendorStatus { get; set; } = "pending_vendor";
        public DateTime CreatedAt { get; set; }

        // ===== JOIN RESULT =====
        public string? VendorName { get; set; }
        public string? VendorPhoneNumber { get; set; }

        public string? PackageName { get; set; }
        public decimal BasePrice { get; set; }

        public string? AdditionalRequest { get; set; }
        public DateTime EventDate { get; set; }
    }



    public class VendorOrderViewModel
    {
        public Guid VendorConfirmationId { get; set; }
        public Guid OrderId { get; set; }

        public string ClientName { get; set; } = "";
        public string ClientEmail { get; set; } = "";
        public string PackageName { get; set; } = "";
        public DateTime EventDate { get; set; }

        public decimal ActualPrice { get; set; }
        public string VendorStatus { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
