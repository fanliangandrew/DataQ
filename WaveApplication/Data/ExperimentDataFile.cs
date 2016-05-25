using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WaveApplication.Data {
    class ExperimentDataFile {
        protected readonly string _path;
        protected StreamReader _reader;
        protected readonly char[] _buffer = new char[25];

        public ExperimentDataFile(string path) {
            _path = path;
        }

        protected float? ReadPoint() {
            int c;
            while (true) {
                c = _reader.Read();
                if (c != ' ') break;
            }
            if (c < 0) return null;
            _buffer[0] = (char)c;
            int i;
            for (i = 1; ; i++) {
                c = _reader.Read();
                if (c == ',') break;
                _buffer[i] = (char)c;
            }
            string data = new string(_buffer, 0, i);
            return (float)Double.Parse(data);
        }
    }
}
