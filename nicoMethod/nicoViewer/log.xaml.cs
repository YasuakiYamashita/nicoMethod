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
using System.Windows.Shapes;

namespace nicoViewer
{
    /// <summary>
    /// log.xaml の相互作用ロジック
    /// </summary>
    public partial class log : Window
    {
        public log()
        {
            InitializeComponent();
        }


        public void add_log(string commnet)
        {
            DateTime dt = DateTime.Now;
            string time = dt.Hour + ":" + dt.Minute + ":" + dt.Second;

            // ログ追加
            logs.Dispatcher.BeginInvoke(new Action(() => {
                // ログ追加
                int id = logs.Items.Add(new string[] { time, commnet });

                // スクロールさせる
                logs.ScrollIntoView(logs.Items[logs.Items.Count - 1]);
            }));

            
        }
    }
}
