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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPicker2 {
    /// <summary>
    /// IconButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IconButton : UserControl {
        public IconButton() {
            InitializeComponent();
        }
        
        public object Icon {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(IconButton), new PropertyMetadata(0));
        
        private void IconButtonControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Point mPosition = e.GetPosition(this);

            RadialGradientBrush overlayBrush = new RadialGradientBrush();
            overlayBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 22, 22, 22), 0.5));
            overlayBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 22, 22, 22), 0.5));
            overlayBrush.GradientOrigin = new Point((mPosition.X / 50 * 2) - 0.5, (mPosition.Y / 50 * 2) - 0.5);
            overlayBrush.RadiusX = 0;
            overlayBrush.RadiusY = 0;

            MouseOverlay.Fill = overlayBrush;

            System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("MouseClick");
            BeginStoryboard(storyBoard);
        }
    }
}
