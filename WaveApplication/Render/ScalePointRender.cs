using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WaveApplication.Data;

namespace WaveApplication.Render {
    class ScalePointRender : IPointRender {

        private class ScaleRenderContext : RenderContext {
            private RenderContext _context;
            private ScalePointRender _render;

            public override int Index {
                get { return _context.Index; }
            }

            public ScaleRenderContext(RenderContext context, ScalePointRender render) {
                _context = context;
                _render = render;
            }

            public override void RenderPoint(float basePoint, float realPoint) {
                _context.RenderPoint(
                    _render.Scale(basePoint),
                    _render.Scale(realPoint)
                );
            }

            public override void Close() {
                _context.Close();
            }
        }

        private IPointRender _render;
        private float _scale;
        private float _offset;
        private StorageBlock _block;

        public ScalePointRender(IPointRender render, float scale, float offset) {
            _render = render;
            _scale = scale;
            _offset = offset;
            _block = new StorageBlock(Viewport.ViewportSize);
        }

        public UIElement CreateImage() {
            return _render.CreateImage();
        }

        public void RenderItem(UIElement image, Viewport baseVp, Viewport realVp) {
            throw new NotImplementedException();
        }

        public void RenderBasePoints(UIElement image, Viewport baseVp) {
            var vp = CreateScaledViewport(baseVp);
            _render.RenderBasePoints(image, vp);
        }

        public RenderContext OpenRenderRealPointContext(UIElement image) {
            var context = _render.OpenRenderRealPointContext(image);
            return new ScaleRenderContext(context, this);
        }

        protected float Scale(float p) {
            var t = p + _offset;
            return t >= 0 ? t * _scale : 0;
        }

        private Viewport CreateScaledViewport(Viewport vp) {
            int i = 0;
            var buffer = _block._buffer;
            foreach (var p in vp) {
                buffer[i++] = Scale(p);
            }
            return new Viewport(0, 0, _block);
        }
    }
}
