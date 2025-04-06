using Microsoft.AspNetCore.Mvc;
using someren_application.Models;

namespace someren_application.IRepositories
{
    public interface IStudentsRepository
    {
        List<Students> GetAllStudent();
        Students GetStudentsById (int studentID);
        void Add(Students students);
        void Edit(Students students);
        void Delete(Students students);

        List<Students> Filter(string lastName);
        
    }
}
