using Microsoft.AspNetCore.Mvc;
using someren_application.Models;
using someren_application.Repositories;

namespace someren_application.Controllers
{
    public class LecturerController : Controller

    {


        private ILecturerRepository _lecturerRpository;
        private readonly IRoomRepository _roomRepository;

        public LecturerController(ILecturerRepository lecturerRpository, IRoomRepository roomRepository)

        {
            _lecturerRpository = lecturerRpository; //Properly using the injected repository
            _roomRepository = roomRepository;
        }


        public IActionResult Index()
        {
            //get All lecturer repstory
            List<Lecturer> lecturers = _lecturerRpository.GetAll();
            return View(lecturers);


        }



        // GET: Lecturer/Create
        [HttpGet]
        public IActionResult Create()
        {
            //return View();
            ViewBag.Rooms = _roomRepository.GetAll();
            return View();
        }


        //// POST: Lecturer/Create
        [HttpPost]   
        public IActionResult Create(Lecturer lecturer)
        {
            try
            {
                //add users via repostory
                _lecturerRpository.Add(lecturer);

                //go back to user list  (via index)
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                ViewBag.Rooms = _roomRepository.GetAll();
                return View(lecturer);
            }

        }




        ///GET:  Lecturer/ Edit
        [HttpGet]
        public IActionResult Edit(int? id)
        {

            if (id is null)
            {
                return NotFound();
            }
            else
            {
                //get user via repository
                Lecturer? lecturer = _lecturerRpository.GetById((int)id);
                return View(lecturer);
            }
        }

        ///POST:  Lecturer/ Edit
        [HttpPost]
        public IActionResult Edit(Lecturer lecturer)
        {
            try
            {
                //add users via repository
                _lecturerRpository.Update(lecturer);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(lecturer);
            }
        }


        ///GET:  Lecturer/ DELETE
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
                Lecturer? lecturer = _lecturerRpository.GetById((int)id);
                return View(lecturer);
            }
        }

        [HttpPost] ///POST:  Lecturer/ DELETE
        public IActionResult Delete(Lecturer lecturer)
        {
            try
            {
                //delete user via repository
                _lecturerRpository.Delete(lecturer);

                //go back to user list(via Index)
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                //something went wrong, go back to view with user
                return View(lecturer);
            }
        }

    }
}

