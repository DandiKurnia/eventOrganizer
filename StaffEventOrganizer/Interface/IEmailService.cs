namespace StaffEventOrganizer.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendVendorRequestEmailAsync(string vendorEmail, string vendorName, string clientName, string packageName, DateTime eventDate);
    }
}
