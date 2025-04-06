using Microsoft.AspNetCore.Mvc;
using someren_application.Models;
using someren_application.Repositories;

namespace someren_application.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly IActivityRepository _activityRepository;

        public ActivitiesController(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

        public IActionResult Index()
        {
            List<Activities> activities = _activityRepository.GetAll();
            return View(activities);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Activities activities)
        {
            try
            {
                _activityRepository.Add(activities);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View(activities);
            }
        }

        [HttpGet]
        public IActionResult Edit(int activityId)
        {
            Activities? activities = _activityRepository.GetById(activityId);
            return View(activities);
        }


        [HttpPost]
        public IActionResult Edit(Activities activities)
        {
            try
            {
                _activityRepository.Update(activities);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View(activities);
            }
        }

        //Get user
        [HttpGet]
        public IActionResult Delete(int? activityId)
        {
            if (activityId is null)
            {
                return NotFound();
            }
            else
            {
                //get user via repository
                Activities? activities = _activityRepository.GetById((int)activityId);
                return View(activities);
            }
        }
        [HttpPost]
        public IActionResult Delete(Activities activities)
        {
            try
            {
                //delete user via repository
                _activityRepository.Delete(activities);

                //go back to user list(via Index)
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                //something went wrong, go back to view with user
                return View(activities);
            }
        }
    }

}
