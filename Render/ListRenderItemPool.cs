using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WaveApplication.Data;

namespace WaveApplication.Render {
    class ListRenderItemPool : RenderItemPool {
        protected ObservableCollection<RenderItem> _items;
        protected int _realIndex;
        protected bool _isFreezed;

        public ObservableCollection<RenderItem> RenderItems {
            get { return _items; }
        }

        public ListRenderItemPool() {
            _items = new ObservableCollection<RenderItem>();
            _isFreezed = false;
            _realIndex = 0;
        }

        public override RenderItem CreateRenderItem(IPointRender render) {
            Debug.Assert(!_isFreezed);
            var item = new RenderItem(render);
            item.AllocateResource();
            _items.Add(item);
            return item;
        }

        public override RenderItem GetNextRenderingRealPointItem() {
            Debug.Assert(!_isFreezed && _realIndex < _items.Count);
            return _items[_realIndex++];
        }

        public override void Freeze() {
            _isFreezed = true;
            for (int i = _items.Count-1; i >=_realIndex; i--) {
                _items.RemoveAt(i);
            }
        }
    }
}
