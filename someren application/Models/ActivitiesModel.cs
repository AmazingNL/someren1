using System.Threading;

namespace someren_application.Models
{
    public class ActivitiesModel
    {
        public string ActivityName { get; set; }
        public int ActivityId { get; set; }
        public DateTime TimeSlot { get; set; }

        public ActivitiesModel(int activityId, string activityName, DateTime timeSet)
        {
            ActivityName = activityName;
            ActivityId = activityId;
            TimeSlot = timeSet;
        }

        public ActivitiesModel()
        {
            ActivityName = "";
            ActivityId = 0;
            TimeSlot = DateTime.Now;
        }

       

        
    
    }
}

