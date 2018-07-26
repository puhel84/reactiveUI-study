namespace Util.Model.Interface
{
    public interface IConfig
    {
        int Timeout { get; set; }
        long Cycle { get; set; }
        int FailCount { get; set; }
        bool CanAutoRecovery { get; set; }
    }
}