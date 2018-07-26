using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Util.Model.Interface;

namespace Util.Model
{
    public sealed class Worker : ReactiveObject, IDisposable, IWorker, IEnableLogger
    {
        #region Events
        public event EventHandler Created;
        public event EventHandler Disposed;

        public event EventHandler<bool> Connected;
        public event EventHandler Disconnected;
        public event EventHandler RaiseDriverFailed;
        #endregion

        #region Fields
        private readonly System.Timers.Timer timer = new System.Timers.Timer();
        private readonly Stopwatch sw = new Stopwatch();
        private readonly CancellationTokenSource ct = new CancellationTokenSource();

        private bool isDisposed = false;
        private int heartbitFail = 0;

        private ReactiveCommand _ConnectAsync;
        private ReactiveCommand _DisconnectAsync;
        private ReactiveCommand _DoworkAsync;
        #endregion

        #region Properties
        public Category Category { get; }
        public IDriver Driver { get; }
        public string Project { get; }
        public string Name { get; }
        public string Alias => $"{Category}.{Project}.{Name}";
        [JsonIgnore]
        public IConfig Config { get; }

        [Reactive]
        public string Group { get; set; }
        [JsonIgnore]
        [Reactive]
        public bool IsBusy { get; private set; } = false;
        [JsonIgnore]
        [Reactive]
        public long Latency { get; private set; }
        [JsonIgnore]
        [Reactive]
        public int Hearbit { get; private set; }
        [JsonIgnore]
        [Reactive]
        public int Paritybit { get; private set; }
        [JsonIgnore]
        [Reactive]
        public int Count { get; private set; }
        [JsonIgnore]
        [Reactive]
        public ConnectionState State { get; private set; } = ConnectionState.Disconnected;
        #endregion

        #region Constructors
        public Worker(Category category, IDriver driver, string project, string name, string group, IConfig config = null)
        {
            Category = category;
            Driver = driver ?? throw new ArgumentNullException(nameof(driver));
            Project = project.ToUpperInvariant() ?? throw new ArgumentNullException(nameof(project));
            Name = name.ToUpperInvariant() ?? throw new ArgumentNullException(nameof(name));
            Group = group;
            Config = config ?? Locator.Current.GetService<IConfig>();

            timer.Elapsed += Timer_Elapsed;
            Created?.Invoke(this, null);
        }
        #endregion

        #region Public Methods
        public async void Dispose()
        {
            if (isDisposed) throw new ObjectDisposedException(nameof(Name));
            timer.Elapsed -= Timer_Elapsed;
            timer.Dispose();
            DisconnectAsync.CanExecute.Where(x => x).InvokeCommand(DisconnectAsync);
            isDisposed = true;
            Disposed?.Invoke(this, null);
        }

        [JsonIgnore]
        public ReactiveCommand ConnectAsync => _ConnectAsync = _ConnectAsync ?? ReactiveCommand.Create(async () =>
        {
            IsBusy = true;
            State = ConnectionState.Connecting;
            ct.CancelAfter(Config.Timeout);
            var ret = false;
            try
            {
                ret = await Driver.Connect(ct);
                if (ret)
                {
                    State = ConnectionState.Connected;
                    timer.Interval = Config.Cycle;
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
        }, this.WhenAnyValue(x => x.IsBusy, x => x.State, (b, s) => !b && s.HasFlag(ConnectionState.Disconnected)));

        [JsonIgnore]
        public ReactiveCommand DisconnectAsync => _DisconnectAsync = _DisconnectAsync ?? ReactiveCommand.CreateFromTask(async () =>
        {
            IsBusy = true;
            try
            {
                await Driver.Disconnect(ct);
                State = ConnectionState.Disconnected;
                if (timer.Enabled) timer.Stop();
                Disconnected?.Invoke(this, null);
            }
            catch (Exception)
            {
            }
            IsBusy = false;
        }, this.WhenAnyValue(x => x.IsBusy, x => x.State, (b, s) => !b && s.HasFlag(ConnectionState.Connected)));

        [JsonIgnore]
        public ReactiveCommand DoworkAsync => _DoworkAsync = _DoworkAsync ?? ReactiveCommand.CreateFromTask(async () =>
        {
            IsBusy = true;
            sw.Restart();
            var ct = new CancellationTokenSource(Config.Timeout);
            try
            {
                State = State == ConnectionState.Connected ? ConnectionState.Running : State;
                Count = await Driver.DoWork(ct);
            }
            catch (TimeoutException)
            {
                State = ConnectionState.WorkingTimeout;
            }
            catch (Exception)
            {
                State = ConnectionState.WorkingError;
            }
            Latency = sw.ElapsedMilliseconds;
            IsBusy = false;
        }, this.WhenAnyValue(x => x.IsBusy, x => x.State, (b, s) => !b && s.HasFlag(ConnectionState.Connected)));
        #endregion

        #region Private Methods
        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var ret = await Driver.IsConnected(ct);
            if (ret)
            {
                Hearbit += 1;
                heartbitFail = 0;
            }
            else
            {
                heartbitFail++;
                if (heartbitFail > Config.FailCount)
                {
                    RaiseDriverFailAsync();
                }
            }
        }

        private void RaiseDriverFailAsync()
        {
            RaiseDriverFailed?.Invoke(this, null);

            if (Config.CanAutoRecovery)
            {
                ConnectAsync.CanExecute.Where(x => x).InvokeCommand(ConnectAsync);
            }
            else
            {
                DisconnectAsync.CanExecute.Where(x => x).InvokeCommand(DisconnectAsync);
            }
        }
        #endregion
    }
}
