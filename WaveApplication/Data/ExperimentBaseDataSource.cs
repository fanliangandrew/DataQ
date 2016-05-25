using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace WaveApplication.Data {
    class ExperimentBaseDataSource : ExperimentDataFile, IBaseDataSource {
        private readonly bool _channel;

        public ExperimentBaseDataSource(bool channel)
            : base("base.dat") {
            _reader = new StreamReader(_path);
            _channel = channel;
        }

        public void SetOffset(int offset) {
            throw new NotImplementedException();
        }

        public void LoadBlock(StorageBlock block) {
            int length = StorageBlock.StorageBlockSize;
            for (int i = 0; i < length; i++) {
                float? p;
                var tp1 = ReadPoint();
                var tp2 = ReadPoint();
                p = _channel ? tp1 : tp2;
                block.Write(p == null ? 0.0f : (float)p);
                //var p = ReadPoint();
                //block.Write(p == null ? 0f : (float)p);
            }
        }
    }
}
