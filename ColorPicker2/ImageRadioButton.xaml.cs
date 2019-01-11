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
    /// ImageRadioButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageRadioButton : UserControl {
        public ImageRadioButton() {
            InitializeComponent();
        }

        public object ActiveIcon {
            get { return (object)GetValue(ActiveIconProperty); }
            set { SetValue(ActiveIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveIconProperty =
            DependencyProperty.Register("ActiveIcon", typeof(object), typeof(ImageRadioButton), new PropertyMetadata(0));

        public object DeactiveIcon {
            get { return (object)GetValue(DeactiveIconProperty); }
            set { SetValue(DeactiveIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeactiveIconProperty =
            DependencyProperty.Register("DeactiveIcon", typeof(object), typeof(ImageRadioButton), new PropertyMetadata(0));

        private bool IsValue = false;

        public event RoutedEventHandler ValueChanged;
        public Boolean IsChecked {
            get { return IsValue; }
            set {
                if(IsValue != value) {
                    IsValue = value;
                    PlayAnimation();
                }

                ValueChanged?.Invoke(this, null);
            }
        }

        private void PlayAnimation() {
            if (IsValue) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("Active");
                BeginStoryboard(storyBoard);
            } else {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("Deactive");
                BeginStoryboard(storyBoard);
            }
        }

        private void ImageRadioButtonControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
                IsChecked = true;
        }
    }
}
