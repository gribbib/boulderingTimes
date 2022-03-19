using BoulderingTimes.Api.Models;
using BoulderingTimes.Api.Models.Enums;
using BoulderingTimes.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BoulderingTimes.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BoulderingTimesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BoulderingTimesController> _logger;
        private readonly WebClimberTimesLoader _webClimberTimesLoader;
        private readonly DrPlanoTimesLoader _drPlanoTimesLoader;

        public BoulderingTimesController(ILogger<BoulderingTimesController> logger, IConfiguration configuration, WebClimberTimesLoader webClimberTimesLoader, DrPlanoTimesLoader drPlanoTimesLoader)
        {
            _configuration = configuration;
            _logger = logger;
            _webClimberTimesLoader = webClimberTimesLoader;
            _drPlanoTimesLoader = drPlanoTimesLoader;
        }

        [HttpGet(Name = "GetBoulderingTimes")]
        public async Task<IEnumerable<BoulderingTime>> GetAsync([FromQuery] DateTime requestDate, [FromQuery] List<string> boulderingPlaceKeys)
        {
            var boulderingTimes = new List<BoulderingTime>();
            Dictionary<string, BoulderingPlace> dict = _configuration.GetSection("boulderingPlaces").Get<Dictionary<string, BoulderingPlace>>();//.ToDictionary(x => x.Key, x => (BoulderingPlace)x.Value);
            if (boulderingPlaceKeys != null && boulderingPlaceKeys.Count > 0)
            {
                dict = dict.Where(d => boulderingPlaceKeys.Contains(d.Key)).ToDictionary(x => x.Key, x => x.Value);
            }

            var taskList = new List<Task<BoulderingTime>>();

            foreach (KeyValuePair<string, BoulderingPlace> kvp in dict)
            {
                taskList.Add(GetBoulderTime(kvp, requestDate));
            }

            foreach (Task<Task<BoulderingTime>> bucket in Interleaved(taskList))
            {
                Task<BoulderingTime> task = await bucket;
                BoulderingTime boulderingTime = await task;

                boulderingTimes.Add(boulderingTime);
            }

            return boulderingTimes;
        }

        private async Task<BoulderingTime> GetBoulderTime(KeyValuePair<string, BoulderingPlace> kvp, DateTime requestDate)
        {
            var boulderingPlace = kvp.Value;
            if (boulderingPlace == null)
            {
                return default;
            }
            List<TimeSlot> timeSlots = new List<TimeSlot>();
            switch (boulderingPlace.Type)
            {
                case BoulderingPlaceTypes.Webclimber:
                    timeSlots = _webClimberTimesLoader.GetTimes(requestDate, boulderingPlace);
                    break;
                case BoulderingPlaceTypes.DrPlano:
                    timeSlots = _drPlanoTimesLoader.GetTimes(requestDate, boulderingPlace);
                    break;
                default:
                    return default;
            }

            return new BoulderingTime
            {
                BoulderingPlace = kvp.Key,
                Date = requestDate,
                TimeSlots = timeSlots
            };
        }

        private static Task<Task<T>>[] Interleaved<T>(List<Task<T>> tasks)
        {
            TaskCompletionSource<Task<T>>[] buckets = new TaskCompletionSource<Task<T>>[tasks.Count];
            Task<Task<T>>[] results = new Task<Task<T>>[buckets.Length];
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new TaskCompletionSource<Task<T>>();
                results[i] = buckets[i].Task;
            }

            int nextTaskIndex = -1;
            Action<Task<T>> continuation = completed =>
            {
                TaskCompletionSource<Task<T>> bucket = buckets[Interlocked.Increment(ref nextTaskIndex)];
                bucket.TrySetResult(completed);
            };

            foreach (Task<T> inputTask in tasks)
            {
                inputTask.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            }

            return results;
        }
    }
}