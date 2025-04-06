using Microsoft.AspNetCore.Mvc;
using someren_application.Models;
//using someren_application.Repositories;
using someren_application.IRepositories;
using someren_application.DbRepository;

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
            List<Students> students = _studentsRepository.GetAllStudent();
            return View(students);
        }


        [HttpGet]
        public IActionResult Create()
        {
            // Fetch room from the repository
            var rooms = _roomRepository.GetAll() ?? new List<Room>();

            // Create the view model and pass it to the view
            var viewModel = new Students
            {
                Rooms = rooms
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(Students student)
        {
            var room = _roomRepository.GetById(student.Rooms[0].RoomId);
            if (room == null)
            {
                return RedirectToAction("Create");
            }
            //student.Rooms[0] = room;  // Assign the selected room to the student
            var newStudent = new Students
            {
                StudentNumber = student.StudentNumber,
                FirstName = student.FirstName,
                LastName = student.LastName,
                PhoneNumber = student.PhoneNumber,
                StudentClass = student.StudentClass,
                Rooms = new List<Room> { room }  // Assign the selected room to the student
            };
            _studentsRepository.Add(newStudent);  // Add student to database
            return RedirectToAction("Index");  // Redirect after successful addition
            //try
            //{

            //}
            //catch (Exception)
            //{
            //    return View(student);  // Return the student object with validation errors
            //}
        }


        [HttpGet]
        public IActionResult Edit(int studentId)
        {
            Students? students = _studentsRepository.GetStudentsById(studentId);
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
        public IActionResult Delete(int? studentId)
        {
            if (studentId is null)
            {
                return NotFound();
            }
            Students? students = _studentsRepository.GetStudentsById((int)studentId);
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

