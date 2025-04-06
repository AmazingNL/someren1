namespace someren_application.Models
{
    public class ActivitySupervisorsViewModel
    {

        public int SupervisorsLen { get; set; }
        
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public List<Lecturer> Supervisors { get; set; }
        public List<Lecturer> NonSupervisors { get; set; }


    }
}
