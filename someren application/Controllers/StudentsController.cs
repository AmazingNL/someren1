using Microsoft.AspNetCore.Mvc;
using someren_application.Models;
using someren_application.Repositories;

namespace someren_application.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentsRepository _studentsRepository;
        private readonly IRoomRepository _roomRepository;
        public StudentsController(IStudentsRepository studentsRepository, IRoomRepository roomRepository)
        {
            _studentsRepository = studentsRepository;
            _roomRepository = roomRepository;
        }

        public IActionResult Index()
        {
            var students = _studentsRepository.GetAllStudents();
            return View(students);
        }
        

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Rooms = _roomRepository.GetAll();
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
                ViewBag.Rooms = _roomRepository.GetAll();                
                return View(students);
            }
        }

        [HttpGet]
        public IActionResult Edit(int studentId)
        {
            Students? students = _studentsRepository.GetStudentById(studentId);
            if (students == null)
            {
                return NotFound();
            }
            ViewBag.Rooms = _roomRepository.GetAll();
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
            Students? students = _studentsRepository.GetStudentById((int)id);
            if (students == null)
            {
                return NotFound();
            }
            return View(students);
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
