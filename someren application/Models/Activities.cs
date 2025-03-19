using System.Threading;

namespace someren_application.Models
{
    public class Activities
    {
        private DateTime dateTime;

        public Activities()
        {
            ActivityName = "";
            ActivityId = 0;
            TimeSlot = dateTime;
        }

        public Activities(int activityId, string activityName, DateTime timeSet)
        {
            ActivityName = activityName;
            ActivityId = activityId;
            TimeSlot = timeSet;
        }

        public string ActivityName { get; set; }
        public int ActivityId { get; set; }
        public DateTime TimeSlot { get; set; }
    
    }
}

