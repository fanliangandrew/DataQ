using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using WaveApplication.Data;

namespace WaveApplication.Render {
    class Viewport : IEnumerable<float> {
        public static readonly int ViewportSize;

        static Viewport() {
            var param = Parameter.GetInstance();
            ViewportSize = param.ViewportSize;
        }

        private StorageBlock _storageBlock;
        private readonly int _blockId;
        private readonly int _offset;

        public int BlockId {
            get { return _blockId; }
        }

        public int Offset {
            get { return _offset; }
        }

        public float this[int index] {
            get {
                return _storageBlock[_offset + index];
            }
        }

        public Viewport(int blockId, int offset, StorageBlock storage) {
            _blockId = blockId;
            _offset = offset;
            _storageBlock = storage;
        }

        public IEnumerator<float> GetEnumerator() {
            int boundary = _offset + ViewportSize;
            for (int i = _offset; i < boundary; i++) {
                yield return _storageBlock[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
