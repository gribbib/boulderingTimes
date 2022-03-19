namespace BoulderingTimes.Api.Models
{
    public class BoulderingTime
    {
        public string BoulderingPlace { get; set; }
        public DateTime Date { get; set; }
        public List<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
    }
}
