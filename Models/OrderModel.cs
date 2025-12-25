namespace Models
{
    public class OrderModel
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public Guid PackageEventId { get; set; }
        public string? AdditionalRequest { get; set; }
        public DateTime EventDate { get; set; }
        public string? Status { get; set; }
        public int ConfirmClient { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public string? PackageName { get; set; }
        public string? ClientName { get; set; }
        public string? ClientPhoneNumber { get; set; }

        public string? ClientEmail { get; set; }

    }


}
