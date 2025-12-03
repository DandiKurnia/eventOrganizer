using Models;

namespace AdminEventOrganizer.Interface
{
    public interface IVendor
    {
        Task<IEnumerable<VendorModel>> Get();
        Task<VendorModel?> GetById(Guid id);
        Task<VendorModel> Update(VendorModel model);
        Task Delete(Guid id);
    }
}
