namespace someren_application.Models
{
    public class OrderDrinks
    {
        public OrderDrinks(int orderId, int totalDrink, List<Students> students, List<Drinks> drinks)
        {
            OrderId = orderId;
            TotalDrink = totalDrink;
            Students = students;
            Drinks = drinks;
        }
        public OrderDrinks()
        {
            OrderId = 0;
            TotalDrink = 0;
            Students = new List<Students>();
            Drinks = new List<Drinks>();
        }

        public int OrderId { get; set; }
        public List<Students> Students { get; set; }
        public List<Drinks> Drinks { get; set; }
        public int TotalDrink { get; set; }

    }
}
