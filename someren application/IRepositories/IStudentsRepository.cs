using Microsoft.AspNetCore.Mvc;
using someren_application.Models;

namespace someren_application.Repositories
{
    public interface IStudentsRepository
    {
        List<Students> GetAllStudent();
        Students? GetStudentsById (int studentID);
        void Add(Students students);
        void Edit(Students students);
        void Delete(Students students);

        List<Students> GetStudentsInRooms(int roomId);

    }
}
