using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Moduler.ScheduledServices.Controllers
{
    public class HangfireController : Controller
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        public HangfireController(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        [HttpGet]
        [Route("IFireAndForgetJob")]
        public string FireAndForgetJob()
        {
            //Fire-and-Forget Jobs
            //Fire-and-forget jobs are executed only once and almost immediately after creation.
            var jobId = _backgroundJobClient.Enqueue(() => Console.WriteLine("Welcome user in Fire and Forget Job Demo!"));

            return $"Job ID: {jobId}. Welcome user in Fire and Forget Job Demo!";
        }

        [HttpGet]
        [Route("IDelayedJob")]
        public string DelayedJob()
        {
            //Delayed Jobs
            //Delayed jobs are executed only once too, but not immediately, after a certain time interval.
            var jobId = _backgroundJobClient.Schedule(() => Console.WriteLine("Welcome user in Delayed Job Demo!"), TimeSpan.FromSeconds(5));

            return $"Job ID: {jobId}. Welcome user in Delayed Job Demo!";
        }

        [HttpGet]
        [Route("IContinuousJob")]
        public string ContinuousJob()
        {
            //Fire-and-Forget Jobs
            //Fire-and-forget jobs are executed only once and almost immediately after creation.
            var parentjobId = _backgroundJobClient.Enqueue(() => Console.WriteLine("Welcome user in Fire and Forget Job Demo!"));

            //Continuations
            //Continuations are executed when its parent job has been finished.
            var jobId = BackgroundJob.ContinueJobWith(parentjobId, () => Console.WriteLine("Welcome Sachchi in Continuos Job Demo!"));

            return $"Job ID: {parentjobId}. Welcome user in Continuos Job Demo!";
        }

        [HttpGet]
        [Route("IRecurringJob")]
        public string RecurringJobs()
        {
            //Recurring Jobs
            //Recurring jobs fire many times on the specified CRON schedule.
            var jobId = Guid.NewGuid().ToString();
            _recurringJobManager.AddOrUpdate($"Job ID: {jobId}", () => Console.WriteLine($"Welcome user in Recurring Job Demo! Job ID: {jobId} CreatedOn: {DateTime.Now}"), Cron.Minutely);

            return $"Job ID: {jobId}. Welcome user in Recurring Job Demo!";
        }
    }
}
