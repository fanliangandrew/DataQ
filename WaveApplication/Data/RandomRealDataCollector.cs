using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Data;

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
                block.Write(Next());
                string str = @"Data Source=WIN-13B7K72BH2U;Initial Catalog=logintest;Persist Security Info=True;User ID=sa;Password=sa";
                SqlConnection conn = new SqlConnection(str);
                conn.Open();
                string selectsql = "insert into logintest (data) values (" + _last + ")";
                SqlCommand cmd = new SqlCommand(selectsql, conn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader sdr;
                sdr = cmd.ExecuteReader();
                conn.Close();
            }
            return length;
        }
    }
}
