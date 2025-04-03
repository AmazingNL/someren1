using Microsoft.AspNetCore.Mvc;

namespace someren_application.Models
{
    public class Students
    {
        public List<Room> Rooms { get; set; }
        public int StudentId { get; set; }
        public string StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string StudentClass { get; set; }

        public Students()
        {
            StudentId = 0;
            StudentNumber = " ";
            FirstName = " ";
            LastName = " ";
            PhoneNumber = "";
            StudentClass = " ";
            Rooms = new List<Room>();
        }

        public Students(int studentId, string studentNumber, string firstName, string lastName, string phoneNumber, string studentClass, List<Room> rooms)
        {
            StudentId = studentId;
            StudentNumber = studentNumber;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            StudentClass = studentClass;
            Rooms = rooms;
    
        }
    }
}
