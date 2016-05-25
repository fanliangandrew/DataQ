using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WaveApplication.Render {
    class PillarImage : FrameworkElement {
        private static readonly Size ImageSize;

        static PillarImage() {
            var param = Parameter.GetInstance();
            ImageSize = new Size(
                 param.PointWidth * Viewport.ViewportSize,
                 param.CanvasHeight
            );
        }

        private PillarLayer[] _layers;

        public PillarLayer BaseDataLayer {
            get { return _layers[0]; }
        }

        public PillarLayer RealDataLayer {
            get { return _layers[1]; }
        }

        public PillarLayer LimitLineLayer {
            get { return _layers[2]; }
        }

        public PillarImage() {
            _layers = new PillarLayer[3];
            for (int i = 0; i < 3; i++) {
                _layers[i] = new PillarLayer();
                AddVisualChild(_layers[i]);
            }
        }

        protected override int VisualChildrenCount {
            get { return 3; }
        }

        protected override Visual GetVisualChild(int index) {
            return _layers[index];
        }

        protected override Size MeasureOverride(Size availableSize) {
            return ImageSize;
        }

        protected override Size ArrangeOverride(Size finalSize) {
            return ImageSize;
        }
    }
}
