using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WaveApplication {
    class Parameter {
        private static Parameter _instance;

        public static Parameter GetInstance() {
            return _instance;
        }

        static Parameter() {
            _instance = new Parameter();
            _instance.StorageBlockSize = 1024;
            _instance.ViewportSize = 256;
            _instance.PointWidth = 1.8;
            _instance.CanvasHeight = 250;
            _instance.BasePointColor = Colors.LightGreen;
            _instance.RealPointColor = Colors.Blue;
            _instance.ExceedColor = Colors.Purple;
            _instance.BluntLimitColor = Colors.Orange;
            _instance.BreakLimitColor = Colors.Red;
            _instance.LimitLineThickness = 0.7;
            _instance.ExceedPercent = 0.1f;
            _instance.UnderPercent = 0.1f;
            _instance.BluntLimitPercent = 0.3f;
            _instance.BreakLimitPercent = 0.6f;
        }

        public int StorageBlockSize { get; set; }

        public int ViewportSize { get; set; }

        public double PointWidth { get; set; }

        public double CanvasHeight { get; set; }

        public Color BasePointColor { get; set; }

        public Color RealPointColor { get; set; }

        public Color ExceedColor { get; set; }

        public Color BluntLimitColor { get; set; }

        public Color BreakLimitColor { get; set; }

        public double LimitLineThickness { get; set; }

        public float ExceedPercent { get; set; }

        public float UnderPercent { get; set; }

        public float BluntLimitPercent { get; set; }

        public float BreakLimitPercent { get; set; }

        private Parameter() {
        }
    }
}
