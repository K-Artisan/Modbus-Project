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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Azir.Modbus.Client.WPF.UserControls;
using Easy5.WPF.DazzleUI.Controls;

namespace Azir.Modbus.Client.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NCSMainWindow : DazzleWindow
    {
        public NCSMainWindow()
        {
            InitializeComponent();
            InitializeMenu();
        }
     
        #region 窗口顶部按钮
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
        #endregion

        #region 菜单

        /// <summary>
        /// 初始化菜单
        /// </summary>
        private void InitializeMenu()
        {
            ContextMenu mainMenu = new ContextMenu();
            mainMenu.FontSize = 12;

            this.BtnMianMenu.ContextMenu = mainMenu;

            MenuItem aboutUsItem = new MenuItem();
            aboutUsItem.Header = "关于我们";
            //aboutUsItem.Icon;
            aboutUsItem.Click += new RoutedEventHandler(AboutUsItem_Click);
            mainMenu.Items.Add(aboutUsItem);
        }

        private void AboutUsItem_Click(object sender, RoutedEventArgs e)
        {
            NCSAbout aboutBox = new NCSAbout();
            aboutBox.ShowDialog();
        }
        #endregion
    }
}
