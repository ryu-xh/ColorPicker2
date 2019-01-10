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

        bool isEnable = false;
        private void ToggleRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            PlayAnimation();

            isEnable = !isEnable;
        }

        public Boolean IsChecked {
            get { return isEnable; }
            set { isEnable = value; }
        }


        private void PlayAnimation() {
            if (isEnable) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("Disable");
                BeginStoryboard(storyBoard);
            } else {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("Enable");
                BeginStoryboard(storyBoard);
            }
        }

        private void ToggleControl_Loaded(object sender, RoutedEventArgs e) {
            if (isEnable) {
                ColorRect.Opacity = 100;
                rectangle.Margin = new Thickness(20, 4, 4, 4);
            }
        }
    }
}
