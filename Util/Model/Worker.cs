using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Util.Model.Interface;

namespace Util.Model
{
    public sealed class Worker : IDisposable
    {
        #region Events
        public event EventHandler Created;
        public event EventHandler Disposed;

        public event EventHandler<bool> Connected;
        public event EventHandler Disconnected;
        public event EventHandler Elapsed;
        public event EventHandler DoworkFinished;
        public event EventHandler RaiseDriverFailed;
        #endregion

        #region Fields
        private readonly System.Timers.Timer timer = new System.Timers.Timer();
        private readonly Stopwatch sw = new Stopwatch();
        private bool isDisposed = false;
        private int heartbitFailue = 0;
        #endregion

        #region Properties
        public Category Category { get; }
        public IDriver Driver { get; }
        public Properties Props { get; }
        public string Project { get; }
        public string Name { get; }
        public string Group { get; private set; }

        public string Alias => $"{Category}.{Project}.{Name}";
        public bool IsBusy { get; private set; } = false;
        public long LatencyMillisecond { get; private set; }
        public int Hearbit { get; private set; }
        public ConnectionState State { get; private set; } = ConnectionState.Disconnected;
        #endregion

        #region Constructors
        public Worker(Category category, IDriver driver, string project, string name, string group)
        {
            Category = category;
            Driver = driver ?? throw new ArgumentNullException(nameof(driver));
            Project = project.ToUpperInvariant() ?? throw new ArgumentNullException(nameof(project));
            Name = name.ToUpperInvariant() ?? throw new ArgumentNullException(nameof(name));
            Group = group;
            Props = new Properties();

            timer.Elapsed += Timer_Elapsed;
            Created?.Invoke(this, null);
        }
        #endregion

        #region Public Methods
        public async void Dispose()
        {
            if (isDisposed) throw new ObjectDisposedException(nameof(Name));
            timer.Elapsed -= Timer_Elapsed;
            timer?.Dispose();
            await DisconnectAsync();
            isDisposed = true;
            Disposed?.Invoke(this, null);
        }

        public async Task ConnectAsync()
        {
            IsBusy = true;
            State = ConnectionState.Connecting;
            var ct = new CancellationTokenSource(Props.TimeoutMillisecond);
            var ret = false;
            try
            {
                ret = await Task.Run(Driver.Connect, ct.Token);
                if (ret)
                {
                    State = ConnectionState.Connected;
                    timer.Interval = Props.HearbitIntervalMillisecond;
                    if (!timer.Enabled) timer.Start();
                }
                else
                {
                    State = ConnectionState.ConnectionError;
                }
            }
            catch (TimeoutException)
            {
                State = ConnectionState.ConnectionTimeout;
            }
            catch (Exception)
            {
                State = ConnectionState.ConnectionError;
            }
            Connected?.Invoke(this, ret);
            IsBusy = false;
        }

        public async Task DisconnectAsync()
        {
            IsBusy = true;
            try
            {
                await Task.Run(Driver.Disconnect);
                State = ConnectionState.Disconnected;
                if (timer.Enabled) timer.Stop();
                Disconnected?.Invoke(this, null);
            }
            catch (Exception)
            {
            }
            IsBusy = false;
        }

        public async Task DoworkAsync()
        {
            IsBusy = true;
            sw.Restart();
            var ct = new CancellationTokenSource(Props.TimeoutMillisecond);
            try
            {
                var ret = await Task.Run(Driver.DoWork, ct.Token);
                DoworkFinished?.Invoke(this, null);
            }
            catch (TimeoutException)
            {
                State = ConnectionState.WorkingTimeout;
            }
            catch (Exception)
            {
                State = ConnectionState.WorkingError;
            }
            LatencyMillisecond = sw.ElapsedMilliseconds;
            IsBusy = false;
        }
        #endregion

        #region Private Methods
        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var ret = await Task.Run(Driver.IsConnected);
            if (ret)
            {
                Hearbit += 1;
                heartbitFailue = 0;
            }
            else
            {
                heartbitFailue++;
                if (heartbitFailue > Props.HearbitWaitCount)
                {
                    await RaiseDriverFailAsync();
                }
            }
            Elapsed?.Invoke(this, null);
        }

        private async Task RaiseDriverFailAsync()
        {
            RaiseDriverFailed?.Invoke(this, null);

            if (Props.CanAutoRecovery)
            {
                await Task.Run(ConnectAsync);
            }
            else
            {
                await Task.Run(DisconnectAsync);
            }
        }
        #endregion
    }
}
