using Microsoft.AspNetCore.Mvc;
using someren_application.Models;
using someren_application.Repositories;

namespace someren_application.Controllers
{
    public class LecturerController : Controller

    {


        private ILecturerRepository _lecturerRpository;

        //public UsersController(IUsersRepository DbUsersRepository)
        //{   


        //    //ideallly via injection 
        //    // userRepository = userRepository;
        //    _usersRepository = new DbUsersRepository();
        //}
        public LecturerController(ILecturerRepository lecturerRpository)

        {
            _lecturerRpository = lecturerRpository; // ✅ Properly using the injected repository
        }


        public IActionResult Index()
        {
            //get All users repstory
            List<Lecturer> lecturers = _lecturerRpository.GetAll();
            return View(lecturers);

            //returen View( _usersRepository.GetAll());

        }
    }
}
//        [HttpGet]
//        public IActionResult Create()
//        {
//            return View();

//        }
//        [HttpPost]
//        public IActionResult Create(Lecturer lecturer)
//        {
//            try
//            {
//                //add users via repostory
//                _lecturerRpository.Add(lecturer);

//                //go back to user list  (via index)
//                return RedirectToAction("Index");

//            }
//            catch (Exception ex)
//            {
//                return View(lecturer);
//            }

//        }




//        //Get user 
//        [HttpGet]
//        public IActionResult Delete(int? id)
//        {
//            if (id is null)
//            {
//                return NotFound();
//            }
//            else
//            {
//                //get user via repository
//                User? user = _usersRepository.GetById((int)id);
//                return View(user);
//            }
//        }

//        public IActionResult Delete(User user)
//        {
//            try
//            {
//                //delete user via repository
//                _usersRepository.Delete(user);

//                //go back to user list(via Index)
//                return RedirectToAction("Index");
//            }
//            catch (Exception ex)
//            {
//                //something went wrong, go back to view with user
//                return View(user);
//            }
//        }



//        //edit
//        [HttpGet]
//        public IActionResult Edit(int? id)
//        {

//            if (id is null)
//            {
//                return NotFound();
//            }
//            else
//            {
//                //get user via repository
//                User? user = _usersRepository.GetById((int)id);
//                return View(user);
//            }
//        }

//        [HttpPost]
//        public IActionResult Edit(User user)
//        {
//            try
//            {
//                //add users via repository
//                _usersRepository.Update(user);

//                return RedirectToAction("Index");
//            }
//            catch (Exception ex)
//            {
//                return View(user);
//            }
//        }




//    }
//}
