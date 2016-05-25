using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WaveApplication.UI {
    /// <summary>
    /// DrawOptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DrawOptionWindow : Window {

        public int N { get; private set; }

        public int Channel { get; private set; }

        public DrawOptionWindow() {
            InitializeComponent();
        }

        private void SumbitButton_Click(object sender, RoutedEventArgs e) {
            int n;
            if (!Int32.TryParse(NTextBox.Text, out n)) {

            }
            N = n;
            Channel = ChannelComboBox.SelectedIndex;
            this.Close();
        }
    }
}
