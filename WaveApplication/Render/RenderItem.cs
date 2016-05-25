using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WaveApplication.Data;

namespace WaveApplication.Render {
    class RenderItem : INotifyPropertyChanged {
        private Viewport _baseViewport;
        private Viewport _realViewport;
        private IPointRender _render;
        private RenderContext _renderContext;
        private UIElement _image;

        public event PropertyChangedEventHandler PropertyChanged;

        public Viewport BaseViewport {
            get { return _baseViewport; }
            set { _baseViewport = value; }
        }

        public Viewport RealViewport {
            get { return _realViewport; }
            set { _realViewport = value; }
        }

        public bool IsDisplayed { get; set; }

        public virtual UIElement RenderImage {
            get { return _image; }
            private set {
                if (_image != value) {
                    _image = value;
                    OnPropertyChanged("RenderImage");
                }
            }
        }

        public RenderItem(IPointRender render) {
            _render = render;
        }

        public void AllocateResource() {
            RenderImage = _render.CreateImage();
        }

        public void ReleaseResource() {
            RenderImage = null;
        }

        public void RenderBasePoints() {
            Debug.Assert(_image != null);
            _render.RenderBasePoints(_image, _baseViewport);
        }

        public void BeginRenderRealData() {
            Debug.Assert(_image != null);
            _renderContext = _render.OpenRenderRealPointContext(_image);
        }

        public void RenderRealPoints(int count) {
            Debug.Assert(_renderContext != null);
            int boundary = _renderContext.Index + count;
            for (int i = _renderContext.Index; i < boundary; i++) {
                /*Debug.Write(String.Format("({0},{1:f4},{2:f4}) ",
                      i,_baseViewport[i], _realViewport[i]));*/
                _renderContext.RenderPoint(
                    _baseViewport[i], _realViewport[i]
                );
            }
        }

        public void FinishRenderRealData() {
            Debug.Assert(_renderContext != null);
            _renderContext.Close();
        }

        protected void OnPropertyChanged(string property) {
            if (PropertyChanged != null) {
                var e = new PropertyChangedEventArgs(property);
                PropertyChanged(this, e);
            }
        }
    }
}
