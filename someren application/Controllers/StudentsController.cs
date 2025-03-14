using Microsoft.AspNetCore.Mvc;
using someren_application.Models;
using someren_application.Repositories;

namespace someren_application.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentsRepository _studentsRepository;

        public StudentsController(IStudentsRepository studentsRepository)
        {
            _studentsRepository = studentsRepository;
        }

        public IActionResult Index()
        {
            List<Students> students = _studentsRepository.GetAll();
            return View(students);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Students students)
        {
            try
            {
                _studentsRepository.Add(students);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View(students);
            }
        }

        [HttpGet]
        public IActionResult Update(int studentId)
        {
            Students? students = _studentsRepository.GetById(studentId);
            return View(studentId);
        }

        [HttpPost]
        public IActionResult Update(Students students)
        {
            try
            {
                _studentsRepository.Update(students);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View(students);
            }
        }

        [HttpPost]
        public IActionResult Delete(Students students)
        {
            try
            {
                _studentsRepository.Delete(students);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(students);
            }
        }
    }
}
