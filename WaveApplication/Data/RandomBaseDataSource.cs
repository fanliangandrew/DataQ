using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveApplication.Data {
    class RandomBaseDataSource : RandomDataSource, IBaseDataSource {

        public RandomBaseDataSource() {
            Reset(true);
        }

        public void LoadBlock(StorageBlock block) {
            int length = StorageBlock.StorageBlockSize;
            while (--length >= 0) {
                block.Write(Next());
            }
        }

        public void SetOffset(int offset) {
            throw new NotImplementedException();
        }
    }
}
