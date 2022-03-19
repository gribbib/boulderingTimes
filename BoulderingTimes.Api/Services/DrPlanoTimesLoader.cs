using BoulderingTimes.Api.Helpers;
using BoulderingTimes.Api.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace BoulderingTimes.Api.Services
{
    public class DrPlanoTimesLoader : BoulderingTimesLoader
    {
        public int Location { get; set; }
        public DrPlanoTimesLoader(IConfiguration configuration)
        {
            base.BaseUrl = configuration.GetSection("baseUrls:drPlano").Value;
        }

        public List<TimeSlot> GetTimes(DateTime requestDate, BoulderingPlace boulderingPlace)
        {
            var list = new List<TimeSlot>();

            if (boulderingPlace.Id <= 0 || requestDate == DateTime.MinValue)
            {
                return list;
            }

            var requestStart = TimeHelper.ConvertDatetimeToUnixTimeStamp(requestDate);
            var requestEnd = TimeHelper.ConvertDatetimeToUnixTimeStamp(requestDate.AddDays(1));

            var content = base.RequestContent(boulderingPlace.Id, requestStart, requestEnd);

            if (content == null)
            {
                return list;
            }

            list = JsonConvert.DeserializeObject<List<TimeSlot>>(content);

            return list;
        }
    }
}
