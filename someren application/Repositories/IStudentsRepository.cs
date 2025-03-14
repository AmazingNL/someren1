using Microsoft.AspNetCore.Mvc;
using someren_application.Models;

namespace someren_application.Repositories
{
    public interface IStudentsRepository
    {
        List<Students> GetAll();
        Students? GetById(int studentID);
        void Add(Students students);
        void Update(Students students);
        void Delete(Students students);

    }
}
