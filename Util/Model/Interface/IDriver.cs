using System.Threading.Tasks;

namespace Util.Model.Interface
{
    public interface IDriver
    {
        ISettings Setting { get; }

        void Dispose();
        Task<bool> Connect();
        Task Disconnect();
        Task<bool> DoWork();
        Task<bool> IsConnected();
    }
}