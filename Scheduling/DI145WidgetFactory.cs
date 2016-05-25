using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using WaveApplication.Data;
using WaveApplication.Render;

namespace WaveApplication.Scheduling
{
    class DI145WidgetFactory : WidgetFactory
    {
        public override IBaseDataSource CreateBaseDataSource(int channel, int n)
        {
            return new RandomBaseDataSource();
        }

        public override IRealDataCollector CreateRealDataCollector(int channel, int n)
        {
            return new DI145RealDataCollector();
        }

        public override RealDataFile CreateRealDataFile()
        {
            return new RealDataFile();
        }

        public override IPointRender CreatePointRender()
        {
            var param = Parameter.GetInstance();
            var render = new PillarPointRender(param.CanvasHeight, param.PointWidth);
            return new LimitLineRender(render, param.CanvasHeight, param.PointWidth);
        }

        public override RenderTask CreateListRenderTask(Dispatcher dispatcher, IPointRender render, RenderItemPool pool)
        {
            return new RenderTask(dispatcher, render, pool);
        }

        public override RenderTask CreateViewRenderTask(Dispatcher dispatcher, IPointRender render, RenderItemPool pool)
        {
            return new RenderTask(dispatcher, render, pool);
        }

        public override StatisticTask CreateStatisticTask(ExceedLimitCallbackDelegate callback)
        {
            return new StatisticTask(callback);
        }

        public override ScrollTask CreateScrollTask(ScrollViewer viewer)
        {
            return new ScrollTask(viewer);
        }
    }
}
