using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using WaveApplication.Data;
using WaveApplication.Render;

namespace WaveApplication.Scheduling {
    class RenderTask {
        private delegate void GetRenderingItemDelegate(Viewport viewport);
        private delegate void RenderBaseDataDelegate(Viewport viewport);
        private delegate void RenderRealDataDelegate(RenderItem item, int count);
        private delegate void FinishRenderItem(RenderItem item);

        private Dispatcher _dispatcher;
        private IPointRender _render;

        private RenderItemPool _pool;
        private ViewportGenerator _baseVpGenerator;
        private ViewportGenerator _realVpGenerator;
        private RenderItem _item;

        private bool _needNewItem;
        private AutoResetEvent _isCreated;

        public RenderTask(Dispatcher dispatcher, IPointRender render, RenderItemPool pool) {
            _dispatcher = dispatcher;
            _render = render;
            _pool = pool;
            _baseVpGenerator = new ViewportGenerator();
            _realVpGenerator = new ViewportGenerator();
            _isCreated = new AutoResetEvent(false);
        }

        public void Begin(StorageBlock realBlock, StorageBlock baseBlock, StorageBlock preBaseBlock) {
            _baseVpGenerator.Reset(0);
            _realVpGenerator.Reset(0);
            _isCreated.Reset();

            RenderBasePointsBlock(baseBlock);
            RenderBasePointsBlock(preBaseBlock);
            _needNewItem = true;
            PrepareRenderItem(realBlock);
        }

        public void Finish() {
            _dispatcher.BeginInvoke((Action)_pool.Freeze);
        }

        public bool RenderRealPoints(int length) {
            if (_needNewItem) {
                _isCreated.WaitOne();
            }
            Debug.Assert(_realVpGenerator.ViewportSize < Viewport.ViewportSize);
            length = _realVpGenerator.EnlargeViewport(length, out _needNewItem);
            _dispatcher.BeginInvoke(
                (RenderRealDataDelegate)((item, count) => {
                    item.RenderRealPoints(count);
                }),
                _item,
                length
            );
            if (_needNewItem) {
                _dispatcher.BeginInvoke(
                    (FinishRenderItem)((item) => {
                        item.FinishRenderRealData();
                    }),
                    _item
                );
            }
            return _needNewItem;
        }

        public void PrepareRenderItem(StorageBlock block) {
            var vp = _realVpGenerator.Next(block);
            _dispatcher.BeginInvoke(
                (GetRenderingItemDelegate)((viewport) => {
                    _item = _pool.GetNextRenderingRealPointItem();
                    _item.RealViewport = viewport;
                    _item.BeginRenderRealData();
                    _isCreated.Set();
                }),
                vp
            );
        }

        public void RenderBasePointsBlock(StorageBlock block) {
            bool need;
            do {
                var vp = _baseVpGenerator.Next(block, out need);
                _dispatcher.BeginInvoke(
                    (RenderBaseDataDelegate)((viewport) => {
                        var item = _pool.CreateRenderItem(_render);
                        item.BaseViewport = viewport;
                        item.RenderBasePoints();
                    }),
                    vp
                );
            } while (!need);
        }
    }
}
