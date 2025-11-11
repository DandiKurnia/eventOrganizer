using Models;
namespace EventOrganizer.Interface
{
    public interface IOrder
    {
        Task<IEnumerable<OrderModel>> Get(Guid userId);
        Task<OrderModel> GetById(Guid orderId);
    }
}
