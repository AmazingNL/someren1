using someren_application.Models;

namespace someren_application.Repositories
{
    public interface ILecturerRepository
    {
        List<Lecturer> GetAll();
        Lecturer? GetById(int lecturerId);
        void Add(Lecturer lecturer);
        void Update(Lecturer lecturer);
        void Delete(Lecturer lecturer);

    }
}
