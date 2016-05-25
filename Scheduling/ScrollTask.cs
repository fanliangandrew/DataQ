using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using WaveApplication.Render;

namespace WaveApplication.Scheduling {
    class ScrollTask {
        private delegate void ScrollDelegate(double hOffset);
        private readonly double width;
        private ScrollViewer _scrollViewer;

        public bool IsEnable { get; set; }

        public ScrollTask(ScrollViewer viewer) {
            var param = Parameter.GetInstance();
            width = param.PointWidth;
            _scrollViewer = viewer;
            IsEnable = true;
        }

        public void Scroll(int count) {
            double offset = count * width - _scrollViewer.ActualWidth / 2;
            _scrollViewer.Dispatcher.BeginInvoke(
                (ScrollDelegate)((hOffset) => {
                    _scrollViewer.ScrollToHorizontalOffset(hOffset);
                }),
                offset
            );

        }
    }
}
