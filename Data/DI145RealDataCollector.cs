using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace WaveApplication.Data
{
    class DI145RealDataCollector : DI145DataSource,IRealDataCollector
    {
        public float CollectOnePoint()
        {
            throw new NotImplementedException();
        }
        public int CollectPoints(StorageBlock block, int length)
        {
            Thread.Sleep(50);
    
            for (int i = 0; i < length; i++)
            {
                float data = getDI145Data();
                if(data ==0f)
                {
                    data = 0.015f;
                }
                block.Write(data*30);
            }
            return length;
        }

        public void Start()
        {
            initDI145();
        }

        public void Stop()
        {
            stopWork();
        }
    }
}
