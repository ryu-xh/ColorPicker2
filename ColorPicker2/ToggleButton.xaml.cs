using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPicker2 {
    /// <summary>
    /// ToggleButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ToggleButton : UserControl {
        public ToggleButton() {
            InitializeComponent();
        }

        private bool IsValue = false;
        private void ToggleRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            IsChecked = !IsChecked;
        }

        public Boolean IsChecked {
            get { return IsValue; }
            set {
                IsValue = value;
                PlayAnimation();

                ValueChanged?.Invoke(this, null);
            }
        }

        public event RoutedEventHandler ValueChanged;
        public virtual void Value_Changed(object sender, RoutedEventArgs args) {
            ValueChanged?.Invoke(this, args);
        }

        private void PlayAnimation() {
            if (IsValue) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("Enable");
                BeginStoryboard(storyBoard);
            } else {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("Disable");
                BeginStoryboard(storyBoard);
            }
        }

        private void ToggleControl_Loaded(object sender, RoutedEventArgs e) {
            if (IsValue) {
                ColorRect.Opacity = 100;
                rectangle.Margin = new Thickness(20, 4, 4, 4);
            }
        }

    }
}
