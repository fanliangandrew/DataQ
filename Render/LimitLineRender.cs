using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WaveApplication.Render {
    class LimitLineRender : IPointRender {
        private class LimitLineRenderContext : RenderContext {
            public static double LastBlunt;
            public static double LastBreak;

            private readonly float _bluntScale;
            private readonly float _breakScale;

            private PillarLayer _layer;
            private RenderContext _context;
            private double _canvasHeight;
            private double _pillarWidth;
            private Point _lastBlunt;
            private Point _currentBlunt;
            private Point _lastBreak;
            private Point _currentBreak;

            public override int Index {
                get { return _context.Index; }
            }

            public LimitLineRenderContext(PillarLayer layer, RenderContext context, double canvasHeight, double pillarWidth) {
                var param = Parameter.GetInstance();
                _bluntScale = param.BluntLimitPercent + 1f;
                _breakScale = param.BreakLimitPercent + 1f;

                _layer = layer;
                _context = context;
                _canvasHeight = canvasHeight;
                _pillarWidth = pillarWidth;

                _lastBlunt = new Point(0, LastBlunt);
                _lastBreak = new Point(0, LastBreak);
                _currentBlunt = new Point(pillarWidth, 0);
                _currentBreak = new Point(pillarWidth, 0);
            }

            public override void RenderPoint(float basePoint, float realPoint) {
                _currentBlunt.Y = _canvasHeight - basePoint * _bluntScale;
                var line = new LimitLine(BluntPen, _lastBlunt, _currentBlunt);
                _layer.Add(line);
                _lastBlunt = _currentBlunt;
                _currentBlunt.X += _pillarWidth;

                _currentBreak.Y = _canvasHeight - basePoint * _breakScale;
                line = new LimitLine(BreakPen, _lastBreak, _currentBreak);
                _layer.Add(line);
                _lastBreak = _currentBreak;
                _currentBreak.X += _pillarWidth;

                _context.RenderPoint(basePoint, realPoint);
            }

            public override void Close() {
                LastBlunt = _lastBlunt.Y;
                LastBreak = _lastBreak.Y;
            }
        }

        private static readonly Pen BluntPen;
        private static readonly Pen BreakPen;

        static LimitLineRender() {
            var param = Parameter.GetInstance();

            var brush = new SolidColorBrush(param.BluntLimitColor);
            brush.Freeze();
            BluntPen = new Pen(brush, param.LimitLineThickness);
            BluntPen.Freeze();

            brush = new SolidColorBrush(param.BreakLimitColor);
            brush.Freeze();
            BreakPen = new Pen(brush, param.LimitLineThickness);
            BreakPen.Freeze();
        }

        private IPointRender _render;
        private double _canvasHeight;
        private double _pillarWidth;

        public LimitLineRender(IPointRender render, double canvasHeight, double pillarWidth) {
            _render = render;
            _canvasHeight = canvasHeight;
            _pillarWidth = pillarWidth;
            LimitLineRenderContext.LastBlunt = 0;
            LimitLineRenderContext.LastBreak = 0;
        }

        public UIElement CreateImage() {
            return _render.CreateImage();
        }

        public void RenderItem(UIElement image, Viewport baseVp, Viewport realVp) {
            throw new NotImplementedException();
        }

        public void RenderBasePoints(UIElement image, Viewport baseVp) {
            _render.RenderBasePoints(image, baseVp);
        }

        public RenderContext OpenRenderRealPointContext(UIElement image) {
            var himage = image as PillarImage;
            Debug.Assert(himage != null);
            var context = _render.OpenRenderRealPointContext(image);
            return new LimitLineRenderContext(himage.LimitLineLayer, context, _canvasHeight, _pillarWidth);
        }
    }
}
