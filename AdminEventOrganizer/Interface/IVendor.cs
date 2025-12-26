using Models;

namespace AdminEventOrganizer.Interface
{
    public interface IVendor
    {
        Task<IEnumerable<VendorModel>> Get();
        Task<VendorModel?> GetById(Guid id);

        Task UpdateStatus(Guid vendorId, string status);
        Task<IEnumerable<VendorModel>> GetAvailableVendors();

        Task<IEnumerable<VendorConfirmationModel>> GetByOrderId(Guid orderId);

        Task SendVendorRequest(Guid orderId, Guid vendorId);
        Task<IEnumerable<VendorCategoryModel>> GetVendorCategories(Guid vendorId);

    }

}
