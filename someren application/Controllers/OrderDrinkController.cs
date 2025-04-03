using Microsoft.AspNetCore.Mvc;
using someren_application.IRepositories;
using someren_application.Models;

namespace someren_application.Controllers
{
    public class OrderDrinkController : Controller
    {
        private readonly IOrderDrinkRepository _orderDrinkRepository;
        private readonly IDrinksRepository _drinksRepository;
        private readonly IStudentsRepository _studentsRepository;

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

            // Create the view model and pass it to the view
            var viewModel = new OrderDrinks
            {
                Students = students,
                Drinks = drinks,
            };

            return View(viewModel);
        }


        // Action to handle the confirmation page after the form is submitted
        [HttpPost]
        public IActionResult CreateConfirm(OrderDrinks order)
        {
            try
            {
                // Fetch the selected student and drink based on IDs
                var selectedStudent = _studentsRepository.GetStudentsById(order.Students[0].StudentId);
                var selectedDrink = _drinksRepository.GetById(order.Drinks[0].DrinkId);
                var totalDrink = order.TotalDrink;

                // Check for null references
                if (selectedStudent == null || selectedDrink == null)
                {
                    return RedirectToAction("Create");
                }

                // Process the order if everything is valid
                selectedDrink.Quantity -= totalDrink; // Update stock
                _drinksRepository.Update(selectedDrink); // Persist changes

                // Create a view model to pass to the confirmation page
                var confirmationModel = new OrderDrinks
                {
                    Students = new List<Students> { selectedStudent },
                    Drinks = new List<Drinks> { selectedDrink },
                    TotalDrink = totalDrink,
                };
                _orderDrinkRepository.AddOrder(confirmationModel); // Save the new order

                // Successfully processed, redirect to an index or order list page
                return View(confirmationModel); // Show confirmation page
            }
            catch (Exception)
            {
                return RedirectToAction("Create");
            }
        }


        // Action to handle the actual creation of the order
        [HttpPost]
        public IActionResult CreateConfirmed(OrderDrinks order)
        {
            ////Fetch the selected drink and student from repositories
            //var selectedDrink = _drinksRepository.GetById(order.Drinks[0].DrinkId);
            //var selectedStudent = _studentsRepository.GetStudentsById(order.Students[0].StudentId);
            //var totalDrink = order.TotalDrink;
            //var confirmationModel = new OrderDrinks
            //{
            //    Students = order.Students,
            //    Drinks = order.Drinks,
            //    TotalDrink = order.TotalDrink,
            //};

            // Successfully processed, redirect to an index or order list page
            return RedirectToAction("Index");
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
