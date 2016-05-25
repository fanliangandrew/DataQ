using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveApplication.Data;

namespace WaveApplication.Data {
    class AverageRealDataCollect : IRealDataCollector {
        private readonly IRealDataCollector _collector;
        private readonly int _n;
        private readonly StorageBlock _block;

        public AverageRealDataCollect(IRealDataCollector collector, int n) {
            _collector = collector;
            _n = n;
            _block = new StorageBlock(n);
        }

        public void Start() {
            _collector.Start();
        }

        public void Stop() {
            _collector.Stop();
        }

        public float CollectOnePoint() {
            throw new NotImplementedException();
        }

        public int CollectPoints(StorageBlock block, int length) {
            int i;
            for (i = 0; i < length; i++) {
                var p = OnePoint();
                if (p == null) break;
                block.Write((float)p);
            }
            return i;
        }

        private float? OnePoint() {
            _block.Clear();
            int length = _collector.CollectPoints(_block, _n);
            //how to handle?
            if (length < _n) return null;
            return Average();
        }

        private float Average() {
            float sum = 0;
            for (int i = 0; i < _n; i++) {
                sum += _block[i];
            }
            return sum / _n;
        }
    }
}
