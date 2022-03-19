using BoulderingTimes.Api.Models.Enums;

namespace BoulderingTimes.Api.Models
{
    public class BoulderingPlace
    {
        public BoulderingPlaceTypes Type { get; set; }
        public int Id { get; set; }
        public string? Method { get; set; }
        public int? MaxCourseParticipantCount { get; set; }

    }
}
