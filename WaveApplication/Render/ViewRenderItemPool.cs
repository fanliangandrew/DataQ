using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using WaveApplication.Data;

namespace WaveApplication.Render {
    class ViewRenderItemPool : RenderItemPool, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private delegate void PropertyChangedDelegate(string property);
        private readonly Dispatcher _dispatcher;
        private RenderItem[] _items;
        private readonly int _size;
        private int _newIndex;
        private int _rendreIndex;

        public RenderItem ViewRenderItem {
            get { return _items[_rendreIndex]; }
        }

        public ViewRenderItemPool(IPointRender render, Dispatcher dispatcher) {
            _dispatcher = dispatcher;
            _size = StorageBlock.StorageBlockSize / Viewport.ViewportSize * 2;
            _items = new RenderItem[_size];
            for (int i = 0; i < _size; i++) {
                _items[i] = new RenderItem(render);
            }
            _newIndex = 0;
            _rendreIndex = _size - 1;
        }

        public override RenderItem CreateRenderItem(IPointRender render) {
            var i = _newIndex++;
            if (_newIndex >= _size) {
                _newIndex = 0;
            }
            _items[i].AllocateResource();
            return _items[i];
        }

        public override RenderItem GetNextRenderingRealPointItem() {
            if (++_rendreIndex >= _size) {
                _rendreIndex = 0;
            }
            _dispatcher.BeginInvoke(
                (PropertyChangedDelegate)OnPropertyChanged,
                "ViewRenderItem"
            );
            return _items[_rendreIndex];
        }

        public override void Freeze() {
        }

        protected void OnPropertyChanged(string property) {
            if (PropertyChanged != null) {
                var e = new PropertyChangedEventArgs(property);
                PropertyChanged(this, e);
            }
        }
    }
}
