using someren_application.Models;

namespace someren_application.Repositories
{
    public interface IRoomRepository 
    {
        List<Room> GetAll();
        Room? GetById(int userId);
        void Add(Room room);
        void Update(Room room);
        void Delete(Room room);
    }
}
