using Main.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Util;
using Util.Model;
using Util.Model.Driver;
using Util.Model.Interface;

namespace Main
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public Worker ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Locator.CurrentMutable.RegisterConstant<IConfig>(new Config());
            ViewModel = new Worker(Category.Broker, new Redis(), "TEST-PROJECT", "TEST-NAME", "TEST-GROUP");
            DataContext = ViewModel;
        }
    }
}
