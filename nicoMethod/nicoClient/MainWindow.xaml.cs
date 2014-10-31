using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace nicoServer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // 送信元とするポート
        private const int localPort = 1234;

        // UDP クライアント
        private UdpClient client = new UdpClient(localPort);

        public MainWindow()
        {
            InitializeComponent();
            comment.Text = "";

            // UDP パケットの送信先
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5678);

            client.Connect(remoteEP);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void comment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                SendMessage();
            }
        }

        // コメント送信
        private void SendMessage()
        {
            string[] colors = { "", "red ", "blue ", "green " };
            string[] sizes = { "", "big ", "small " };

            string text = colors[Color.SelectedIndex] + sizes[Size.SelectedIndex] + comment.Text;

            // 送信する内容(バイト配列で指定)
            byte[] msg = Encoding.UTF8.GetBytes(text);

            // 送信するc
            client.Send(msg, msg.Length);

            comment.Text = "";
        }
    }
}
