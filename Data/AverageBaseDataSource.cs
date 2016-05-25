using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveApplication.Data {
    class AverageBaseDataSource : IBaseDataSource {
        private readonly IBaseDataSource _source;
        private readonly int _n;
        private readonly StorageBlock _block;
        private int _index;

        public AverageBaseDataSource(IBaseDataSource source, int n) {
            _source = source;
            _n = n;
            _block = new StorageBlock();
            _index = 0;
        }

        public void SetOffset(int offset) {
            throw new NotImplementedException();
        }

        public void LoadBlock(StorageBlock block) {
            int capacity = block.Capacity;
            while (--capacity >= 0) {
                var p = OnePoint();
                if (p == null) break;
                block.Write((float)p);
            }
        }

        private float? OnePoint() {
            float sum = 0;
            int boundary;
            if (_block.Length - _index < _n) {
                boundary = _n - (_block.Length - _index);
                for (; _index < _block.Length; _index++) {
                    sum += _block[_index];
                }
                _block.Clear();
                _source.LoadBlock(_block);
                _index = 0;
            } else {
                boundary = _index + _n;
            }
            for (; _index < boundary; _index++) {
                sum += _block[_index];
            }
            return sum / _n;
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
