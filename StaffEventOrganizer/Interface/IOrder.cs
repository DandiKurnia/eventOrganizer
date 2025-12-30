using Models;
namespace StaffEventOrganizer.Interface
{
    public interface IOrder
    {
        Task<IEnumerable<OrderModel>> GetAll();
        Task<OrderModel?> GetById(Guid orderId);
        Task<bool> ConfirmOrder(Guid orderId);
        Task UpdateStatus(Guid orderId, string status);
    }
}
