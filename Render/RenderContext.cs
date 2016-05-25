using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WaveApplication.Render {
    abstract class RenderContext {
        public abstract int Index { get; }
        public abstract void RenderPoint(float basePoint, float realPoint);
        public abstract void Close();
    }
}
