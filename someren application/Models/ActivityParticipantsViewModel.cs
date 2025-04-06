using System.Diagnostics;

namespace someren_application.Models
{
    public class ActivityParticipantsViewModel
    {
        public Activity Activity { get; set; }
        public List<Students> Participants { get; set; }
        public List<Students> NonParticipants { get; set; }
        public string? ConfirmationMessage { get; set; }
    }
}
