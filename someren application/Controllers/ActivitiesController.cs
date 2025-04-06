using Microsoft.AspNetCore.Mvc;
using someren_application.DbRepository;
using someren_application.IRepositories;
using someren_application.Models;
using someren_application.Repositories;
using System.Diagnostics;



namespace someren_application.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IStudentsRepository _studentsRepository;
        

        public ActivitiesController(IActivityRepository activityRepository, IStudentsRepository studentsRepository)
        {
            _activityRepository = activityRepository;
            _studentsRepository = studentsRepository;
        }

        public IActionResult Index()
        {
            List<ActivitiesModel> activities = _activityRepository.GetAll();
            return View(activities);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ActivitiesModel activities)
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
            ActivitiesModel? activities = _activityRepository.GetById(activityId);
            return View(activities);
        }

        [HttpPost]
        public IActionResult Edit(ActivitiesModel activities)
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
                ActivitiesModel? activities = _activityRepository.GetById((int)activityId);
                return View(activities);
            }
        }
        [HttpPost]
        public IActionResult Delete(ActivitiesModel activities)
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



        // Action to display the selected activity with participants and non-participants
        [HttpGet]
        public IActionResult ViewParticipants(int activityId, string? confirmation = null)
        {
            var activity = _activityRepository.GetById(activityId);
            var participants = _activityRepository.GetParticipants(activityId);
            var nonParticipants = _studentsRepository.GetAllStudent().Where(s => !participants.Any(p => p.StudentId == s.StudentId)).ToList();
            ViewBag.Activity = activity;
            ViewBag.Participants = participants;
            ViewBag.NonParticipants = nonParticipants;
            ViewBag.Confirmation = confirmation;
            return View();
        }


        [HttpPost]
        public IActionResult AddParticipant(int activityId, int studentId)
        {
            _activityRepository.AddParticipant(activityId, studentId);
            return RedirectToAction("ViewParticipants", new { activityId = activityId, confirmation = "Participant added." });
        }

        [HttpPost]
        public IActionResult RemoveParticipant(int activityId, int studentId)
        {
            _activityRepository.RemoveParticipant(activityId, studentId);
            return RedirectToAction("ViewParticipants", new { activityId = activityId, confirmation = "Participant removed." });
        }
    }
}
    
