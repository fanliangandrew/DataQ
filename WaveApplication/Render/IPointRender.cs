using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WaveApplication.Render {
    interface IPointRender {
        UIElement CreateImage();

        void RenderItem(UIElement image, Viewport baseVp, Viewport realVp);

        void RenderBasePoints(UIElement image, Viewport baseVp);

        RenderContext OpenRenderRealPointContext(UIElement image);
    }
}
