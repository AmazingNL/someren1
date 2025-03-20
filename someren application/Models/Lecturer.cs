using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace someren_application.Models
{
    public class Lecturer
    {

        public int LecturerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public  Room? Room { get; set; }
        public int RoomID { get; set; }
        public Lecturer()
        {

            LecturerID = 0;
            FirstName = "";
            LastName = "";
            PhoneNumber = "";
            Age = 0;
            RoomID = 0;
        }

        public Lecturer(int lecturerID, string firstName, string lastName, string phoneNumber, int age, int roomId)
        {
            LecturerID = lecturerID;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Age = age;
            RoomID = roomId;
           
        }
    }
}
