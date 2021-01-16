using System.Windows;
using System.Windows.Controls;

namespace Easy5.WPF.DazzleUI.Controls
{
	public class DazzleTabControl : TabControl
	{
        static DazzleTabControl()
		{
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DazzleTabControl), new FrameworkPropertyMetadata(typeof(DazzleTabControl)));
		}
	}
}
