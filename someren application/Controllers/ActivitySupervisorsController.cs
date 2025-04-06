using Microsoft.AspNetCore.Mvc;
using someren_application.DbRepository;
using someren_application.Models;

namespace someren_application.Controllers
{
    public class ActivitySupervisorsController : Controller
    {
        private readonly ActivitySupervisorRepository repository = new ActivitySupervisorRepository();

        // Display Activities and their Supervisors
        public ActionResult Index(int activityId)
        {
            var activity = repository.GetAllActivities().FirstOrDefault(a => a.ActivityId == activityId);
            var supervisors = repository.GetSupervisorsForActivity(activityId);
            var nonSupervisors = repository.GetAllLecturers().Except(supervisors).ToList();

            var viewModel = new ActivitySupervisorsViewModel
            {
                ActivityId = activity.ActivityId,
                ActivityName = activity.ActivityName,
                Supervisors = supervisors,
                NonSupervisors = nonSupervisors
            };

            return View(viewModel); // This will return Views/ActivitySupervisors/Index.cshtml
        }
        // Action to display the activity and manage supervisors
        public ActionResult ManageSupervisors(int activityId)
        {

            var activity = repository.getActivityById(activityId);

            var supervisors = repository.GetSupervisorsForActivity(activityId);
            var nonSupervisors = repository.GetNonSupervisors(supervisors);

            
            

            var viewModel = new ActivitySupervisorsViewModel
            {
                ActivityId = activity.ActivityId,
                ActivityName = activity.ActivityName,
                Supervisors = supervisors,
                SupervisorsLen = supervisors.Count(),
                NonSupervisors = nonSupervisors
            };

        

            return View(viewModel);


        }
            
        // Add a supervisor to an activity
        public ActionResult AddSupervisor(int activityId, int lecturerId)
        {
            repository.AddSupervisorToActivity(activityId, lecturerId);
            return RedirectToAction("ManageSupervisors", new { activityId = activityId });
        }

        // Remove a supervisor from an activity
        public ActionResult RemoveSupervisor(int activityId, int lecturerId)
        {
            repository.RemoveSupervisorFromActivity(activityId, lecturerId);
            return RedirectToAction("ManageSupervisors", new { activityId = activityId });
        }
    }
}
