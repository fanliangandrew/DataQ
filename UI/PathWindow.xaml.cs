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
    /// PathWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PathWindow : Window {
        private bool _isCollect;

        public PathWindow(bool isCollect) {
            InitializeComponent();

            _isCollect = isCollect;
            RealTextBlock.Text = isCollect ?
                "采集数据保存路径" : "采集数据文件路径";
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
