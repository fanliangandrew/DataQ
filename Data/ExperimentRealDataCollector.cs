using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace WaveApplication.Data {
    class ExperimentRealDataCollector : ExperimentDataFile, IRealDataCollector {
        private bool _channel;

        public ExperimentRealDataCollector(bool channel)
            : base("real.dat") {
            _channel = channel;
        }

        public void Start() {
            _reader = new StreamReader(_path);
        }

        public void Stop() {
            _reader.Close();
        }

        public float CollectOnePoint() {
            throw new NotImplementedException();
        }

        public int CollectPoints(StorageBlock block, int length) {
            int i;
            for (i = 0; i < length; i++) {
                float? p;
                var tp1 = ReadPoint();
                var tp2 = ReadPoint();
                p = _channel ? tp1 : tp2;
                if (p == null) break;
                block.Write((float)p);
                /*var p = ReadPoint();
                if (p == null) break;
                block.Write((float)p);*/
            }
            return i;
        }
    }
}
