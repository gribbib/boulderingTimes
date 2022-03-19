namespace BoulderingTimes.Api.Helpers
{
    public static class TimeHelper
    { 
        public static long ConvertDatetimeToUnixTimeStamp(DateTime date)
        {
            var dateTimeOffset = new DateTimeOffset(date);
            long unixDateTime = dateTimeOffset.ToUnixTimeMilliseconds();
            return unixDateTime;
        }
    }
}
