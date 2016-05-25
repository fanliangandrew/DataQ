using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WaveApplication.Render {
    class LimitLine : DrawingVisual {
        public LimitLine(Pen pen, Point p1, Point p2) {
            using (var context = base.RenderOpen()) {
                context.DrawLine(pen, p1, p2);
            }
        }
    }
}
