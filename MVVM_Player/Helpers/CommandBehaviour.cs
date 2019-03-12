using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MVVM_Player.Helpers
{
    public class CommandBehaviour
    {
        #region Members

        private static DispatcherTimer m_timerTrack;

        #endregion // Members

        #region DependencyProperty

        public static readonly DependencyProperty MediaEndedCommandProperty =
           DependencyProperty.RegisterAttached("MediaEndedCommand", typeof(ICommand), typeof(CommandBehaviour),
           new FrameworkPropertyMetadata(new PropertyChangedCallback(MediaEndedCommandChanged)));

        public static readonly DependencyProperty TimerTrackCommandProperty =
          DependencyProperty.RegisterAttached("TimerTrackCommand", typeof(ICommand), typeof(CommandBehaviour),
          new FrameworkPropertyMetadata(new PropertyChangedCallback(TimerTrackCommandChanged)));

        public static readonly DependencyProperty DoubleClickCommandProperty =
           DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(CommandBehaviour),
           new FrameworkPropertyMetadata(new PropertyChangedCallback(DoubleClickCommandChanged)));

        public static readonly DependencyProperty DoubleClickCommandParameterProperty =
          DependencyProperty.RegisterAttached("DoubleClickCommandParameter", typeof(object), typeof(CommandBehaviour),
          new FrameworkPropertyMetadata(null));

        #endregion // DependencyProperty

        #region Commands

        public static void MediaEndedCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is MediaElement mediaElement)
            {
                mediaElement.MediaEnded += mediaElement_MediaEnded;
            }
        }

        public static void TimerTrackCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            m_timerTrack = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            m_timerTrack.Tick += timerTrack_Tick;
            m_timerTrack.Start();
        }

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

        public static void SetMediaEndedCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(MediaEndedCommandProperty, value);
        }

        public static void SetTimerTrackCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(TimerTrackCommandProperty, value);
        }

        public static void SetDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickCommandProperty, value);
        }

        public static void SetDoubleClickCommandParameter(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickCommandParameterProperty, value);
        }

        public static ICommand GetMediaEndedCommand(UIElement element)
           => (ICommand)element.GetValue(MediaEndedCommandProperty);

        public static ICommand GetTimerTrackCommand(UIElement element)
           => (ICommand)element.GetValue(TimerTrackCommandProperty);

        public static ICommand GetDoubleClickCommand(DependencyObject obj)
           => (ICommand)obj.GetValue(DoubleClickCommandProperty);

        public static object GetDoubleClickCommandParameter(DependencyObject obj)
           => obj.GetValue(DoubleClickCommandParameterProperty);

        #endregion // Commands

        #region Events

        private static void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            
        }

        private static void timerTrack_Tick(object sender, EventArgs e)
        {
           
        }

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
