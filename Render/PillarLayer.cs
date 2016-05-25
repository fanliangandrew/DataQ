using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WaveApplication.Render {
    class PillarLayer : FrameworkElement {
        private List<Visual> _list;

        public PillarLayer() {
            _list = new List<Visual>();
        }

        public PillarLayer(int size) {
            _list = new List<Visual>(size);
        }

        public void Add(Visual visual) {
            _list.Add(visual);
            AddVisualChild(visual);
        }

        protected override int VisualChildrenCount {
            get { return _list.Count; }
        }

        protected override Visual GetVisualChild(int index) {
            return _list[index];
        }

    }
}
