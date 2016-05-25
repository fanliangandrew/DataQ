using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WaveApplication;
using WaveApplication.Data;


namespace WaveApplication.Render {
    class PillarPointRender : IPointRender {
        private static readonly Brush _baseBrush;
        private static readonly Brush _realBrush;
        private static readonly Brush _exceedBrush;

        private class PillarRenderContext : RenderContext {
            private readonly double _canvasHeight;
            private int _index;
            private Point _location;
            private Size _size;
            private PillarLayer _layer;
            private double Y = Parameter.GetInstance().CanvasHeight;

            public override int Index {
                get { return _index; }
            }

            public PillarRenderContext(PillarLayer layer, double canvasHeight, double pillarWidth) {
                _index = 0;
                _layer = layer;
                _canvasHeight = canvasHeight;
                _location.X = 0;
                _size.Width = pillarWidth;
            }

            public override void RenderPoint(float basePoint, float realPoint) {
                if(realPoint < 0 )
                {
                    _location.Y = Y;
                    _size.Height = -realPoint;
                }
                else
                {
                    _location.Y = _canvasHeight - realPoint;
                    _size.Height = realPoint;
                }
             
                var rect = new Rect(_location, _size);
                var pillar = new PillarVisual(_realBrush, rect);
                _layer.Add(pillar);
                if (realPoint > basePoint) {
                    _size.Height = realPoint - basePoint;
                    rect.Size = _size;
                    var exceed = new PillarVisual(_exceedBrush, rect);
                    _layer.Add(exceed);
                }
                _location.X += _size.Width;
                _index++;
            }

            public override void Close() {
            }
        }

        private readonly double _pillarWidth;
        private readonly double _canvasHeight;

        static PillarPointRender() {
            var param = Parameter.GetInstance();
            _baseBrush = new SolidColorBrush(param.BasePointColor);
            _baseBrush.Freeze();
            _realBrush = new SolidColorBrush(param.RealPointColor);
            _realBrush.Freeze();
            _exceedBrush = new SolidColorBrush(param.ExceedColor);
            _exceedBrush.Freeze();
        }

        public PillarPointRender(double canvasHeight, double pillarWidth) {
            _canvasHeight = canvasHeight;
            _pillarWidth = pillarWidth;
        }

        public UIElement CreateImage() {
            return new PillarImage();
        }

        public void RenderItem(UIElement image, Viewport baseVp, Viewport realVp) {
            throw new NotImplementedException();
        }

        public void RenderBasePoints(UIElement image, Viewport baseVp) {
            var himage = image as PillarImage;
            Debug.Assert(himage != null && baseVp != null);
            var layer = himage.BaseDataLayer;
            Debug.Assert(layer != null);
            Size size = new Size(_pillarWidth, 0);
            Rect rect = new Rect();
            foreach (var p in baseVp) {
                size.Height = p;
                rect.Size = size;
                rect.Y = _canvasHeight - p;
                var pillar = new PillarVisual(_baseBrush, rect);
                layer.Add(pillar);
                rect.X += _pillarWidth;
            }
        }

        public RenderContext OpenRenderRealPointContext(UIElement image) {
            var himage = image as PillarImage;
            Debug.Assert(himage != null);
            return new PillarRenderContext(himage.RealDataLayer, _canvasHeight, _pillarWidth);
        }
    }
}
