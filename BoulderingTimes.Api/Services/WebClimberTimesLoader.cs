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

            // Console.Write(content);
            if (content == null)
            {
                return list;
            }

            var array = content.Split("<td>");
            if (array[0] != "\n<table class=\"table table-striped\">\n    <tbody>\n\n\t<tr>")
            {
                return list;
            }
            
            for (int i = 0; i < array.Length - 1; i = i + 3)
            {
                if (array[i + 1].Contains("keine Plätze mehr", StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }
                var timeSplit = array[i + 1].Split(" - ");
                var start = TimeHelper.ConvertDatetimeToUnixTimeStamp((requestDate + TimeSpan.ParseExact(timeSplit[0], @"h\:m", CultureInfo.InvariantCulture)).ToUniversalTime());
                var end = TimeHelper.ConvertDatetimeToUnixTimeStamp((requestDate + TimeSpan.ParseExact(timeSplit[1].Replace(" Uhr</td>", ""), @"h\:m", CultureInfo.InvariantCulture)).ToUniversalTime());

                list.Add(new TimeSlot
                {
                    currentCourseFreePlacesCount = Convert.ToInt16(array[i + 2]
                        .Replace("</td>", "")
                        .Replace("mehr als ", "")
                        .Replace(" Plätze verfügbar", "")
                        .Replace(" freier Platz", "")
                        .Replace(" freie Plätze", "")),
                    maxCourseParticipantCount = boulderingPlace.MaxCourseParticipantCount,
                    dateList = new Datelist[] { new Datelist { start = start, end = end } },
                    state = "BOOKABLE"
                });
            }
            return list;
        }
    }
}
