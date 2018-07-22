using System;
using System.Collections.Generic;
using System.Text;
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
        #endregion

        #region Constructors
        public Redis()
        {
            Setting = new SettingsForRedis();
        }
        #endregion

        #region Public Methods
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Connect()
        {
            throw new NotImplementedException();
        }

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DoWork()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsConnected()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
