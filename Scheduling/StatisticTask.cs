using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveApplication.Data;

namespace WaveApplication.Scheduling {

    delegate void ExceedLimitCallbackDelegate(ExceedLimitItem item);

    class StatisticTask {
        private readonly float _exceedPercent;
        private readonly float _underPercent;
        private readonly float _bluntLimitPercent;
        private readonly float _breakLimitPercent;

        private float _baseAmount;
        private StatisticResult _result;
        private ExceedLimitCallbackDelegate _callback;

        public StatisticResult StatisticResult {
            get { return _result; }
        }

        public StatisticTask(ExceedLimitCallbackDelegate callback) {
            var param = Parameter.GetInstance();
            _exceedPercent = param.ExceedPercent;
            _underPercent = param.UnderPercent;
            _bluntLimitPercent = param.BluntLimitPercent;
            _breakLimitPercent = param.BreakLimitPercent;
            _callback = callback;
        }

        public void Start() {
            _result = new StatisticResult();
            _result.StartTime = DateTime.Now;
            _baseAmount = 0;
        }

        public void Statistics(StorageBlock baseBlock, StorageBlock realBlock, int index, int length) {
            var boundary = index + length;
            for (; index < boundary; index++) {
                var bp = baseBlock[index];
                var rp = realBlock[index];
                if (rp > bp) {
                    var diff = rp - bp;
                    var percent = diff / bp;
                    if (percent > _breakLimitPercent) {
                        _result.ExceedAmount += diff;
                        var item = CreateExceedLimitItem(true);
                        OnExceedLimitOccur(item);
                    } else if (percent > _bluntLimitPercent) {
                        _result.ExceedAmount += diff;
                        var item = CreateExceedLimitItem(false);
                        OnExceedLimitOccur(item);
                    } else if (percent > _exceedPercent) {
                        _result.ExceedAmount += diff;
                        _result.ExceedCount++;
                    }
                } else if (rp < bp) {
                    var diff = bp - rp;
                    var percent = (bp - rp) / bp;
                    if (percent > _underPercent) {
                        _result.UnderAmount += diff;
                        _result.UnderCount++;
                    }
                }
                _baseAmount += bp;
            }
            _result.Count += length;
        }

        public void Stop() {
            _result.StopTime = DateTime.Now;
            _result.ExceedPercent = _result.ExceedAmount / _baseAmount;
            _result.UnderPercent = _result.UnderAmount / _baseAmount;
        }

        private ExceedLimitItem CreateExceedLimitItem(bool type) {
            return new ExceedLimitItem(
                DateTime.Now - _result.StartTime,
                type
            );
        }

        private void OnExceedLimitOccur(ExceedLimitItem item) {
            _result.ExceedLimitItems.Add(item);
            if (_callback != null) {
                _callback(item);
            }
        }
    }
}
