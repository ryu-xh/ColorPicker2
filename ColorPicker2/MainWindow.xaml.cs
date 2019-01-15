using ColorPicker2.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        }

        public struct HSLColor {
            private int _h;
            private float _s;
            private float _l;

            public HSLColor(int h, float s, float l) {
                this._h = h;
                this._s = s;
                this._l = l;
            }

            public int H {
                get { return this._h; }
                set { this._h = value; }
            }

            public float S {
                get { return this._s; }
                set { this._s = value; }
            }

            public float L {
                get { return this._l; }
                set { this._l = value; }
            }

            public bool Equals(HSLColor hsl) {
                return (this.H == hsl.H) && (this.S == hsl.S) && (this.L == hsl.L);
            }
        }

        #endregion

        #region Conversions
        private static System.Windows.Media.Color HsbToRgb(HsbColor hsbColor) {
            // Initialize
            var rgbColor = new System.Windows.Media.Color();

            /* Gray (zero saturation) is a special case.We simply
             * set RGB values to HSB Brightness value and exit. */

            // Gray: Set RGB and return
            if (hsbColor.S == 0.0) {
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
            rgbColor.R = Convert.ToByte(r * 255);
            rgbColor.G = Convert.ToByte(g * 255);
            rgbColor.B = Convert.ToByte(b * 255);

            // Set return value
            return rgbColor;
        }

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
            HsbColor hsbColor;
            double delta, min;
            double h = 0, s, v;

            min = Math.Min(Math.Min(rgbColor.R, rgbColor.G), rgbColor.B);
            v = Math.Max(Math.Max(rgbColor.R, rgbColor.G), rgbColor.B);
            delta = v - min;

            if (v == 0.0)
                s = 0;
            else
                s = delta / v;

            if (s == 0)
                h = 0.0;

            else {
                if (rgbColor.R == v)
                    h = (rgbColor.G - rgbColor.B) / delta;
                else if (rgbColor.G == v)
                    h = 2 + (rgbColor.B - rgbColor.R) / delta;
                else if (rgbColor.B == v)
                    h = 4 + (rgbColor.R - rgbColor.G) / delta;

                h *= 60;

                if (h < 0.0)
                    h = h + 360;
            }

            hsbColor.H = h;
            hsbColor.S = s;
            hsbColor.B = v / 255;

            return hsbColor;
        }

        private double RgbToL(int R, int G, int B) {
            return (Math.Sqrt((0.299 * Math.Pow(R, 2)) + (0.587 * Math.Pow(G, 2)) + (0.114 * Math.Pow(B, 2)))) / 255;
        }

        private static HSLColor RgbToHSL(System.Windows.Media.Color rgbColor) {
            HSLColor hsl = new HSLColor();

            float r = (rgbColor.R / 255.0f);
            float g = (rgbColor.G / 255.0f);
            float b = (rgbColor.B / 255.0f);

            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);
            float delta = max - min;

            hsl.L = (max + min) / 2;

            if (delta == 0) {
                hsl.H = 0;
                hsl.S = 0.0f;
            } else {
                hsl.S = (hsl.L <= 0.5) ? (delta / (max + min)) : (delta / (2 - max - min));

                float hue;

                if (r == max) {
                    hue = ((g - b) / 6) / delta;
                } else if (g == max) {
                    hue = (1.0f / 3) + ((b - r) / 6) / delta;
                } else {
                    hue = (2.0f / 3) + ((r - g) / 6) / delta;
                }

                if (hue < 0)
                    hue += 1;
                if (hue > 1)
                    hue -= 1;

                hsl.H = (int)(hue * 360);
            }

            return hsl;
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
                HelpButton_MouseLeftButtonDown(this, null);
            }

            System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("CopyButtonInvisible");
            BeginStoryboard(storyBoard);

            storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("CopyButtonVisible");
            BeginStoryboard(storyBoard);

            SettingApply();
            
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

        private void DragRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            this.DragMove();
        }

        double prevL = 0;
        private void SetColor(byte _R, byte _G, byte _B) {
            Settings.Default.SaveR = _R;
            Settings.Default.SaveG = _G;
            Settings.Default.SaveB = _B;
            Settings.Default.Save();

            BackRect.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(_R, _G, _B));
            CopyShadow.Fill = BackRect.Fill;
            CloseRect_Back.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(38, 255, 255, 255));
            OptionRect_Back.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(38, 255, 255, 255));


            var hsbColor = RgbToHsb(System.Windows.Media.Color.FromRgb(_R, _G, _B));
            var hslColor = RgbToHSL(System.Windows.Media.Color.FromRgb(_R, _G, _B));
            double L = RgbToL(_R, _G, _B);


            // 타입별로 색상코드 등록
            switch (formatType) {
                case 0:
                    byte[] hex = { _R, _G, _B };
                    ColorCode.Padding = new Thickness(0, 67, 0, 0);
                    ColorCode.FontSize = 40;
                    ColorCode.Text = InsertHashspace(BitConverter.ToString(hex).Replace("-", string.Empty));
                    break;
                case 1:
                    ColorCode.Padding = new Thickness(0, 72, 0, 0);
                    ColorCode.FontSize = 30;
                    ColorCode.Text = Insertspace(_R.ToString()) + ",| " + Insertspace(_G.ToString()) + ",| " + Insertspace(_B.ToString());
                    break;
                case 2:
                    ColorCode.Padding = new Thickness(0, 72, 0, 0);
                    ColorCode.FontSize = 30;
                    ColorCode.Text = Insertspace(((int)hsbColor.H).ToString()) + ",| " + Insertspace(((int)(hsbColor.S * 100)).ToString()) + ",| " + Insertspace(((int)(hsbColor.B * 100)).ToString());
                    break;
                case 3:
                    ColorCode.Padding = new Thickness(0, 72, 0, 0);
                    ColorCode.FontSize = 30;
                    ColorCode.Text = Insertspace(((int)hslColor.H).ToString()) + ",| " + Insertspace(((int)(hslColor.S * 100)).ToString())  + ",| " + Insertspace(((int)(hslColor.L * 100)).ToString());
                    break;
            }

            if (L >= 0.80) {
                var TextColor = hsbColor;
                TextColor.B = TextColor.B - 0.3;
                var TextRGBColor = HsbToRgb(TextColor);
                ColorCode.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(TextRGBColor.R, TextRGBColor.G, TextRGBColor.B));
                CopyShadow.Fill = ColorCode.Foreground;

            } else
                ColorCode.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

            // 닫기 버튼 배경 색상조절
            if (L >= 0.55) {
                CloseRect_Back.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(38, 0, 0, 0));
                OptionRect_Back.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(38, 0, 0, 0));
            }

            double colorL;
            colorL = L;

            // 태그, 색상, 옵션 색상조절
            SetImgColor(prevL, colorL);
            prevL = colorL;

            // 자동카피
            if (isAutoCopy) {
                CopyCode();
            }
        }

        private string Insertspace(string str) {
            string newstr = "  ";
            for (int i = 0; i < str.Length; i++) {
                newstr += str.Substring(i, 1);
                newstr += "  ";
            }

            return newstr;
        }

        private string InsertHashspace(string str) {
            string newstr = "| ";
            for (int i = 0; i < str.Length; i++) {
                newstr += str.Substring(i, 1);
                newstr += "| ";
            }

            return newstr;
        }

        private void SetImgColor(double _prevL, double _color) {
            if (_color <= 0.80) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("BlackToWhite_CloseRect");
                BeginStoryboard(storyBoard);
            }

            if (_color > 0.80) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("WhiteToBlack_CloseRect");
                BeginStoryboard(storyBoard);
            }

            if (_color < 0.55) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("BlackToWhite_TAG");
                BeginStoryboard(storyBoard);
            }

            if (_color >= 0.55) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("WhiteToBlack_TAG");
                BeginStoryboard(storyBoard);
            }

            if (_color < 0.55) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("BlackToWhite_OptionRect");
                BeginStoryboard(storyBoard);
            }

            if (_color >= 0.55) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("WhiteToBlack_OptionRect");
                BeginStoryboard(storyBoard);
            }
        }

        private void CopyCode() {
            String code = ColorCode.Text.Replace("|", "");
            code = code.Replace("  ", "");

            if (formatType == 0) {
                code = code.Replace(" ", "");
                code = "#" + code;
            }
                
            Clipboard.SetText(code);
        }

        private void Copy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            CopyCode();
        }

        bool pXKey;
        System.Windows.Media.Color nColor;
        private void Main_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.C) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("Copied");
                BeginStoryboard(storyBoard);
                Copy_MouseLeftButtonDown(sender, null);
            }

            if (e.Key == Key.X) {
                System.Drawing.Color c = GetColorOnScreen();

                nColor.R = c.R;
                nColor.G = c.G;
                nColor.B = c.B;

                SetColor(c.R, c.G, c.B);
            }
        }

        private void InitializeColorRect_Loaded(object sender, RoutedEventArgs e) {
            ColorLoad();
        }

        private void ColorLoad() {
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
            GetCursorPos(out System.Drawing.Point lpPoint);

            return lpPoint;
        }

        private System.Drawing.Color GetPixel(System.Drawing.Point _position) {
            // Console.WriteLine("X : " + _position.X + ", Y : " + _position.Y);
            System.Drawing.Point position = new System.Drawing.Point(Convert.ToInt32(_position.X), Convert.ToInt32(_position.Y));
            using (var bitmap = new Bitmap(1, 1)) {
                using (var graphics = Graphics.FromImage(bitmap)) {
                    graphics.CopyFromScreen(position, new System.Drawing.Point(0, 0), new System.Drawing.Size(1, 1));
                }
                return bitmap.GetPixel(0, 0);
            }
        }

        bool isOption = false;
        private void OptionBtn_Click(object sender, MouseButtonEventArgs e) {
            if (isOption) {
                this.Height = 200;
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("Option_Close");
                BeginStoryboard(storyBoard);

            } else {
                this.Height = 300;
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("Option_Open");
                BeginStoryboard(storyBoard);

            }

            isOption = !isOption;
        }

        private void CloseBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            settingWindow.Close();
            helpWindow.Close();
            this.Close();
        }

        SettingWindow settingWindow = new SettingWindow();
        private void SettingButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            settingWindow.SettingApply += SettingApply;
            settingWindow.Show();
        }

        bool isAutoCopy;
        bool isHideCopyButton;
        byte formatType;
        private void SettingApply() {
            isAutoCopy = Properties.Settings.Default.AutoCopy;
            isHideCopyButton = Properties.Settings.Default.HideCopyButton;
            formatType = Properties.Settings.Default.Format;

            if (isHideCopyButton) {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("CopyButtonInvisible");
                BeginStoryboard(storyBoard);
            } else {
                System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("CopyButtonVisible");
                BeginStoryboard(storyBoard);
            }

            SetTagFormat();
        }

        HelpWindow helpWindow = new HelpWindow();
        private void HelpButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            helpWindow.KeyPress += new RoutedEventHandler(KeyPressEventFromOtherWindow);
            helpWindow.ActivatedWindow += new RoutedEventHandler(Main_Activated);
            helpWindow.DeactivatedWindow += new RoutedEventHandler(Main_Deactivated);
            helpWindow.Show();
        }

        private void KeyPressEventFromOtherWindow(object sender, RoutedEventArgs e) {
            KeyEventArgs key = e as KeyEventArgs;

            Main_KeyDown(this, key);
        }

        private void Main_Activated(object sender, EventArgs e) {
            System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("Activated");
            BeginStoryboard(storyBoard);
        }

        private void Main_Deactivated(object sender, EventArgs e) {
            System.Windows.Media.Animation.Storyboard storyBoard = (System.Windows.Media.Animation.Storyboard)FindResource("Deactivated");
            BeginStoryboard(storyBoard);
        }

        private void SetTagFormat() {
            String[] tagCode = { "Hash", "RGB", "HSV", "HSL" };

            Tag_White.Source = new BitmapImage(new Uri("pack://application:,,,/ColorPicker2;component/Res/Tag_" + tagCode[formatType] + "_White.png"));
            Tag_Black.Source = new BitmapImage(new Uri("pack://application:,,,/ColorPicker2;component/Res/Tag_" + tagCode[formatType] + "_Black.png"));

            ColorLoad();
        }

        private void InfoButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Process.Start("explorer.exe", @"https://ryuusei.io/ColorPicker2/Info.html?");
        }
    }
}