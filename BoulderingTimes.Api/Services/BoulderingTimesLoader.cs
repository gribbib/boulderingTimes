using BoulderingTimes.Api.Models;

namespace BoulderingTimes.Api.Services
{
    public class BoulderingTimesLoader
    {
        public IConfiguration _configuration { get; internal set; }

        public BoulderingTimesLoader()
        {
            HttpClient = new HttpClient();
        }

        public string BaseUrl { get; internal set; }
        public DateTime RequestDate { get; internal set; }
        public HttpClient HttpClient { get; internal set; }


        public string? RequestContent(params object[] queryParams)
        {
            string formatBaseUrl = string.Format(BaseUrl, queryParams);
            HttpResponseMessage? response = Task.Run(async () => await HttpClient.GetAsync(formatBaseUrl)).Result;

            string? content = response.Content.ReadAsStringAsync().Result;
            return content;
        }
    }
}
