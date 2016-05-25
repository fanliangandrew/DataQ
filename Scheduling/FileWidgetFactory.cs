using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using WaveApplication.Data;
using WaveApplication.Render;

namespace WaveApplication.Scheduling {
    class FileWidgetFactory : WidgetFactory {
        public override IBaseDataSource CreateBaseDataSource(int channel, int n) {
            IBaseDataSource source = new ExperimentBaseDataSource(channel == 0);
            if (n != 1) {
                source = new AverageBaseDataSource(source, n);
            }
            return source;
        }

        public override IRealDataCollector CreateRealDataCollector(int channel, int n) {
            IRealDataCollector collector = new ExperimentRealDataCollector(channel == 0);
            if (n != 1) {
                collector = new AverageRealDataCollect(collector, n);
            }
            return collector;
        }

        public override RealDataFile CreateRealDataFile() {
            return new RealDataFile();
        }

        public override IPointRender CreatePointRender() {
            var param = Parameter.GetInstance();
            var ppr = new PillarPointRender(param.CanvasHeight, param.PointWidth);
            return new ScalePointRender(ppr, 50, 1);
        }

        public override RenderTask CreateListRenderTask(Dispatcher dispatcher, IPointRender render, RenderItemPool pool) {
            return new RenderTask(dispatcher, render, pool);
        }

        public override RenderTask CreateViewRenderTask(Dispatcher dispatcher, IPointRender render, RenderItemPool pool) {
            return null;
        }

        public override StatisticTask CreateStatisticTask(ExceedLimitCallbackDelegate callback) {
            return null;
        }

        public override ScrollTask CreateScrollTask(ScrollViewer viewer) {
            return null;
        }
    }
}
