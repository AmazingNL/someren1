using someren_application.Models;

namespace someren_application.IRepositories
{
    public interface IRoomRepository 
    {
        List<Room> GetAll();
        Room? GetById(int roomId);
        void Add(Room room);
        void Update(Room room);
        void Delete(Room room);
        List<Room> Filter(int capacity);
    }
}
