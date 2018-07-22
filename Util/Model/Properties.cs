namespace Util.Model
{
    public class Properties
    {
        public int TimeoutMillisecond { get; set; }
        public long HearbitIntervalMillisecond { get; set; }
        public int HearbitWaitCount { get; set; }
        public bool CanAutoRecovery { get; set; }
    }
}