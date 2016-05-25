using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WaveApplication.Data {
    class RandomRealDataCollector : RandomDataSource, IRealDataCollector {

        public void Start() {
            Reset(false);
        }

        public void Stop() {
        }

        public float CollectOnePoint() {
            Thread.Sleep(10);
            return Next();
        }

        public int CollectPoints(StorageBlock block, int length) {
            Thread.Sleep(50);
            for (int i = 0; i < length; i++) {
               //block.Write(Next());
                block.Write(0.0001f);
            }
            return length;
        }
    }
}
