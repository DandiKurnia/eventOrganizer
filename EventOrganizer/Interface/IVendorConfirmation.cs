using Models;

namespace EventOrganizer.Interface
{
    public interface IVendorConfirmation
    {
        Task<VendorConfirmationModel> Create(VendorConfirmationModel model);
        Task<VendorConfirmationModel?> GetById(Guid id);
        Task<IEnumerable<VendorConfirmationModel>> GetByOrderId(Guid orderId);

        Task UpdateStatus(Guid vendorConfirmationId, string newStatus);
        Task CloseOtherVendors(Guid orderId, Guid vendorId);
    }
}
