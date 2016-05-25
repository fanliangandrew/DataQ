using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveApplication.Data {
    class StorageBlock {
        public static readonly int StorageBlockSize;

        static StorageBlock() {
            var param=Parameter.GetInstance();
            StorageBlockSize = param.StorageBlockSize;
        }

        internal float[] _buffer;
        internal int _length;

        public int Length { get { return _length; } }

        public int Capacity { get { return StorageBlockSize - _length; } }

        public bool IsFull { get { return _length == StorageBlockSize; } }

        public float this[int index] { get { return _buffer[index]; } }

        public StorageBlock() {
            _buffer = new float[StorageBlockSize];
            _length = 0;
        }

        public StorageBlock(int size) {
            _buffer = new float[size];
            _length = 0;
        }

        public float Read(int index) {
            return _buffer[index];
        }

        public void Write(float e) {
            _buffer[_length++] = e;
        }

        public void Write(float[] es, int length) {
            for (int i = 0; i < length; i++) {
                _buffer[_length++] = es[i];
            }
        }

        public void Clear() {
            _length = 0;
        }
    }
}
