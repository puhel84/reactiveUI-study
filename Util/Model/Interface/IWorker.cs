using System;
using ReactiveUI;
using Util.Model.Interface;

namespace Util.Model
{
    public interface IWorker
    {
        event EventHandler<bool> Connected;
        event EventHandler Created;
        event EventHandler Disconnected;
        event EventHandler Disposed;
        event EventHandler RaiseDriverFailed;

        string Alias { get; }
        Category Category { get; }
        IConfig Config { get; }
        int Count { get; }
        IDriver Driver { get; }
        string Group { get; set; }
        int Hearbit { get; }
        bool IsBusy { get; }
        long Latency { get; }
        string Name { get; }
        int Paritybit { get; }
        string Project { get; }
        ConnectionState State { get; }

        void Dispose();
        ReactiveCommand ConnectAsync { get; }
        ReactiveCommand DisconnectAsync { get; }
        ReactiveCommand DoworkAsync { get; }
    }
}