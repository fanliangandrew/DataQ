using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveApplication.Data {
    class RealDataFile {

        public virtual void Open() {
        }

        public virtual void ReadBlcok(int blockId, StorageBlock block) {
        }

        public virtual void WriteBlock(StorageBlock block) {
        }

        public virtual void Close() {
        }
    }
}
