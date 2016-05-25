using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveApplication.Data {
    interface IRealDataCollector {
        void Start();
        void Stop();
        float CollectOnePoint();
        int CollectPoints(StorageBlock block, int length);
    }
}
