using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;
using WaveApplication.Render;
using WaveApplication.Scheduling;

namespace WaveApplication.UI {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        private ListRenderItemPool _listPool;
        private ViewRenderItemPool _viewPool;
        private RenderTask _listRenderTask;
        private RenderTask _viewRenderTask;
        private StatisticTask _statisticTask;
        private ScrollTask _scrollTask;
        private WaveTaskScheduler _scheduler;

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (_scheduler != null) {
                _scheduler.StopRendering();
                _scheduler.StopScheduling();
            }
        }

        private void StartMenuItem_Click(object sender, RoutedEventArgs e) {
            if (_scheduler != null) {
                _scheduler.StopRendering();
                _scheduler.StopScheduling();
            }
            PathWindow wnd = new PathWindow(true);
            wnd.ShowDialog();
            WaveListBox.OwnedScrollViewer.HorizontalScrollBarVisibility =
                ScrollBarVisibility.Hidden;
            var factory = new RandomWidgetFactory();
            CreateTaskAndScheduler(factory, 1, 1);
            WaveListBox.ItemsSource = _listPool.RenderItems;
            ViewViewBox.DataContext = _viewPool;
            _scheduler.StartSchedulingAndRendering();
        }

        private void StopMenuItem_Click(object sender, RoutedEventArgs e) {
            _scheduler.StopRendering();
            ViewViewBox.DataContext = null;
            WaveListBox.OwnedScrollViewer.HorizontalScrollBarVisibility =
                ScrollBarVisibility.Visible;
        }

        private void ScrollMenuItem_Click(object sender, RoutedEventArgs e) {
            Debug.Assert(_scheduler != null);
            var check = (bool)ScrollMenuItem.IsChecked;
            _scrollTask.IsEnable = check;
            WaveListBox.OwnedScrollViewer.HorizontalScrollBarVisibility =
                check ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Visible;
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e) {
            if (_scheduler != null) {
                _scheduler.StopRendering();
                _scheduler.StopScheduling();
            }
            PathWindow pwnd = new PathWindow(false);
            pwnd.ShowDialog();
            DrawOptionWindow dwnd = new DrawOptionWindow();
            dwnd.ShowDialog();
            WaveListBox.OwnedScrollViewer.HorizontalScrollBarVisibility =
                ScrollBarVisibility.Visible;
            var factory = new FileWidgetFactory();
            CreateTaskAndScheduler(factory, dwnd.Channel, dwnd.N);
            WaveListBox.ItemsSource = _listPool.RenderItems;
            ViewViewBox.DataContext = null;
            _scheduler.StartSchedulingAndRendering();
        }

        private void ConfigurationMenuItem_Click(object sender, RoutedEventArgs e) {
            ConfigurationWindow wnd = new ConfigurationWindow();
            wnd.ShowDialog();
        }

        private void OnRenderingStarted() {
            this.Dispatcher.BeginInvoke(
                (Action)(() => {
                    CountTextBlock.Text = null;
                    ExceedAmountTextBlock.Text = null;
                    UnderAmountTextBlock.Text = null;
                    ExceedPercentTextBlock.Text = null;
                    UnderPercentTextBlock.Text = null;
                    LogListBox.Items.Clear();
                })
            );
        }

        private void OnExceedLimit(ExceedLimitItem item) {
            this.Dispatcher.BeginInvoke(
                (Action)(() => {
                    LogListBox.Items.Add(item);
                    LogListBox.ScrollIntoView(item);
                })
            );
        }

        private void OnRenderingStoped() {
            if (_statisticTask != null) {
                this.Dispatcher.BeginInvoke(
                    (Action<StatisticResult>)((result) => {
                        ShowStatisticResult(result);
                    }),
                    _statisticTask.StatisticResult
                );
            }
        }

        private void ShowStatisticResult(StatisticResult result) {
            CountTextBlock.Text = result.Count.ToString();
            ExceedAmountTextBlock.Text = result.ExceedAmount.ToString();
            UnderAmountTextBlock.Text = result.UnderAmount.ToString();
            ExceedPercentTextBlock.Text = String.Format("{0:f2}%", result.ExceedPercent*100);
            UnderPercentTextBlock.Text = String.Format("{0:f2}%", result.UnderPercent * 100);
        }

        private void CreateTaskAndScheduler(WidgetFactory factory, int channel, int n) {
            var render = factory.CreatePointRender();
            _listPool = new ListRenderItemPool();
            _listRenderTask = factory.CreateListRenderTask(this.Dispatcher, render, _listPool);
            _viewPool = new ViewRenderItemPool(render, this.Dispatcher);
            _viewRenderTask = factory.CreateViewRenderTask(this.Dispatcher, render, _viewPool);
            _statisticTask = factory.CreateStatisticTask(OnExceedLimit);
            _scrollTask = factory.CreateScrollTask(WaveListBox.OwnedScrollViewer);
            _scheduler = new WaveTaskScheduler(
                factory.CreateBaseDataSource(channel, n),
                factory.CreateRealDataCollector(channel, n),
                factory.CreateRealDataFile(),
                _listRenderTask,
                _viewRenderTask,
                _statisticTask,
                _scrollTask,
                OnRenderingStarted,
                OnRenderingStoped
           );
        }
    }
}
