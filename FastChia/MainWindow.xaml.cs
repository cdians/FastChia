using FastChia.common;
using FastChia.model;
using System;
using System.Windows;

namespace FastChia
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainViewModel = MainViewModel.GetModel(this);
            this.DataContext = mainViewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确定要退出程序？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
