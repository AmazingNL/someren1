using Microsoft.AspNetCore.Mvc;
using someren_application.Repositories;
using someren_application.Models;
namespace someren_application.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomRepository _roomRepository;

        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public IActionResult Index()
        {
            List<Room> rooms = _roomRepository.GetAll();
            return View(rooms);
        }

        [HttpPost]
        public IActionResult Filter(int capacity)
        {

            try
            {
                List<Room> rooms = _roomRepository.Filter(capacity);
                return View(rooms);
            }
            catch (Exception)
            {

                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public IActionResult Create()
        {
             return View();
        }
        [HttpPost]
        public IActionResult Create(Room room)
        {
            try
            {
                _roomRepository.Add(room);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View(room);
            }
        }

        [HttpGet]
        public IActionResult Edit(int roomId)
        {
            Room? room = _roomRepository.GetById(roomId);
            return View(room);
        }

        [HttpPost]
        public IActionResult Edit(Room room)
        {
            try
            {
                _roomRepository.Update(room);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View(room);
            }
        }

        //Get user
        [HttpGet]
        public IActionResult Delete(int? roomId)
        {
            if (roomId is null)
            {
                return NotFound();
            }
            else
            {
                //get user via repository
                Room? room = _roomRepository.GetById((int)roomId);
                return View(room);
            }
        }
        [HttpPost]
        public IActionResult Delete(Room room)
        {
            try
            {
                //delete user via repository
                _roomRepository.Delete(room);

                //go back to user list(via Index)
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //something went wrong, go back to view with user
                return View(room);
            }
        }
    }
}
