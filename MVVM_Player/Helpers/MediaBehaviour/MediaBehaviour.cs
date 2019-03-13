using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MVVM_Player.Helpers.MediaBehaviour
{
    public class MediaBehaviour
    {
        #region Members

        private static DispatcherTimer m_timerTrack;

        #endregion // Members

        #region DependencyProperty

        public static readonly DependencyProperty TimerTrackCommandProperty =
        DependencyProperty.RegisterAttached("TimerTrackCommand", typeof(ICommand), typeof(MediaBehaviour),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(TimerTrackCommandChanged)));

        public static readonly DependencyProperty MediaEndedCommandProperty =
        DependencyProperty.RegisterAttached("MediaEndedCommand", typeof(ICommand), typeof(MediaBehaviour),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(MediaEndedCommandChanged)));

        public static readonly DependencyProperty MediaOpenedCommandProperty =
        DependencyProperty.RegisterAttached("MediaOpenedCommand", typeof(ICommand), typeof(MediaBehaviour),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(MediaOpenedCommandChanged)));

        public static readonly DependencyProperty MediaEndedCommandParameterProperty =
          DependencyProperty.RegisterAttached("MediaEndedCommandParameter", typeof(object), typeof(MediaBehaviour),
          new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty SliderValueChangedCommandProperty =
         DependencyProperty.RegisterAttached("SliderValueChangedCommand", typeof(ICommand), typeof(MediaBehaviour),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(SliderValueChangedCommandChanged)));

        public static readonly DependencyProperty ButtonMoveRightCommandProperty =
         DependencyProperty.RegisterAttached("ButtonMoveRightCommand", typeof(ICommand), typeof(MediaBehaviour),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(ButtonMoveRightCommandChanged)));

        public static readonly DependencyProperty ButtonMoveLeftCommandProperty =
        DependencyProperty.RegisterAttached("ButtonMoveLeftCommand", typeof(ICommand), typeof(MediaBehaviour),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(ButtonMoveLeftCommandChanged)));

        #endregion // DependencyProperty

        #region Commands

        public static void TimerTrackCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is MediaElement mediaElement)
            {
                m_timerTrack = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(0.3),
                    Tag = mediaElement
                };
                m_timerTrack.Tick += timerTrack_Tick;
            }
        }

        public static void MediaEndedCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is MediaElement mediaElement)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    mediaElement.MediaEnded += mediaElement_MediaEnded;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    mediaElement.MediaEnded -= mediaElement_MediaEnded;
                }
            }
        }

        public static void MediaOpenedCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is MediaElement mediaElement)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    mediaElement.MediaOpened += mediaElement_MediaOpened;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    mediaElement.MediaOpened -= mediaElement_MediaOpened;
                }
            }
        }

        public static void SliderValueChangedCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is Slider slider)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    slider.ValueChanged += slider_ValueChanged;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    slider.ValueChanged -= slider_ValueChanged;
                }
            }
        }

        public static void ButtonMoveRightCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is Button button)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    button.Click += Button_MoveRightClick;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    button.Click -= Button_MoveRightClick;
                }
            }
        }

        public static void ButtonMoveLeftCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is Button button)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    button.Click += Button_MoveLeftClick;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    button.Click -= Button_MoveLeftClick;
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

        public static void SetMediaOpenedCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(MediaOpenedCommandProperty, value);
        }

        public static void SetMediaEndedCommandParameter(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MediaEndedCommandParameterProperty, value);
        }

        public static void SetSliderValueChangedCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(SliderValueChangedCommandProperty, value);
        }

        public static void SetButtonMoveRightCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(ButtonMoveRightCommandProperty, value);
        }

        public static void SetButtonMoveLeftCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(ButtonMoveLeftCommandProperty, value);
        }

        public static ICommand GetMediaEndedCommand(UIElement element)
           => (ICommand)element.GetValue(MediaEndedCommandProperty);

        public static ICommand GetTimerTrackCommand(UIElement element)
            => (ICommand)element.GetValue(TimerTrackCommandProperty);

        public static ICommand GetMediaOpenedCommand(UIElement element)
           => (ICommand)element.GetValue(MediaOpenedCommandProperty);

        public static object GetMediaEndedCommandParameter(DependencyObject obj)
          => obj.GetValue(MediaEndedCommandParameterProperty);

        public static ICommand GetSliderValueChangedCommand(UIElement element)
           => (ICommand)element.GetValue(SliderValueChangedCommandProperty);

        public static ICommand GetButtonMoveRightCommand(UIElement element)
           => (ICommand)element.GetValue(ButtonMoveRightCommandProperty);

        public static ICommand GetButtonMoveLeftCommand(UIElement element)
           => (ICommand)element.GetValue(ButtonMoveLeftCommandProperty);

        #endregion // Commands

        #region Events

        private static void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (sender is MediaElement mediaElement)
            {
                var command = GetMediaEndedCommand(mediaElement);
                var parameter = GetMediaEndedCommandParameter(mediaElement);
                if (command != null && command.CanExecute(parameter))
                {
                    e.Handled = true;
                    command.Execute(parameter);
                    m_timerTrack.Stop();
                }
            }
        }

        private static void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (sender is MediaElement mediaElement)
            {
                var command = GetMediaOpenedCommand(mediaElement);
                if (command != null)
                {
                    e.Handled = true;
                    command.Execute(mediaElement.NaturalDuration);
                    m_timerTrack.Start();
                }
            }
        }

        private static void timerTrack_Tick(object sender, EventArgs e)
        {
            if (sender is DispatcherTimer dispatcherTimer && dispatcherTimer.Tag is MediaElement mediaElement)
            {
                var command = GetTimerTrackCommand(mediaElement);
                if (command != null)
                {
                    command.Execute(mediaElement.Position);
                }
            }
        }

        private static void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_timerTrack.Tag is MediaElement mediaElement)
            {
                mediaElement.Position = TimeSpan.FromSeconds(e.NewValue);
            }
        }

        private static void Button_MoveRightClick(object sender, RoutedEventArgs e)
        {
            if (m_timerTrack.Tag is MediaElement mediaElement)
            {
                mediaElement.Position = TimeSpan.FromSeconds(mediaElement.Position.TotalSeconds + 15);
            }
        }

        private static void Button_MoveLeftClick(object sender, RoutedEventArgs e)
        {
            if (m_timerTrack.Tag is MediaElement mediaElement)
            {
                mediaElement.Position = TimeSpan.FromSeconds(mediaElement.Position.TotalSeconds - 15);
            }
        }

        #endregion // Events
    }
}
