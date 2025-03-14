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

        [HttpPost]
        public IActionResult Delete(Room room)
        {
            try
            {
                _roomRepository.Delete(room);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View(room);
            }
        }
    }
}
