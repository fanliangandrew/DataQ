using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WaveApplication.UI {
    class ExceedLimitTypeToStringConverter : IValueConverter {
        private static string[] Information = new string[2]{
            "超出blunt警戒线",
            "超出break警戒线"
        };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            int index = (bool)value ? 1 : 0;
            return Information[index];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
