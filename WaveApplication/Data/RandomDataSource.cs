using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WaveApplication.Data {
    class RandomDataSource {
        protected readonly float Low;
        protected readonly float High;
        protected const float Span = 40;
        protected const float Step = Span / 2;
        protected Random _random;
        protected float _last;

        public RandomDataSource() {
            var param = Parameter.GetInstance();
            Low = (float)(param.CanvasHeight * 0.1);
            High = (float)(param.CanvasHeight * 0.6);
        }

        protected void Reset(bool couple) {
            _random = new Random();
            if (couple) {
                _random = new Random(_random.Next());
            }
            _last = (float)_random.NextDouble() * (High - Low) + Low;
        }

        protected float Next() {
            float step = (float)_random.NextDouble() * Span - Step;
            var cur = _last + step;
            if (cur > High || cur < Low) {
                _last -= step;
            } else {
                _last = cur;
            }
            return _last;
        }
    }
}
