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

            formatValue = Properties.Settings.Default.Format;

            switch (formatValue) {
                case 0:
                    HexRadioButton.IsChecked = true;
                    break;
                case 1:
                    RGBRadioButton.IsChecked = true;
                    break;
                case 2:
                    HSVRadioButton.IsChecked = true;
                    break;
                case 3:
                    HSLRadioButton.IsChecked = true;
                    break;
            }

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

        byte formatValue = 0;
        private void HexRadioButton_ValueChanged(object sender, RoutedEventArgs e) {
            if (HexRadioButton.IsChecked) {
                formatValue = 0;
                Properties.Settings.Default.Format = formatValue;
                RGBRadioButton.IsChecked = false;
                HSVRadioButton.IsChecked = false;
                HSLRadioButton.IsChecked = false;
            }
                
            if (isInit)
                Apply();
        }

        private void RGBRadioButton_ValueChanged(object sender, RoutedEventArgs e) {
            if (RGBRadioButton.IsChecked) {
                formatValue = 1;
                Properties.Settings.Default.Format = formatValue;
                HexRadioButton.IsChecked = false;
                HSVRadioButton.IsChecked = false;
                HSLRadioButton.IsChecked = false;
            }
                
            if (isInit)
                Apply();
        }

        private void HSVRadioButton_ValueChanged(object sender, RoutedEventArgs e) {
            if (HSVRadioButton.IsChecked) {
                formatValue = 2;
                Properties.Settings.Default.Format = formatValue;
                HexRadioButton.IsChecked = false;
                RGBRadioButton.IsChecked = false;
                HSLRadioButton.IsChecked = false;
            }
                
            if (isInit)
                Apply();
        }

        private void HSLRadioButton_ValueChanged(object sender, RoutedEventArgs e) {
            if (HSLRadioButton.IsChecked) {
                formatValue = 3;
                Properties.Settings.Default.Format = formatValue;
                HexRadioButton.IsChecked = false;
                RGBRadioButton.IsChecked = false;
                HSVRadioButton.IsChecked = false;
            }

            if (isInit)
                Apply();
        }
    }
}
