namespace someren_application.Models
{
    public class Lecturer
    {

        //public int LecturerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public int Age { get; set; }


        public Lecturer(string firstName, string lastName, string telephone, int age)
        {
            //LecturerID = lecturerID;
            FirstName = firstName;
            LastName = lastName;
            Telephone = telephone;
            Age = age;
        }
    }
}
