namespace BoulderingTimes.Api.Models
{
    public class TimeSlot
    {
        public object[] selector { get; set; }
        public long bookableFrom { get; set; }
        public string state { get; set; }
        public long bookableUntilDuration { get; set; }
        public int? minCourseParticipantCount { get; set; }
        public int? maxCourseParticipantCount { get; set; }
        public int? currentCourseParticipantCount { get; set; }
        public int? currentCourseFreePlacesCount { get; set; }
        public Datelist[] dateList { get; set; }

        public override string ToString()
        {
            return $"currentCourseFreePlacesCount: {currentCourseFreePlacesCount}\ndateList: {string.Join("\n", dateList.ToList())}";
        }
    }
}
