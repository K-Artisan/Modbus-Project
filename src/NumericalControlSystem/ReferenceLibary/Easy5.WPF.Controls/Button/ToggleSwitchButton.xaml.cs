using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Easy5.WPF.Controls
{
    /// <summary>
    /// ToggleSwitchButton.xaml 的交互逻辑
    /// </summary>
    public partial class ToggleSwitchButton : UserControl
    {
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof (bool), typeof (ToggleSwitchButton), new PropertyMetadata(default(bool), OnIsCheckedChanged));


        public event RoutedEventHandler Checked;
        public event RoutedEventHandler UnChecked;

        public bool IsChecked
        {
            get { return (bool) GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }   

        public ToggleSwitchButton()
        {
            InitializeComponent();
        }

        private static void OnIsCheckedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as ToggleSwitchButton).OnIsCheckedChanged(args);
        }

        private void OnIsCheckedChanged(DependencyPropertyChangedEventArgs args)
        {
            fillRectangle.Visibility = IsChecked ? Visibility.Visible : Visibility.Collapsed;
            slideBorder.HorizontalAlignment = IsChecked ? HorizontalAlignment.Right : HorizontalAlignment.Left;

            if (IsChecked && Checked != null)
            {
                Checked(this, new RoutedEventArgs());
            }

            if (!IsChecked && UnChecked != null)
            {
                UnChecked(this, new RoutedEventArgs());
            }
        }

        protected override void OnManipulationStarted(ManipulationStartedEventArgs args)
        {
            args.Handled = true;
            base.OnManipulationStarted(args);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs args)
        {
            Point point =  args.ManipulationOrigin;

            if (point.X > 0 && point.X < this.ActualWidth &&
                point.Y > 0 && point.Y< this.ActualHeight)
            {
                IsChecked ^= true;
                base.OnManipulationCompleted(args);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs args)
        {
            args.Handled = true;
            IsChecked ^= true;
            base.OnMouseLeftButtonUp(args);
        }

    }
}
