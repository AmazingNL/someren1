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
            var students = _studentsRepository.GetAllStudents();
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
        public IActionResult Edit(int studentId)
        {
            Students? students = _studentsRepository.GetStudentById(studentId);
            return View(students);
        }

        [HttpPost]
        public IActionResult Edit(Students students)
        {
            try
            {
                _studentsRepository.Edit(students);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View(students);
            }
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }
            else
            {
                //get user via repository
                Students? students = _studentsRepository.GetStudentById((int)id);
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
