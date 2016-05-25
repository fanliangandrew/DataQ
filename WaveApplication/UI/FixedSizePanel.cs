using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WaveApplication.UI {
    class FixedSizePanel : VirtualizingPanel, IScrollInfo {
        public static readonly DependencyProperty FixedItemWidthProperty =
            DependencyProperty.Register(
                "FixedItemWidth",
                typeof(Double),
                typeof(FixedSizePanel),
                new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsMeasure)
            );

        public static readonly DependencyProperty TailWidthProperty =
            DependencyProperty.Register(
                "TailWidth",
                typeof(Double),
                typeof(FixedSizePanel),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure)
            );

        public static readonly DependencyProperty CacheCountProperty =
            DependencyProperty.Register(
                "CacheCount",
                typeof(Int32),
                typeof(FixedSizePanel),
                new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.AffectsMeasure)
            );

        private struct ScrollData {
            public Size ExtentSize;
            public Size ViewportSize;
            public ScrollViewer ScrollOwner;
            public double Offset;
        }

        private ScrollData _scrollData = new ScrollData();

        public bool CanHorizontallyScroll {
            get { return true; }
            set { }
        }

        public bool CanVerticallyScroll {
            get { return false; }
            set { }
        }

        public double ViewportWidth {
            get { return _scrollData.ViewportSize.Width; }
        }

        public double ViewportHeight {
            get { return _scrollData.ViewportSize.Height; }
        }

        public double ExtentWidth {
            get { return _scrollData.ExtentSize.Width; }
        }

        public double ExtentHeight {
            get { return _scrollData.ExtentSize.Height; }
        }

        public void LineUp() { }

        public void LineDown() { }

        public void LineLeft() {
            SetHorizontalOffset(_scrollData.Offset - FixedItemWidth / 5);
        }

        public void LineRight() {
            SetHorizontalOffset(_scrollData.Offset + FixedItemWidth / 5);
        }

        public void MouseWheelUp() {
            LineLeft();
        }

        public void MouseWheelDown() {
            LineRight();
        }

        public void MouseWheelLeft() {
            LineLeft();
        }

        public void MouseWheelRight() {
            LineDown();
        }

        public void PageUp() {
        }

        public void PageDown() {
        }

        public void PageLeft() {
            SetHorizontalOffset(_scrollData.Offset - FixedItemWidth);
        }

        public void PageRight() {
            SetHorizontalOffset(_scrollData.Offset + FixedItemWidth);
        }

        public Rect MakeVisible(System.Windows.Media.Visual visual, Rect rectangle) {
            return new Rect(0, 0, 0, 0);
        }

        public ScrollViewer ScrollOwner {
            get { return _scrollData.ScrollOwner; }
            set { _scrollData.ScrollOwner = value; }
        }

        public double HorizontalOffset {
            get { return _scrollData.Offset; }
        }

        public double VerticalOffset {
            get { return 0.0; }
        }

        public void SetHorizontalOffset(double offset) {
            if (_scrollData.ScrollOwner != null) {
                offset = CalculateOffset(offset);
                if (_scrollData.Offset != offset) {
                    _scrollData.Offset = offset;
                    base.InvalidateMeasure();
                }
            }
        }

        public void SetVerticalOffset(double offset) { }

        public double FixedItemWidth {
            get { return (double)GetValue(FixedItemWidthProperty); }
            set { SetValue(FixedItemWidthProperty, value); }
        }

        public double TailWidth {
            get { return (double)GetValue(FixedSizePanel.TailWidthProperty); }
            set { SetValue(FixedSizePanel.TailWidthProperty, value); }
        }

        public int CacheCount {
            get { return (int)GetValue(CacheCountProperty); }
            set { SetValue(CacheCountProperty, value); }
        }

        private ItemsControl ItemsOwner {
            get { return ItemsControl.GetItemsOwner(this); }
        }

        protected override Size MeasureOverride(Size availableSize) {
            //Debug.WriteLine("In measure override");
            double width, height;
            width = CalculateExtentWidth();
            if (_scrollData.ScrollOwner == null) {
                height = AddRealizingItems(0, ItemsOwner.Items.Count - 1, availableSize.Height);
                return new Size(width, height);
            } else {
                int start, end;
                var extent = new Size(width, availableSize.Height);
                VerifyScrollInfo(availableSize, extent);
                CalculateRealizingRange(out start, out end);
                //Debug.WriteLine("start,end {0} {1}", start, end);
                if (start <= end) {
                    AddRealizingItems(start, end, availableSize.Height);
                    RemoveVirtualizingItems(start, end);
                }
                return availableSize;
            }
        }

        protected override Size ArrangeOverride(Size finalSize) {
            //Debug.WriteLine("In arrange override: {0}", InternalChildren.Count);
            var fixedWidth = FixedItemWidth;
            var children = InternalChildren;
            var generator = base.ItemContainerGenerator;
            var position = new GeneratorPosition(0, 0);
            var index = generator.IndexFromGeneratorPosition(position);
            var start = -_scrollData.Offset + index * fixedWidth;
            Point point = new Point(start, 0);
            Size size = new Size(fixedWidth, finalSize.Height);
            Rect rect = new Rect(point, size);
            foreach (UIElement child in children) {
                child.Arrange(rect);
                point.X += FixedItemWidth;
                rect.Location = point;
            }
            if (_scrollData.ScrollOwner != null) {
                _scrollData.ScrollOwner.InvalidateScrollInfo();
            }
            return finalSize;
        }

        private double CalculateExtentWidth() {
            var count = ItemsOwner.Items.Count;
            return count > 0 ? count * FixedItemWidth + TailWidth : 0.0;
        }

        private double CalculateOffset(double offset) {
            if (offset < 0 || _scrollData.ViewportSize.Width >= _scrollData.ExtentSize.Width) {
                offset = 0;
            } else {
                var boundary = _scrollData.ExtentSize.Width - _scrollData.ViewportSize.Width;
                if (offset > boundary) {
                    offset = boundary;
                }
            }
            return offset;
        }

        private void VerifyScrollInfo(Size viewport, Size extent) {
            bool viewportChanged, extentChanged;
            viewportChanged = _scrollData.ViewportSize != viewport;
            if (viewportChanged) {
                _scrollData.ViewportSize = viewport;
            }
            extentChanged = _scrollData.ExtentSize != extent;
            if (extentChanged) {
                _scrollData.ExtentSize = extent;
            }
            if (viewportChanged || extentChanged) {
                _scrollData.Offset = CalculateOffset(_scrollData.Offset);
            }
        }

        private void CalculateRealizingRange(out int start, out int end) {
            double itemWidth = FixedItemWidth;
            int cacheCount = CacheCount;
            start = (int)Math.Floor(_scrollData.Offset / itemWidth) - cacheCount;
            if (start < 0) {
                start = 0;
            }
            end = (int)Math.Floor((_scrollData.ViewportSize.Width + _scrollData.Offset) / itemWidth) + cacheCount;
            int count = ItemsOwner.Items.Count;
            if (end >= count) {
                end = count - 1;
            }
        }

        private double AddRealizingItems(int start, int end, double availableHeight) {
            Size size = new Size(FixedItemWidth, availableHeight);
            double height = 0;
            //here is bug of WPF,
            //we must access InternalChildren before accessing ItemContainerGenerator
            var children = this.InternalChildren;
            var generator = this.ItemContainerGenerator;
            var position = generator.GeneratorPositionFromIndex(start);
            int index = position.Offset == 0 ? position.Index : position.Index + 1;
            using (generator.StartAt(position, GeneratorDirection.Forward, true)) {
                bool isNew;
                for (int i = start; i <= end; i++, index++) {
                    var child = generator.GenerateNext(out isNew) as UIElement;
                    if (isNew) {
                        if (index >= InternalChildren.Count) {
                            AddInternalChild(child);
                        } else {
                            InsertInternalChild(index, child);
                        }
                        generator.PrepareItemContainer(child);
                    }
                    child.Measure(size);
                    if (child.DesiredSize.Height > height) {
                        height = child.DesiredSize.Height;
                    }
                }
            }
            return height;
        }

        private void RemoveVirtualizingItems(int start, int end) {
            int index, i, l;
            var generator = base.ItemContainerGenerator;
            var position = new GeneratorPosition(0, 0);
            i = 0;
            do {
                position.Index = i++;
                index = generator.IndexFromGeneratorPosition(position);
            } while (index < start);
            l = i - 1;
            if (l > 0) {
                position.Index = 0;
                generator.Remove(position, l);
                RemoveInternalChildRange(0, l);
            }
            i = InternalChildren.Count - 1;
            do {
                position.Index = i--;
                index = generator.IndexFromGeneratorPosition(position);
            } while (index > end);
            l = InternalChildren.Count - 2 - i;
            if (l > 0) {
                position.Index = i + 2;
                generator.Remove(position, l);
                RemoveInternalChildRange(i + 2, l);
            }
        }
    }
}
