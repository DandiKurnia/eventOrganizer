namespace Models
{
    public class OrderModel
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public Guid PackageEventId { get; set; }
        public string AdditionalRequest { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PackageName { get; set; } // pastikan ada
    }


}
