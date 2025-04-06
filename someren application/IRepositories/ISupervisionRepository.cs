using someren_application.Models;
using System.Collections.Generic;
using System.Diagnostics;


namespace someren_application.Repositories
{
    public interface ISupervisionRepository
    {
        List<Lecturer> GetAllLecturers();
        List<Activities> GetAllActivities();
        List<Lecturer> GetSupervisorsForActivity(int activityId);
        List<Lecturer> GetNonSupervisorsForActivity(int activityId);
        void AddSupervisorToActivity(int activityId, int lecturerId);
        void RemoveSupervisorFromActivity(int activityId, int lecturerId);
    }
}
