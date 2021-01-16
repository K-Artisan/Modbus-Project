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
using System.Windows.Shapes;
using Easy5.WPF.DazzleUI.Controls;
using NCS.Client.WPF.UserControls;

namespace NCS.Client.WPF
{
    /// <summary>
    /// NCSMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NCSMainWindow : DazzleWindow
    {
        public NCSMainWindow()
        {
            InitializeComponent();
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            ContextMenu mainMenu = new ContextMenu();
            mainMenu.FontSize = 12;

            this.BtnMianMenu.ContextMenu = mainMenu;

            MenuItem aboutUsItem = new MenuItem();
            aboutUsItem.Header = "关于我们";
            //aboutUsItem.Icon;
            aboutUsItem.Click += new RoutedEventHandler(AboutUsItem_Click);
;           mainMenu.Items.Add(aboutUsItem);

        }

        private void AboutUsItem_Click(object sender, RoutedEventArgs e)
        {
            NCSAbout aboutBox = new NCSAbout();
            aboutBox.ShowDialog();
        }
     

        #region About界面

        //private void AboutButton_Click(object sender, RoutedEventArgs e)
        //{
        //    AboutBox.Visibility = Visibility.Visible;
        //}

        //private void About_Click(object sender, RoutedEventArgs e)
        //{
        //    AboutBox.Visibility = Visibility.Visible;
        //}

        //private void About_CloseRequested(object sender, RoutedEventArgs e)
        //{
        //    AboutBox.Visibility = Visibility.Collapsed;
        //}

        #endregion

        private void BtnCloseApp_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnMaxWindows_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void BtnMinWindows_OnClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void BtnBtnMianMenu_OnClick(object sender, RoutedEventArgs e)
        {
             this.BtnMianMenu.ContextMenu.Visibility = Visibility.Visible;
        }

        private void BtnSkinWindows_OnClick(object sender, RoutedEventArgs e)
        {
            
        }



    }
}
