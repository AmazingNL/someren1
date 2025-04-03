using someren_application.Models;

namespace someren_application.IRepositories
{
    public interface IOrderDrinkRepository
    {
        List<OrderDrinks> GetAllOrders(); 
        void AddOrder(OrderDrinks orderDrinks);
        void DeleteOrder(OrderDrinks orderDrinks);
        void UpdateOrder(OrderDrinks orderDrinks);
        OrderDrinks GetOrdersById(int orderId);
    }
}
