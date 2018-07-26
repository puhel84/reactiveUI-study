using Util.Model.Interface;

namespace Util.Model
{
    public class Config : IConfig
    {
        public int Timeout { get; set; }
        public long Cycle { get; set; }
        public int FailCount { get; set; }
        public bool CanAutoRecovery { get; set; }
        public int Count { get; set; }
    }
}