using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveApplication.Data {
    interface IBaseDataSource {
        void SetOffset(int offset);
        void LoadBlock(StorageBlock block);
    }
}
