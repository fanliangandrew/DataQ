using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using WaveApplication.Data;
using WaveApplication.Render;

namespace WaveApplication.Scheduling {
    abstract class WidgetFactory {
        public abstract IBaseDataSource CreateBaseDataSource(int channel, int n);
        public abstract IRealDataCollector CreateRealDataCollector(int channel, int n);
        public abstract RealDataFile CreateRealDataFile();
        public abstract IPointRender CreatePointRender();
        public abstract RenderTask CreateListRenderTask(Dispatcher dispatcher, IPointRender render, RenderItemPool pool);
        public abstract RenderTask CreateViewRenderTask(Dispatcher dispatcher, IPointRender render, RenderItemPool pool);
        public abstract StatisticTask CreateStatisticTask(ExceedLimitCallbackDelegate callback);
        public abstract ScrollTask CreateScrollTask(ScrollViewer viewer);
    }
}
