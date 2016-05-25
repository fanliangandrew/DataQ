using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveApplication.Render {
    abstract class RenderItemPool {
        public abstract RenderItem CreateRenderItem(IPointRender render);
        public abstract RenderItem GetNextRenderingRealPointItem();
        public abstract void Freeze();
    }
}
