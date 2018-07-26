using Newtonsoft.Json;
using Splat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Util.Model.Interface;

namespace Util.Model.Driver
{
    public class SettingsForRedis : ISettings
    {
        public string Address { get; set; } = "localhost";
        public string Password { get; set; } = "";
        public int Port { get; set; } = 6379;
    }

    public class Redis : IDriver, IDisposable
    {
        #region Properties
        public ISettings Setting { get; }
        [JsonIgnore]
        public IConfig Config { get; }
        #endregion

        #region Constructors
        public Redis(ISettings setting = null, IConfig config = null)
        {
            Setting = setting ?? new SettingsForRedis();
            Config = config ?? Locator.Current.GetService<IConfig>();
        }
        #endregion

        #region Public Methods
        public void Dispose()
        {
        }

        public async Task<bool> Connect(CancellationTokenSource ct)
        {
            await Task.Delay(2000);
            return true;
        }

        public async Task Disconnect(CancellationTokenSource ct)
        {
            await Task.Delay(2000);
        }

        public async Task<int> DoWork(CancellationTokenSource ct)
        {
            await Task.Delay(2000);
            return 0;
        }

        public async Task<bool> IsConnected(CancellationTokenSource ct)
        {
            await Task.Delay(2000);
            return true;
        }
        #endregion
    }
}
