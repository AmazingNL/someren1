 using Microsoft.AspNetCore.Mvc;
using someren_application.IRepositories;
using someren_application.Models;
using someren_application.Repositories;

namespace someren_application.Controllers
{
    public class DrinksController : Controller
    {
        private readonly IDrinksRepository _drinkRepository;

        public DrinksController(IDrinksRepository drinkRepository)
        {
            _drinkRepository = drinkRepository;
        }

        public IActionResult Index()
        {
            List<Drinks> drinks = _drinkRepository.GetAllDrink();
            return View(drinks);
        }

        [HttpPost]
        public IActionResult Filter(string isAlcoholic)
        {
            try
            {
                List<Drinks> drinks = _drinkRepository.Filter(isAlcoholic);
                return View(drinks);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {

                return RedirectToAction("Index");
            }
      
        }

        [HttpPost]
        public IActionResult Create(Drinks drink)
        {
            try
            {
                _drinkRepository.Add(drink);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(drink);
            }
        }

        [HttpGet]
        public IActionResult Edit(int drinkId)
        {
            try
            {
                Drinks? drink = _drinkRepository.GetById(drinkId);
                return View(drink);
            }
            catch (Exception)
            {

                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public IActionResult Edit(Drinks drink)
        {
            try
            {
                _drinkRepository.Update(drink);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(drink);
            }
        }

        [HttpGet]
        public IActionResult Delete(int? drinkId)
        {
            if (drinkId is null)
            {
                return NotFound();
            }
            else
            {
                Drinks? drink = _drinkRepository.GetById((int)drinkId);
                return View(drink);
            }
        }

        [HttpPost]
        public IActionResult Delete(Drinks drink)
        {
            try
            {
                _drinkRepository.Delete(drink);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(drink);
            }
        }
    }
}
