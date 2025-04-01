
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
            Capacity = -1;
            RoomType = " ";
        }

        public int RoomId { get; set; } 
        public string Building { get; set; }
        public string RoomNumber { get; set; }
        public int Capacity { get; set; }
        public  string RoomType { get; set; }
    }
}
