using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Threading;

namespace ColorPicker2 {
    /// <summary>
    /// HelpWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HelpWindow : Window {
        public HelpWindow() {
            InitializeComponent();

            Initialize();
        }

        DispatcherTimer MouseTimer = new DispatcherTimer();
        private void Initialize() {
            mousePosition = GetCursorPosition();

            MouseTimer.Interval = TimeSpan.FromMilliseconds(100);
            MouseTimer.Tick += new EventHandler(MouseTimer_Tick);
            MouseTimer.Start();
        }

        System.Drawing.Point mousePosition;
        private void MouseTimer_Tick(object sender, EventArgs e) {
            if(mousePosition != GetCursorPosition()) {
                MouseIndicator.IsChecked = true;
                mousePosition = GetCursorPosition();
            } else {
                MouseIndicator.IsChecked = false;
            }
        }

        private void DragRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            this.DragMove();
        }

        private void CloseBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.Hide();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.X:
                    KeyPress_Handler(e);
                    XKeyIndicator.IsChecked = true;
                    XKeyIndicator0.IsChecked = true;
                    break;

                case Key.C:
                    CKeyIndicator.IsChecked = true;
                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.X:
                    XKeyIndicator.IsChecked = false;
                    XKeyIndicator0.IsChecked = false;
                    break;

                case Key.C:
                    KeyPress_Handler(e);
                    CKeyIndicator.IsChecked = false;
                    break;
            }
        }

        public event RoutedEventHandler KeyPress;
        private void KeyPress_Handler(KeyEventArgs e) {
            KeyPress?.Invoke(this, e);
        }

        public event RoutedEventHandler ActivatedWindow;
        private void Window_Activated(object sender, EventArgs e) {
            ActivatedWindow?.Invoke(this, null);
            MouseTimer.Start();
        }

        public event RoutedEventHandler DeactivatedWindow;
        private void Window_Deactivated(object sender, EventArgs e) {
            DeactivatedWindow?.Invoke(this, null);

            MouseIndicator.IsChecked = false;
            XKeyIndicator.IsChecked = false;
            XKeyIndicator0.IsChecked = false;
            CKeyIndicator.IsChecked = false;
            MouseTimer.Stop();
        }

        #region CursorPosition

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out System.Drawing.Point lpPoint);
        public static System.Drawing.Point GetCursorPosition() {
            GetCursorPos(out System.Drawing.Point lpPoint);

            return lpPoint;
        }

        #endregion
    }
}
