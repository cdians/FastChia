using System.Collections.Generic;
using System.Windows;

namespace FastChia.view
{
    /// <summary>
    /// ShowKeysDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ShowKeysDialog : Window
    {
        public ShowKeysDialog(double x, double y, Dictionary<string, string> info)
        {
            InitializeComponent();
            //启用‘Manual’属性后，可以手动设置窗体的显示位置
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Top = x;
            this.Left = y;
            fpk.Text = info["Farmerpublickey"];
            ppk.Text = info["Poolpublickey"];
            fingerprint.Text = info["Fingerprint"];
            address.Text = info["Firstwalletaddress"];
        }

        private void autoWrite(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
