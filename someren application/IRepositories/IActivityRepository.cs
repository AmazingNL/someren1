using someren_application.Models;

namespace someren_application.Repositories
{
    
    public interface IActivityRepository
    {
        List<ActivitiesModel> GetAll();
        ActivitiesModel? GetById(int studentId);
        void Add(ActivitiesModel activity);
        void Update(ActivitiesModel activity);
        void Delete(ActivitiesModel activity);
        List<Students> GetParticipants(int activityId);
        //List<Students> GetNonParticipants(int activityId);
        void AddParticipant(int activityId, int studentId);
        void RemoveParticipant(int activityId, int studentId);
    }
}
