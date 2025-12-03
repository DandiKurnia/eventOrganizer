using Models;

namespace EventOrganizer.Interface
{
    public interface IOrder
    {
        Task<IEnumerable<OrderModel>> Get(Guid userId);   // untuk client
        Task<IEnumerable<OrderModel>> GetAll();           // untuk staff
        Task<OrderModel?> GetById(Guid orderId);          // 🔥 tambahkan ini
        Task Create(OrderModel model);
        Task<bool> ConfirmOrder(Guid orderId);
        Task UpdateStatus(Guid orderId, string status);


    }
}
