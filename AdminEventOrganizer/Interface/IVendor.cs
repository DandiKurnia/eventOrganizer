using Models;

namespace AdminEventOrganizer.Interface
{
    public interface IVendor
    {
        Task<IEnumerable<VendorModel>> Get();
        Task<VendorModel?> GetById(Guid id);

        Task UpdateStatus(Guid vendorId, string status);
    }

}
