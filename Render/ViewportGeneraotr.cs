using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WaveApplication.Data;

namespace WaveApplication.Render {
    class ViewportGenerator {
        private readonly int _span;
        private int _blockId;
        private int _offset;
        private int _size;
        private int _add;

        public int BlockID {
            get { return _blockId; }
        }

        public int ViewportOffset {
            get { return _offset; }
        }

        public int ViewportSize {
            get { return _size; }
        }

        public ViewportGenerator() {
            Debug.Assert(
                StorageBlock.StorageBlockSize >= Viewport.ViewportSize &&
                StorageBlock.StorageBlockSize % Viewport.ViewportSize == 0);
            _span = StorageBlock.StorageBlockSize / Viewport.ViewportSize;
        }

        public void Reset(int blockId) {
            _blockId = blockId;
            _offset = 0;
            _size = 0;
            _add = 0;
        }

        public int EnlargeViewport(int length, out bool needNewViewport) {
            _size += length;
            length += _add;
            if (_size >= Viewport.ViewportSize) {
                _size -= Viewport.ViewportSize;
                _add = _size;
                length -= _size;
                needNewViewport = true;
            } else {
                _add = 0;
                needNewViewport = false;
            }
            return length;
        }

        public Viewport Next(StorageBlock block) {
            var vp = new Viewport(_blockId, _offset, block);
            Next();
            return vp;
        }

        public Viewport Next(StorageBlock block, out bool needNewBlock) {
            var vp = new Viewport(_blockId, _offset, block);
            needNewBlock = Next();
            return vp;
        }

        private bool Next() {
            _offset += Viewport.ViewportSize;
            if (_offset == StorageBlock.StorageBlockSize) {
                _offset = 0;
                _blockId++;
                return true;
            }
            return false;
        }
    }
}
