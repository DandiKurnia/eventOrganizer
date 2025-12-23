using Models;
namespace StaffEventOrganizer.Interface
{
    public interface IOrder
    {
        Task<IEnumerable<OrderModel>> GetAll();

    }
}
