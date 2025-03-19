
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace someren_application.Models
{
    public class Room
    {
        public Room(int roomId, string building, string roomNumber, int capacity, string roomType)
        {
            RoomId = roomId;
            Building = building;
            RoomNumber = roomNumber;
            Capacity = capacity;
            RoomType = roomType;
        }

        public Room() 
        {
            RoomId = 0;
            Building = " ";
            RoomNumber = " ";
            Capacity = 0;
            RoomType = " ";
        }

        public int RoomId { get; set; } = 0;
        public string Building { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public int Capacity { get; set; } = 0;
        public  string RoomType { get; set; } = string.Empty;

        List<Students> students = new List<Students>();
    }
}
