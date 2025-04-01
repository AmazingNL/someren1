using Microsoft.AspNetCore.Mvc;
using someren_application.DbRepository;
using someren_application.IRepositories;
using someren_application.Models;
using someren_application.Repositories;

namespace someren_application.Controllers
{
    public class OrderDrinkController : Controller
    {
        private readonly IOrderDrinkRepository _orderDrinkRepository;
        private readonly IStudentsRepository _studentsRepository;
        private readonly IDrinksRepository _drinksRepository;

        public OrderDrinkController(IOrderDrinkRepository orderDrinkRepository, IStudentsRepository studentsRepository, IDrinksRepository drinksRepository)
        {
            _orderDrinkRepository = orderDrinkRepository;
            _studentsRepository = studentsRepository;
            _drinksRepository = drinksRepository;
        }
        public IActionResult Index()
        {
            List<OrderDrinks> OrderDrinks = _orderDrinkRepository.GetAllOrders();
            return View(OrderDrinks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Fetch students and drinks from the repository
            var students = _studentsRepository.GetAllStudent() ?? new List<Students>();
            var drinks = _drinksRepository.GetAllDrink() ?? new List<Drinks>();

            // Check if lists are empty, and return an error or redirect
            if (!students.Any())
            {
                TempData["ErrorMessage"] = "No students available. Please add students before creating an order.";
            }

            if (!drinks.Any())
            {
                TempData["ErrorMessage"] = "No drinks available. Please add drinks before creating an order.";
            }

            // Create the view model and pass it to the view
            var viewModel = new OrderDrinks
            {
                Students = students,
                Drinks = drinks
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Create(OrderDrinks order)
        {
            try
            {
                // Ensure that the TotalDrink is valid
                if (order.TotalDrink <= 0)
                {
                    ModelState.AddModelError("TotalDrink", "The quantity of drinks must be greater than zero.");
                    return View(order);
                }

                // Get the list of all drinks
                Drinks? drink = _drinksRepository.GetById(order.Drinks[0].DrinkId);

                // Check if the selected drink exists and has enough quantity
                if (drink != null && drink.Quantity >= order.TotalDrink)
                {
                    // Update the drink quantity
                    drink.Quantity -= order.TotalDrink;
                    _drinksRepository.Update(drink);  // This would persist the updated drink quantity

                    // Create a new order entry
                    var newOrder = new OrderDrinks
                    {
                        Students = order.Students,
                        Drinks = order.Drinks,
                        TotalDrink = order.TotalDrink
                    };
                    _orderDrinkRepository.AddOrder(newOrder);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Insufficient stock for the selected drink.");
                    return View(order);
                }
            }
            catch (Exception)
            {
                return View(order);
            }
        }



        //Get user
        [HttpGet]
        public IActionResult Delete(int? orderId)
        {
            if (orderId is null)
            {
                return NotFound();
            }
            else
            {
                //get user via repository
                OrderDrinks? orderDrinks = _orderDrinkRepository.GetOrdersById((int)orderId);
                return View(orderDrinks);
            }
        }
        [HttpPost]
        public IActionResult Delete(OrderDrinks order)
        {
            try
            {
                //delete user via repository
                _orderDrinkRepository.DeleteOrder(order);

                //go back to user list(via Index)
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                //something went wrong, go back to view with user
                return View(order);
            }
        }

    }

}
