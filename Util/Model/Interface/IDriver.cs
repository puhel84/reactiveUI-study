using System.Threading;
using System.Threading.Tasks;

namespace Util.Model.Interface
{
    public interface IDriver
    {
        ISettings Setting { get; }

        void Dispose();
        Task<bool> Connect(CancellationTokenSource ct);
        Task Disconnect(CancellationTokenSource ct);
        Task<int> DoWork(CancellationTokenSource ct);
        Task<bool> IsConnected(CancellationTokenSource ct);
    }
}