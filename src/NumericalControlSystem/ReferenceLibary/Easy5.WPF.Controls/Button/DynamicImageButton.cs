using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace Easy5.WPF.Controls
{
    public class DynamicImageButton : ButtonBase
    {
        static DynamicImageButton()
        {
            /*WPF采用了在子类控件的静态构造方法中重写DefaultStyleKey元数据的方式来指定该子类控件的默认样式.
             *代码中,我们将
             *        new FrameworkPropertyMetadata(typeof(DynamicImageButton))
             * 指定为其新的元数据值,这个值代表着,
             * 我们将在资源字典中查找一个键值为typeof(DynamicImageButton)的Style来做为控件的默认样式
             */
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicImageButton), new FrameworkPropertyMetadata(typeof(DynamicImageButton)));
        }

        /// <summary>
        /// 使用注意: 
        ///  <easy5Controls:DynamicImageButton IconImageUri="/Resource/Image/wizard.png" Content="123"/>
        /// 其中
        ///   IconImageUri="/Resource/Image/wizard.png" Content="123"
        /// 不能为：
        ///   IconImageUri="./Resource/Image/wizard.png" Content="123"
        /// 奇怪的问题。
        /// </summary>
        public string IconImageUri
        {
            get { return (string)GetValue(IconImageUriProperty); }
            set
            {
                SetValue(IconImageUriProperty, value);
            }
        }
        public static readonly DependencyProperty IconImageUriProperty =
            DependencyProperty.Register("IconImageUri", typeof(string), typeof(DynamicImageButton), new UIPropertyMetadata(string.Empty,
              (o, e) =>
              {
                  try
                  {
                      Uri uriSource = new Uri((string)e.NewValue, UriKind.RelativeOrAbsolute);
                      if (uriSource != null)
                      {
                          DynamicImageButton button = o as DynamicImageButton;
                          BitmapImage img = new BitmapImage(uriSource);
                          button.SetValue(IconImageProperty, img);
                      }
                  }
                  catch (Exception ex)
                  {
                      throw ex;
                  }
              }));

        public BitmapImage IconImage
        {
            get { return (BitmapImage)GetValue(IconImageProperty); }
            set { SetValue(IconImageProperty, value); }
        }
        public static readonly DependencyProperty IconImageProperty =
            DependencyProperty.Register("IconImage", typeof(BitmapImage), typeof(DynamicImageButton), new UIPropertyMetadata(null));
    }
}
