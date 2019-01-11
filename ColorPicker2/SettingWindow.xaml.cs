using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ColorPicker2
{
    /// <summary>
    /// SettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            Initialize();
        }

        bool isInit = false;
        private void Initialize() {
            AutoCopyToggle.IsChecked = Properties.Settings.Default.AutoCopy;
            HideCopyToggle.IsChecked = Properties.Settings.Default.HideCopyButton;

            isInit = true;
        }

        private void CloseBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.Hide();
        }

        private void DragRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            this.DragMove();
        }

        private void AutoCopyToggle_ValueChanged(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.AutoCopy = AutoCopyToggle.IsChecked;

            if(isInit)
                Apply();
        }

        private void HideCopyToggle_ValueChanged(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.HideCopyButton = HideCopyToggle.IsChecked;

            if(isInit)
                Apply();
        }

        private void Apply() {
            Properties.Settings.Default.Save();
            SettingApply();
        }

        // 대리자
        public delegate void SettingApplyHandler();
        // 이벤트 처리기
        public event SettingApplyHandler SettingApply;
    }
}
