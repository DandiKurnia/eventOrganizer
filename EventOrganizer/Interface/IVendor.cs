using Models;

namespace EventOrganizer.Interface
{
    public interface IVendor
    {
        Task<IEnumerable<VendorModel>> Get();
        Task<VendorModel> GetById(Guid vendorId);
        Task<VendorModel> GetVendorByUserId(Guid userId);
        Task Add(VendorModel vendor);
        Task UpdateStatus(Guid vendorId, string status);
    }
}
