using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Model;

namespace Main.ViewModels
{
    internal class WorkerViewModel : ReactiveObject
    {
        private readonly Worker worker;

        #region Properties
        ObservableAsPropertyHelper<string> project;
        public string Project => project.Value;
        #endregion

        #region Constructors
        public WorkerViewModel(Worker worker)
        {
            this.worker = worker;
        }
        #endregion
    }
}
