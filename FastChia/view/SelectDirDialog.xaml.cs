using FastChia.common;
using FastChia.config;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace FastChia.view
{
    /// <summary>
    /// SelectDirDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectDirDialog : Window
    {

        JArray array = new JArray();

        public SelectDirDialog(double x, double y)
        {
            InitializeComponent();
            //启用‘Manual’属性后，可以手动设置窗体的显示位置
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Top = x;
            this.Left = y;
            init();
        }

        private void init()
        {
            string dirs = INIHelper.Read(BaseConfig.INFO_NODE, "tempList", "[]", BaseConfig.INFO_PATH);
            array = JArray.Parse(dirs);
        }

        private void select(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                this.Topmost = true;
                var dir = dialog.FileName;
                if (null!= dir)
                {
                    if (array.Count < 1)
                    {
                        selectDir.Text = dir;
                    } else
                    {
                        foreach (JObject a in array)
                        {
                            if (a.Value<string>("path") != dir)
                            {
                                selectDir.Text = dir;
                            }
                        }
                    }
                }
            } else
            {
                this.Topmost = true;
            }
        }

        private void confirm(object sender, RoutedEventArgs e)
        {
            string confirmDir = selectDir.Text;
            int count = Convert.ToInt32(dirCount.Text);
            JObject json = new JObject();
            json["path"] = confirmDir;
            json["count"] = count;
            array.Add(json);
            INIHelper.Write(BaseConfig.INFO_NODE, "tempList", Regex.Replace(array.ToString(), @"\s",""), BaseConfig.INFO_PATH);
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
