using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using Util.Model;

namespace Main.ViewModels
{
    public class WorkerViewModel : ReactiveObject
    {
        #region Properties
        public Worker Model { get; }

        private string _latencyMillisecond;
        public string LatencyMillisecond { get => _latencyMillisecond; set => this.RaiseAndSetIfChanged(ref _latencyMillisecond, value); }

        private long _hearbit;
        public long Hearbit { get => _hearbit; set => this.RaiseAndSetIfChanged(ref _hearbit, value); }

        public ReactiveCommand<string, bool> CommandLogin { get; }
        #endregion

        #region Constructors
        public WorkerViewModel(Worker model)
        {
        }
        #endregion
    }
}
