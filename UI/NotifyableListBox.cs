using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WaveApplication.Render;

namespace WaveApplication.UI {
    class NotifyableListBox : ListBox {
        private ScrollViewer _scrollViewer;

        public ScrollViewer OwnedScrollViewer {
            get { return _scrollViewer; }
        }

        protected override bool IsItemItsOwnContainerOverride(object item) {
            return (item is ContentPresenter);
        }

        protected override DependencyObject GetContainerForItemOverride() {
            return new ContentPresenter();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
            var ritem = item as RenderItem;
            if (ritem != null) {
                ritem.IsDisplayed = true;
                var presenter = element as ContentPresenter;
                presenter.Content = ritem.RenderImage;
            }
        }

        /*protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
            var block = item as RenderItem<float>;
            if (block != null) {
                block.IsDisplayed = false;
                var image = element as Image;
                if (image != null) {
                    image.Source = null;
                }
            }
        }*/

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            bool find = FindScrollViewer(this);
            Debug.Assert(find);
        }

        private bool FindScrollViewer(Visual visual) {
            int count = VisualTreeHelper.GetChildrenCount(visual);
            for (int i = 0; i < count; i++) {
                Visual child = (Visual)VisualTreeHelper.GetChild(visual, i);
                var viewer = child as ScrollViewer;
                if (viewer != null) {
                    _scrollViewer = viewer;
                    return true;
                }
                if (FindScrollViewer(child)) {
                    return true;
                }
            }
            return false;
        }
    }
}
