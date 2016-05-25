using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using WaveApplication.Data;
using WaveApplication.Render;

namespace WaveApplication.Scheduling {
    class WaveTaskScheduler {
        private const int CollectStep = 5;

        public delegate void RenderingStartedDelegate();
        public delegate void RenderingStopedDeletegate();

        private delegate void GetRenderingItemDelegate(Viewport viewport);
        private delegate void RenderBaseDataDelegate(Viewport viewport);
        private delegate void RenderRealDataDelegate(RenderItem item, int count);

        protected CancellationTokenSource _tokenSource;
        protected Thread _thread;
        private volatile bool _rendering;
        protected AutoResetEvent _redraw;

        private IBaseDataSource _source;
        private IRealDataCollector _collector;
        private RealDataFile _file;
        private RenderTask _listRenderTask;
        private RenderTask _viewRenderTask;
        private StatisticTask _statisticTask;
        private ScrollTask _scrollTask;

        private RenderingStartedDelegate _startedCallback;
        private RenderingStopedDeletegate _stopedCallback;

        private StorageBlock _baseBlock;
        private StorageBlock _preBaseBlock;
        private StorageBlock _realBlock;

        public WaveTaskScheduler(
            IBaseDataSource source,
            IRealDataCollector collector,
            RealDataFile file,
            RenderTask listRenderTask,
            RenderTask viewRenderTask,
            StatisticTask statisticTask,
            ScrollTask scrollTask,
            RenderingStartedDelegate startedCallback,
            RenderingStopedDeletegate stopedCallback
            ) {
            _baseBlock = new StorageBlock();
            _preBaseBlock = new StorageBlock();
            _realBlock = new StorageBlock();

            _source = source;
            _collector = collector;
            _file = file;
            _listRenderTask = listRenderTask;
            _viewRenderTask = viewRenderTask;
            _statisticTask = statisticTask;
            _scrollTask = scrollTask;
            _startedCallback = startedCallback;
            _stopedCallback = stopedCallback;
        }

        public void StartSchedulingAndRendering() {
            _tokenSource = new CancellationTokenSource();
            _rendering = true;
            _redraw = new AutoResetEvent(false);
            _thread = new Thread(ScheduleProcess);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void StopRendering() {
            _rendering = false;
        }

        public void StopScheduling() {
            _tokenSource.Cancel();
        }

        private void ScheduleProcess() {
            TaskSpecificProcess();
            ResideProcess();
        }

        private void TaskSpecificProcess() {
            _collector.Start();
            _realBlock.Clear();
            _baseBlock.Clear();
            _preBaseBlock.Clear();
            _source.LoadBlock(_baseBlock);
            _source.LoadBlock(_preBaseBlock);
            _listRenderTask.Begin(_realBlock, _baseBlock, _preBaseBlock);
            if (_viewRenderTask != null) {
                _viewRenderTask.Begin(_realBlock, _baseBlock, _preBaseBlock);
            }
            if (_statisticTask != null) {
                _statisticTask.Start();
            }
            if (_startedCallback != null) {
                _startedCallback();
            }
            

            while (_rendering) {
                int index = _realBlock.Length;
                int length = Math.Min(_realBlock.Capacity, CollectStep);
                length = _collector.CollectPoints(_realBlock, length);
                if (length == 0) {
                    _rendering = false;
                    break;
                }
                bool needNewList, needNewView = false;
                needNewList = _listRenderTask.RenderRealPoints(length);
                if (_viewRenderTask != null) {
                    needNewView = _viewRenderTask.RenderRealPoints(length);
                }
                if (_statisticTask != null) {
                    _statisticTask.Statistics(_baseBlock, _realBlock, index, length);
                }
                if (_scrollTask != null && _scrollTask.IsEnable) {
                    _scrollTask.Scroll(_statisticTask.StatisticResult.Count);
                }
                Debug.Assert(!_realBlock.IsFull || needNewList);
                if (_realBlock.IsFull) {
                    _file.WriteBlock(_realBlock);
                    _realBlock.Clear();
                    var tb = _baseBlock;
                    _baseBlock = _preBaseBlock;
                    _preBaseBlock = tb;
                    _preBaseBlock.Clear();
                    _source.LoadBlock(_preBaseBlock);
                    _listRenderTask.RenderBasePointsBlock(_preBaseBlock);
                    if (_viewRenderTask != null) {
                        _viewRenderTask.RenderBasePointsBlock(_preBaseBlock);
                    }
                }
                if (needNewList) {
                    _listRenderTask.PrepareRenderItem(_realBlock);
                }
                if (_viewRenderTask != null && needNewView) {
                    _viewRenderTask.PrepareRenderItem(_realBlock);
                }
            }

            _collector.Stop();
            _listRenderTask.Finish();
            if (_viewRenderTask != null) {
                _viewRenderTask.Finish();
            }
            if (_statisticTask != null) {
                _statisticTask.Stop();
            }
            _file.WriteBlock(_realBlock);
            if (_stopedCallback != null) {
                _stopedCallback();
            }
        }

        private void ResideProcess() {
            var token = _tokenSource.Token;
            TimeSpan ts = TimeSpan.FromMinutes(1);
            while (true) {
                int index = WaitHandle.WaitAny(
                    new WaitHandle[] { token.WaitHandle, _redraw }, ts);
                if (index == 0) {
                    break;
                } else if (index == 1) {
                    RedrawRenderItems();
                } else {    //timeout
                    ReleaseRenderItemResources();
                }
            }
        }

        private void RedrawRenderItems() {
        }

        private void ReleaseRenderItemResources() {
        }
    }
}
