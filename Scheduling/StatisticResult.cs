using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveApplication.Scheduling {
    class ExceedLimitItem {
        public TimeSpan Timestamp { get; set; }
        public bool Type { get; set; }           //break: true, blunt: false

        public ExceedLimitItem(TimeSpan stamp, bool type) {
            Timestamp = stamp;
            Type = type;
        }
    }

    class StatisticResult {
        public DateTime StartTime;
        public DateTime StopTime;
        public int Count;
        public int ExceedCount;
        public int UnderCount;
        public float ExceedAmount;
        public float UnderAmount;
        public float ExceedPercent;
        public float UnderPercent;
        public List<ExceedLimitItem> ExceedLimitItems = new List<ExceedLimitItem>();
    }
}
