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
    /// IconIndicator.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ActionIndicator : UserControl {
        public ActionIndicator() {
            InitializeComponent();
        }

        public object Icon {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(ActionIndicator), new PropertyMetadata(0));

        private bool IsValue = false;
        public Boolean IsChecked {
            get { return IsValue; }
            set {
                if(IsValue != value) {
                    IsValue = value;
                    PlayAnimation();

                    ValueChanged?.Invoke(this, null);
                }
            }
        }

        public event RoutedEventHandler ValueChanged;
        public virtual void Value_Changed(object sender, RoutedEventArgs args) {
            ValueChanged?.Invoke(this, args);
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
    }
}
