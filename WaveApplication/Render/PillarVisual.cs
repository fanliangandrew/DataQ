using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WaveApplication.Render {
    class PillarVisual : DrawingVisual {
        public PillarVisual(Brush brush, Rect rect) {
            using (var context = base.RenderOpen()) {
                context.DrawRectangle(brush, null, rect);
            }
        }
    }
}
