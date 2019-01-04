using ColorPicker2.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class MainWindow : Window {

        #region Structs

        struct ColorHls {
            public double H;
            public double L;
            public double S;
            public double A;
        }

        public struct HsbColor {
            // The Hue of the color [0,360]
            public double H;

            // The Saturation of the color [0,1]
            public double S;

            // The Brightness of the color [0,1]
            public double B;

            // The Alpha (opaqueness) of the color [0,1]
            public double A;
        }

        #endregion

        #region Conversions
        static ColorHls RgbToHls(System.Windows.Media.Color rgbColor) {
            // Initialize result
            var hlsColor = new ColorHls();

            // Convert RGB values to percentages
            double r = (double)rgbColor.R / 255;
            var g = (double)rgbColor.G / 255;
            var b = (double)rgbColor.B / 255;
            var a = (double)rgbColor.A / 255;

            // Find min and max RGB values
            var min = Math.Min(r, Math.Min(g, b));
            var max = Math.Max(r, Math.Max(g, b));
            var delta = max - min;

            /* If max and min are equal, that means we are dealing with 
             * a shade of gray. So we set H and S to zero, and L to either
             * max or min (it doesn't matter which), and  then we exit. */

            //Special case: Gray
            if (max == min) {
                hlsColor.H = 0;
                hlsColor.S = 0;
                hlsColor.L = max;
                return hlsColor;
            }

            /* If we get to this point, we know we don't have a shade of gray. */

            // Set L
            hlsColor.L = (min + max) / 2;

            // Set S
            if (hlsColor.L < 0.5) {
                hlsColor.S = delta / (max + min);
            } else {
                hlsColor.S = delta / (2.0 - max - min);
            }

            // Set H
            if (r == max) hlsColor.H = (g - b) / delta;
            if (g == max) hlsColor.H = 2.0 + (b - r) / delta;
            if (b == max) hlsColor.H = 4.0 + (r - g) / delta;
            hlsColor.H *= 60;
            if (hlsColor.H < 0) hlsColor.H += 360;

            // Set A
            hlsColor.A = a;

            // Set return value
            return hlsColor;

        }

        private static HsbColor RgbToHsb(System.Windows.Media.Color rgbColor) {
            /* Hue values range between 0 and 360. All 
             * other values range between 0 and 1. */

            // Create HSB color object
            var hsbColor = new HsbColor();

            // Get RGB color component values
            var r = Convert.ToInt32(rgbColor.R);
            var g = Convert.ToInt32(rgbColor.G);
            var b = Convert.ToInt32(rgbColor.B);
            var a = Convert.ToInt32(rgbColor.A);

            // Get min, max, and delta values
            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;

            /* Black (max = 0) is a special case. We 
             * simply set HSB values to zero and exit. */

            // Black: Set HSB and return
            if (max == 0.0) {
                hsbColor.H = 0.0;
                hsbColor.S = 0.0;
                hsbColor.B = 0.0;
                hsbColor.A = a;
                return hsbColor;
            }

            /* Now we process the normal case. */

            // Set HSB Alpha value
            var alpha = Convert.ToDouble(a);
            hsbColor.A = alpha / 255;

            // Set HSB Hue value
            if (r == max) hsbColor.H = (g - b) / delta;
            else if (g == max) hsbColor.H = 2 + (b - r) / delta;
            else if (b == max) hsbColor.H = 4 + (r - g) / delta;
            hsbColor.H *= 60;
            if (hsbColor.H < 0.0) hsbColor.H += 360;

            // Set other HSB values
            hsbColor.S = delta / max;
            hsbColor.B = max / 255;

            // Set return value
            return hsbColor;
        }

        private static System.Windows.Media.Color HsbToRgb(HsbColor hsbColor) {
            // Initialize
            var rgbColor = new System.Windows.Media.Color();

            /* Gray (zero saturation) is a special case.We simply
             * set RGB values to HSB Brightness value and exit. */

            // Gray: Set RGB and return
            if (hsbColor.S == 0.0) {
                rgbColor.A = Convert.ToByte(hsbColor.A * 255);
                rgbColor.R = Convert.ToByte(hsbColor.B * 255);
                rgbColor.G = Convert.ToByte(hsbColor.B * 255);
                rgbColor.B = Convert.ToByte(hsbColor.B * 255);
                return rgbColor;
            }

            /* Now we process the normal case. */

            var h = (hsbColor.H == 360) ? 0 : hsbColor.H / 60;
            var i = Convert.ToInt32(Math.Truncate(h));
            var f = h - i;

            var p = hsbColor.B * (1.0 - hsbColor.S);
            var q = hsbColor.B * (1.0 - (hsbColor.S * f));
            var t = hsbColor.B * (1.0 - (hsbColor.S * (1.0 - f)));

            double r, g, b;
            switch (i) {
                case 0:
                    r = hsbColor.B;
                    g = t;
                    b = p;
                    break;

                case 1:
                    r = q;
                    g = hsbColor.B;
                    b = p;
                    break;

                case 2:
                    r = p;
                    g = hsbColor.B;
                    b = t;
                    break;

                case 3:
                    r = p;
                    g = q;
                    b = hsbColor.B;
                    break;

                case 4:
                    r = t;
                    g = p;
                    b = hsbColor.B;
                    break;

                default:
                    r = hsbColor.B;
                    g = p;
                    b = q;
                    break;
            }

            // Set WPF Color object
            rgbColor.A = Convert.ToByte(hsbColor.A * 255);
            rgbColor.R = Convert.ToByte(r * 255);
            rgbColor.G = Convert.ToByte(g * 255);
            rgbColor.B = Convert.ToByte(b * 255);

            // Set return value
            return rgbColor;
        }

        private double RgbToL(int R, int G, int B)
        {
            return (Math.Sqrt((0.299 * Math.Pow(R, 2)) + (0.587 * Math.Pow(G, 2)) + (0.114 * Math.Pow(B, 2)))) / 255;
        }
        #endregion

        public MainWindow() {
            InitializeComponent();
        }

        public void Startup() {
            OperatingSystem os = Environment.OSVersion;
            Version v = os.Version;

            if (!(v.Major + v.Minor >= 8)) {
                Console.WriteLine("Windows8 미만의 환경에서는 지원하지 않습니다.");
            }

            pXKey = Settings.Default.pXKey;
            if (!pXKey) {
                PickMessageRect.Visibility = Visibility.Visible;
            }

            // SetColor(248, 169, 175);
            // SetColor(10, 167, 146);
            // SetColor(255, 231, 86);
            // SetColor(248, 169, 175);
            // SetColor(72, 61, 139);
            // SetColor(0x6C, 0xC2, 0xBD);
            // SetColor(0x5A, 0x80, 0x9E);
            // SetColor(0x7C, 0x79, 0xA2);
            // SetColor(0xF5, 0x7D, 0x7C);
            // SetColor(0x75, 0x7D, 0x7C);
            // SetColor(0xFF, 0xC1, 0xA6);

        }

        private void Main_Loaded(object sender, RoutedEventArgs e) {
            Startup();
        }

        private void DragRect_MouseDown(object sender, MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);
            
            this.DragMove();
        }

        double prevL = 0;
        private void SetColor(byte _R, byte _G, byte _B) {
            Settings.Default.SaveR = _R;
            Settings.Default.SaveG = _G;
            Settings.Default.SaveB = _B;
            Settings.Default.Save();

            byte[] hex = { _R, _G, _B };

            BackRect.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(_R, _G, _B));
            CopyShadow.Fill = BackRect.Fill;
            CloseRect_Back.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(38, 255, 255, 255));
            ColorCode.Text = Insertspace(BitConverter.ToString(hex).Replace("-", string.Empty));
            

            var hsbColor = RgbToHsb(System.Windows.Media.Color.FromRgb(_R, _G, _B));

            double L = RgbToL(_R, _G, _B);

            if (L >= 0.80) {
                var TextColor = hsbColor;
                TextColor.B = TextColor.B - 0.3;
                var TextRGBColor = HsbToRgb(TextColor);
                ColorCode.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(TextRGBColor.R, TextRGBColor.G, TextRGBColor.B));
                CopyShadow.Fill = ColorCode.Foreground;
                CloseRect_Back.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(38, 0, 0, 0));

            } else ColorCode.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

            double colorL;

            colorL = L;

            SetImgColor(prevL, colorL);

            prevL = colorL;
        }

        private string Insertspace(string str) {
            string newstr = "|";
            for (int i = 0; i < str.Length; i++) {
                newstr += str.Substring(i, 1);
                newstr += "|";
            }

            return newstr;
        }

        private void SetImgColor(double _prevL, double _color)
        {
            if (_color <= 0.80)
            {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("BlackToWhite_CloseRect");
                BeginStoryboard(storyBoard);
            }

            if (_color > 0.80)
            {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("WhiteToBlack_CloseRect");
                BeginStoryboard(storyBoard);
            }

            if (_color < 0.70)
            {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("BlackToWhite_TAG");
                BeginStoryboard(storyBoard);
            }

            if (_color >= 0.70)
            {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("WhiteToBlack_TAG");
                BeginStoryboard(storyBoard);
            }

            if (_color < 0.60) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("BlackToWhite_OptionRect");
                BeginStoryboard(storyBoard);
            }

            if (_color >= 0.60) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("WhiteToBlack_OptionRect");
                BeginStoryboard(storyBoard);
            }
        }

        private void Copy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Clipboard.SetText("#" + ColorCode.Text.Replace("|",""));
        }

        private void Close_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        bool pXKey;
        private void Main_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.C) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("Copied");
                BeginStoryboard(storyBoard);
                Copy_MouseLeftButtonDown(sender, null);
            }

            if (e.Key == Key.X) {
                System.Drawing.Color c = GetColorOnScreen();

                SetColor(c.R, c.G, c.B);
            }

                // if(e.Key == Key.A)
                //     SetColor(0x5A, 0x80, 0x9E);
                // 
                // if (e.Key == Key.S)
                //     SetColor(0x7C, 0x79, 0xA2);
                // 
                // if (e.Key == Key.D)
                //     SetColor(0xF5, 0x7D, 0x7C);
                // 
                // if (e.Key == Key.F)
                //     SetColor(0xFF, 0xC1, 0xA6);
                // 
                // if (e.Key == Key.G)
                //     SetColor(0x6C, 0xC2, 0xBD);
            }

        private void InitializeColorRect_Loaded(object sender, RoutedEventArgs e) {
            byte _r, _g, _b;
            _r = Settings.Default.SaveR;
            _g = Settings.Default.SaveG;
            _b = Settings.Default.SaveB;
            
            SetColor(_r, _g, _b);
        }

        private void PickMessageRect_MouseDown(object sender, MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            this.DragMove();
        }

        private void Main_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.X) {
                if (!pXKey) {
                    Settings.Default.pXKey = true;
                    Settings.Default.Save();

                    pXKey = true;

                    System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("PressXKey");
                    BeginStoryboard(storyBoard);
                }
            }
        }

        private System.Drawing.Color GetColorOnScreen() {
            System.Drawing.Color c = GetPixel(GetCursorPosition());

            return c;
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out System.Drawing.Point lpPoint);
        public static System.Drawing.Point GetCursorPosition() {
            System.Drawing.Point lpPoint;
            GetCursorPos(out lpPoint);

            return lpPoint;
        }

        private System.Drawing.Color GetPixel(System.Drawing.Point _position) {
            Console.WriteLine("X : " + _position.X + ", Y : " + _position.Y);
            System.Drawing.Point position = new System.Drawing.Point(Convert.ToInt32(_position.X), Convert.ToInt32(_position.Y));
            using (var bitmap = new Bitmap(1, 1)) {
                using (var graphics = Graphics.FromImage(bitmap)) {
                    graphics.CopyFromScreen(position, new System.Drawing.Point(0, 0), new System.Drawing.Size(1, 1));
                }
                return bitmap.GetPixel(0, 0);
            }
        }
    }

}