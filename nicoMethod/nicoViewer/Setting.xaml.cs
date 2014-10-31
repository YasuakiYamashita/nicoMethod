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
    /// Setting.xaml の相互作用ロジック
    /// </summary>
    public partial class Setting : Window
    {

        // コメント流れるウインドウ
        MainWindow mainWindow = null;

        public Setting()
        {
            InitializeComponent();

            Start.IsEnabled = true;
            Stop.IsEnabled = false;
        }

        // 起動ボタンが押された場合
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Start.IsEnabled = false;
            Stop.IsEnabled = true;

            if (mainWindow == null)
            {
                mainWindow = new MainWindow(int.Parse(port.Text), int.Parse(time.Text));
                //mainWindow.Owner = this;
                mainWindow.Show();
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Start.IsEnabled = true;
            Stop.IsEnabled = false;
            mainWindow.Close();
            mainWindow = null;
            this.Close();
        }


        private void NumOnly_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            // 数値以外、または数値の入力に関係しないキーが押された場合、イベントを処理済みに。
            if (!((Key.D0 <= e.Key && e.Key <= Key.D9) ||
                  (Key.NumPad0 <= e.Key && e.Key <= Key.NumPad9) ||
                  Key.Back == e.Key ||
                  Key.Delete == e.Key ||
                  Key.Tab == e.Key) ||
                (Keyboard.Modifiers & ModifierKeys.Shift) > 0)
            {
                e.Handled = true;
            }

            // もう、既に４文字押されている場合は押さない
            if ((Key.D0 <= e.Key && e.Key <= Key.D9) || (Key.NumPad0 <= e.Key && e.Key <= Key.NumPad9))
            {
                if (((TextBox)sender).Text.Length >= 4)
                    e.Handled = true;
            }

            base.OnKeyDown(e);
        }

        private void NumOnly_Changed(object sender, TextChangedEventArgs e)
        {
            for(int i=0; i<((TextBox)sender).Text.Length; ++i)
            {
                if(!(((TextBox)sender).Text[i] >= '0' && ((TextBox)sender).Text[i] <= '9'))
                {
                    ((TextBox)sender).Text = ((TextBox)sender).Text.Remove(i, 1);
                }
            }
        }
    }
}
