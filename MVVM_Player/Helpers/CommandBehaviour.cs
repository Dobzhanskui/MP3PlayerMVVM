using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MVVM_Player.Helpers
{
    public class CommandBehaviour
    {
        #region DependencyProperty

        public static readonly DependencyProperty DoubleClickCommandProperty =
           DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(CommandBehaviour),
           new FrameworkPropertyMetadata(new PropertyChangedCallback(DoubleClickCommandChanged)));

        public static readonly DependencyProperty PowerClickCommandProperty =
           DependencyProperty.RegisterAttached("PowerClickCommand", typeof(ICommand), typeof(CommandBehaviour),
           new FrameworkPropertyMetadata(new PropertyChangedCallback(PowerClickCommandChanged)));

        public static readonly DependencyProperty VisibilityClickCommandProperty =
           DependencyProperty.RegisterAttached("VisibilityClickCommand", typeof(ICommand), typeof(CommandBehaviour),
           new FrameworkPropertyMetadata(new PropertyChangedCallback(VisibilityClickCommandChanged)));

        public static readonly DependencyProperty DoubleClickCommandParameterProperty =
          DependencyProperty.RegisterAttached("DoubleClickCommandParameter", typeof(object), typeof(CommandBehaviour),
          new FrameworkPropertyMetadata(null));

        #endregion // DependencyProperty

        #region Commands

        public static void DoubleClickCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ListBox listBox)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    listBox.MouseDoubleClick += new MouseButtonEventHandler(Control_MouseDoubleClick);
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    listBox.MouseDoubleClick -= new MouseButtonEventHandler(Control_MouseDoubleClick);
                }
            }
        }

        public static void PowerClickCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is Button button)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    button.Click += Power_ButtonClick;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    button.Click -= Power_ButtonClick;
                }
            }
        }

        public static void VisibilityClickCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is Button button)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    button.Click += Visibility_ButtonClick;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    button.Click -= Visibility_ButtonClick;
                }
            }
        }

        public static void SetDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickCommandProperty, value);
        }

        public static void SetDoubleClickCommandParameter(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickCommandParameterProperty, value);
        }

        public static void SetPowerClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(PowerClickCommandProperty, value);
        }

        public static void SetVisibilityClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(VisibilityClickCommandProperty, value);
        }

        public static ICommand GetDoubleClickCommand(DependencyObject obj)
           => (ICommand)obj.GetValue(DoubleClickCommandProperty);

        public static object GetDoubleClickCommandParameter(DependencyObject obj)
           => obj.GetValue(DoubleClickCommandParameterProperty);

        public static ICommand GetPowerClickCommand(DependencyObject obj)
           => (ICommand)obj.GetValue(PowerClickCommandProperty);

        public static ICommand GetVisibilityClickCommand(DependencyObject obj)
           => (ICommand)obj.GetValue(VisibilityClickCommandProperty);


        #endregion // Commands

        #region Events

        private static void Control_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement element)
            {
                var command = GetDoubleClickCommand(element);
                var parameter = GetDoubleClickCommandParameter(element);
                if (command != null && command.CanExecute(parameter))
                {
                    e.Handled = true;
                    command.Execute(parameter);
                }
            }
        }

        private static void Power_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var command = GetPowerClickCommand(button);
                if (command != null)
                {
                    e.Handled = true;
                    command.Execute(button.ClickMode);
                }
            }
        }

        private static void Visibility_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var command = GetVisibilityClickCommand(button);
                if (command != null)
                {
                    e.Handled = true;
                    command.Execute(button.ClickMode);
                }
            }
        }

        #endregion // Events
    }
}
