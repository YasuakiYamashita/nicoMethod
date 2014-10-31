using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace nicoViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, Brush> fontColorMap = new Dictionary<string, Brush>();
        Dictionary<string, double> fontSizeMap = new Dictionary<string, double>();
        private Canvas canvas;

        // 受信ポート番号
        private int localPort = 5678;

        // 表示速度
        private int moveCommentTime = 5000;

        public MainWindow(int port, int comTime)
        {
            InitializeComponent();
            // 透明で常に前面に設定
            this.AllowsTransparency = true;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Top = 0;
            this.Left = 0;
            this.WindowState = WindowState.Maximized;
            this.Background = Brushes.Transparent;
            this.WindowStyle = WindowStyle.None;
            this.Topmost = true;
            this.canvas = new Canvas();
            this.Content = this.canvas;

            // 変数に入れておく
            localPort = port;
            moveCommentTime = comTime;
            
            fontColorMap.Add("blue", Brushes.Blue);
            fontColorMap.Add("red", Brushes.Red);
            fontColorMap.Add("green", Brushes.Green);

            fontSizeMap.Add("big", 200.0);
            fontSizeMap.Add("small", 50.0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // UDPサーバ起動
            Thread thread = new Thread(new ThreadStart(UDPThread));
            thread.IsBackground = true;
            thread.Start();
        }

        private int verticalPosition = 0;
        public delegate void MoveMessageDelegate(string message);

        public void MoveMessage(string message)
        {
            if (!this.CheckAccess())
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new MoveMessageDelegate(this.MoveMessage), message);
            }
            else
            {
                // フォントサイズの決定
                double fontsize = 100;
                foreach (string sizeName in fontSizeMap.Keys)
                {
                    if (message.Contains(sizeName))
                    {
                        message = message.Remove(message.IndexOf(sizeName), sizeName.Length);
                        fontsize = fontSizeMap[sizeName];
                    }
                }
                // フォントカラーの決定
                Brush fontcolor = Brushes.White;
                foreach (string colorname in fontColorMap.Keys)
                {
                    if (message.Contains(colorname))
                    {
                        message = message.Remove(message.IndexOf(colorname), colorname.Length);
                        fontcolor = fontColorMap[colorname];
                    }
                }
                // 新しいテキストを生成
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = fontsize;
                textBlock.Text = message.Trim();
                textBlock.Foreground = fontcolor;
                this.canvas.Children.Add(textBlock);

                // テキストに影をつける
                DropShadowEffect effect = new DropShadowEffect();
                effect.ShadowDepth = 4;
                effect.Direction = 330;
                effect.Color = (Color)ColorConverter.ConvertFromString("black");
                textBlock.Effect = effect;

                // テキストの位置を指定
                verticalPosition += (int)fontsize;
                if (verticalPosition >= this.Height) verticalPosition = 0;
                TranslateTransform transform = new TranslateTransform(this.Width, verticalPosition);

                 // テキストのアニメーション
                textBlock.RenderTransform = transform;
                Duration duration = new Duration(TimeSpan.FromMilliseconds(moveCommentTime));
                DoubleAnimation animationX = new DoubleAnimation(-1 * message.Length * fontsize, duration);
                animationX.Completed += new EventHandler(animationX_Completed);
                animationX.Name = textBlock.Name;
                transform.BeginAnimation(TranslateTransform.XProperty, animationX);
            }
        }

        // 範囲外の時
        void animationX_Completed(object sender, EventArgs e)
        {
            AnimationClock clock = (AnimationClock)sender;
            lock (this.canvas.Children)
            {
                for (int i = 0; i < this.canvas.Children.Count; i++ )
                {
                    if (((TextBlock)this.canvas.Children[i]).Name.Equals(clock.Timeline.Name))
                    {
                        this.canvas.Children.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        // UDPを受信する
        private void UDPThread()
        {

            UdpClient client = null;
            try
            {
                client = new UdpClient(localPort);
            }
            catch
            {
                MessageBox.Show("起動に失敗しました");
                return;
            }

            // 送信元。任意のIPアドレス、任意のポートから許可
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                // 受信するまで待ち続ける
                byte[] res = client.Receive(ref remoteEP);

                // バイト配列から ASCII 文字列に変換して表示
                MoveMessage(Encoding.UTF8.GetString(res));
            }
        }
    }
}
