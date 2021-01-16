using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace NCS.Client.WPF.UserControls
{
    /// <summary>
    /// NCSAbout.xaml 的交互逻辑
    /// </summary>
    public partial class NCSAbout : Window
    {
        public NCSAbout()
        {
            InitializeComponent();
        }

        #region CloseRequested

        public event RoutedEventHandler CloseRequested;

        private void RaiseCloseRequested()
        {
            if (CloseRequested != null)
            {
                CloseRequested(this, new RoutedEventArgs());
            }
        }

        #endregion

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RaiseCloseRequested();
        }

        private void SelaButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("http://www.cnblogs.com/easy5weikai/"));
            RaiseCloseRequested();
        }

        private void PixelLabButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("http://user.qzone.qq.com/277898160/"));
            RaiseCloseRequested();
        }
    }
}
