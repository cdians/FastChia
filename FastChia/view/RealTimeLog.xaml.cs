using FastChia.common;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FastChia.view
{
    /// <summary>
    /// RealTimeLog.xaml 的交互逻辑
    /// </summary>
    public partial class RealTimeLog : Window
    {

        private string fileName = "";

        private string filePath = "";

        public RealTimeLog(string fileName)
        {
            InitializeComponent();
            this.fileName = fileName;
            this.Title = "实时日志    " + fileName;
            this.Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(loadFile);
        }

        private void loadFile()
        {
            var t = Task.Run(() =>
            {
                filePath = AppDomain.CurrentDomain.BaseDirectory + "log\\" + fileName + ".log";
                FileStream fs;
                if (File.Exists(filePath))
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        // 从文件读取并显示行，直到文件的末尾 
                        while (!sr.EndOfStream)
                        {
                            string line;
                            line = sr.ReadLine();
                            if (null != line && !"".Equals(line))
                            {
                                Action action = delegate
                                {
                                    LogList.Items.Add(line);
                                    LogList.ScrollIntoView(line);
                                };
                                LogList.Dispatcher.BeginInvoke(DispatcherPriority.Background,action);
                            }
                        }
                        LogHelper.ShowLog("文件读取完成");
                    }
                }
            });
            t.GetAwaiter().OnCompleted(() =>
            {
                LogHelper.ShowLog("开始监控写入");
                CommandWatcher watcher = new CommandWatcher(filePath);
                watcher.CommandHandler += newLine;
                watcher.Start();
            });
        }

        private void Loads_Completed(object sender, EventArgs e)
        {
            CommandWatcher watcher = new CommandWatcher(filePath);
            watcher.CommandHandler += newLine;
            watcher.Start();
        }

        private string lastStr = String.Empty;

        private void newLine(object o, CommandEventArgs e)
        {
            if (null != e.line)
            {
                string str = String.Empty;
                str = e.line.Replace("\n", "").Replace("\r", "");
                lastStr = str;
                Action action1 = delegate
                {
                    LogList.Items.Add(str);
                    LogList.ScrollIntoView(str);
                };
                LogList.Dispatcher.BeginInvoke(DispatcherPriority.Background,action1);
            }
        }
    }
}
