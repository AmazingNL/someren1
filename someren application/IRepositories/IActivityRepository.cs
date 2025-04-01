using someren_application.Models;

namespace someren_application.Repositories
{
    public interface IActivityRepository
    {
        List<Activities> GetAll();
        Activities? GetById(int userId);
        void Add(Activities activity);
        void Update(Activities activity);
        void Delete(Activities activity);
    }
}
