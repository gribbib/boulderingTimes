using BoulderingTimes.Api.Helpers;
using BoulderingTimes.Api.Models;
using System.Globalization;

namespace BoulderingTimes.Api.Services
{
    public class WebClimberTimesLoader : BoulderingTimesLoader
    {
        public int Location { get; set; }
        public WebClimberTimesLoader(IConfiguration configuration)
        {
            base.BaseUrl = configuration.GetSection("baseUrls:webclimber").Value;
            HttpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        }
        public async Task<List<TimeSlot>> GetTimesAsync(DateTime requestDate, BoulderingPlace boulderingPlace)
        {
            try
            {
                return await Task.Run(() => GetTimes(requestDate, boulderingPlace)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public List<TimeSlot> GetTimes(DateTime requestDate, BoulderingPlace boulderingPlace)
        {
            var list = new List<TimeSlot>();

            if (boulderingPlace.Id <= 0 || string.IsNullOrEmpty(boulderingPlace.Method) || requestDate == DateTime.MinValue)
            {
                return list;
            }

            HttpClient.DefaultRequestHeaders.Remove("Host");
            HttpClient.DefaultRequestHeaders.Add("Host", $"{boulderingPlace.Id}.webclimber.de");
            var content = base.RequestContent(boulderingPlace.Id, boulderingPlace.Method, requestDate.ToString("yyyy-MM-dd"));

            if (content == null)
            {
                return list;
            }

            var array = content.Split("<td>");
            for (int i = 0; i < array.Length - 1; i = i + 3)
            {
                if (array[i + 1].Contains("Leider sind an diesem Tag keine Plätze mehr verfügbar!"))
                {
                    break;
                }
                var timeSplit = array[i + 1].Split(" - ");
                var start = TimeHelper.ConvertDatetimeToUnixTimeStamp(requestDate + TimeSpan.ParseExact(timeSplit[0], @"h\:m", CultureInfo.InvariantCulture));
                var end = TimeHelper.ConvertDatetimeToUnixTimeStamp(requestDate + TimeSpan.ParseExact(timeSplit[1].Replace(" Uhr</td>", ""), @"h\:m", CultureInfo.InvariantCulture));

                list.Add(new TimeSlot
                {
                    currentCourseFreePlacesCount = Convert.ToInt16(array[i + 2]
                        .Replace("</td>", "")
                        .Replace("mehr als ", "")
                        .Replace(" Plätze verfügbar", "")
                        .Replace(" freier Platz", "")
                        .Replace(" freie Plätze", "")),
                    maxCourseParticipantCount = boulderingPlace.MaxCourseParticipantCount,
                    dateList = new Datelist[] { new Datelist { start = start, end = end } }
                });
            }
            return list;
        }
    }
}
