using Models;

namespace StaffEventOrganizer.Interface
{
    public interface IVendor
    {
        Task<IEnumerable<VendorModel>> Get();
        Task<VendorModel?> GetById(Guid id);

        Task UpdateStatus(Guid vendorId, string status);
        Task<IEnumerable<VendorModel>> GetAvailableVendors();

        Task<IEnumerable<VendorConfirmationModel>> GetByOrderId(Guid orderId);
        Task<bool> HasAvailableVendorByPackage(Guid packageEventId, Guid orderId);

        Task SendVendorRequestByPackage(Guid orderId, Guid packageEventId);

        Task<IEnumerable<VendorCategoryModel>> GetVendorCategories(Guid vendorId);
    }
}
