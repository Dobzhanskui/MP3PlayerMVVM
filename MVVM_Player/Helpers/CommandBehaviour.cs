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

        public static void SetDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickCommandProperty, value);
        }

        public static void SetDoubleClickCommandParameter(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickCommandParameterProperty, value);
        }

        public static ICommand GetDoubleClickCommand(DependencyObject obj)
           => (ICommand)obj.GetValue(DoubleClickCommandProperty);

        public static object GetDoubleClickCommandParameter(DependencyObject obj)
           => obj.GetValue(DoubleClickCommandParameterProperty);


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

        #endregion // Events
    }
}
