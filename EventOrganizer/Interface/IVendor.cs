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

        Task<IEnumerable<OrderModel>> GetOrders(Guid vendorId, string? search);
        Task<OrderModel?> GetOrderDetail(Guid orderId);

        Task AddVendorCategories(Guid vendorId, List<Guid> categoryIds);
        Task<IEnumerable<VendorCategoryModel>> GetVendorCategories(Guid vendorId);
        Task UpdateVendorInfo(VendorModel model);
        Task UpdateUserPhone(Guid userId, string? phoneNumber);

        Task<IEnumerable<VendorModel>> GetAvailableVendors();

        Task<VendorDashboardViewModel> GetVendorDashboard(Guid vendorId);


    }
}
