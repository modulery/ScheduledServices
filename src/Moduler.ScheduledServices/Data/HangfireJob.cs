namespace Moduler.ScheduledServices.Data
{
    public class HangfireJob : BaseClass
    {
        public string Name { get; set; }
        public string EnvironmentId { get; set; }
        public string Interface { get; set; }
        public string Method { get; set; }
        public string Query { get; set; }
        public string Body { get; set; }
        public HangfireJobTypes JobType { get; set; }
        public DateTime? PlannedTime { get; set; }
        public HangfireCronTypes CronType { get; set; }
        public int? RecurringTimeValue { get; set; }
        public DateTime? ExecutedTime { get; set; }
    }
    public enum HangfireJobTypes
    {
        None = 0,
        Scheduled = 1,
        Recurring = 2,
        FireAndForget = 3,
        Continuation = 4,
        Batch = 5
    }
    public enum HangfireCronTypes
    {
        None = 0,
        Minutely = 1,
        Hourly = 2,
        Daily = 3,
        Weekly = 4,
        Monthly = 5,
        Yearly = 6
    }
}