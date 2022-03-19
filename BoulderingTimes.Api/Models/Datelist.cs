namespace BoulderingTimes.Api.Models
{
    public class Datelist
    {
        public long start { get; set; }
        public long end { get; set; }

        public override string ToString()
        {
            return $"{start} - {end}";
        }
    }
}
