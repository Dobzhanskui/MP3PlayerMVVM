using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace MVVM_Player.Helpers.MediaBehaviour
{
    public class MediaBehaviour : DependencyObject
    {
        #region Members

        private static DispatcherTimer m_timerTrack;
        private static DispatcherTimer m_timerMoveText;

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

        public static readonly DependencyProperty ButtonClickRightCommandProperty =
        DependencyProperty.RegisterAttached("ButtonClickRightCommand", typeof(ICommand), typeof(MediaBehaviour),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(ButtonClickRightCommandChanged)));

        public static readonly DependencyProperty ButtonClickLeftCommandProperty =
        DependencyProperty.RegisterAttached("ButtonClickLeftCommand", typeof(ICommand), typeof(MediaBehaviour),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(ButtonClickLeftCommandChanged)));

        public static readonly DependencyProperty ButtonRandomCommandProperty =
        DependencyProperty.RegisterAttached("ButtonRandomCommand", typeof(ICommand), typeof(MediaBehaviour),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(ButtonRandomCommandChanged)));

        public static readonly DependencyProperty ButtonReplayCommandProperty =
        DependencyProperty.RegisterAttached("ButtonReplayCommand", typeof(ICommand), typeof(MediaBehaviour),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(ButtonReplayCommandChanged)));

        public static readonly DependencyProperty MoveTextCommandProperty =
        DependencyProperty.RegisterAttached("MoveTextCommand", typeof(ICommand), typeof(MediaBehaviour),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(MoveTextCommandChanged)));

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

        public static void ButtonClickRightCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
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

        public static void ButtonClickLeftCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
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
        public static void ButtonRandomCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ToggleButton toggleButton)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    toggleButton.Click += toggleButton_RandomClick;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    toggleButton.Click -= toggleButton_RandomClick;
                }
            }
        }

        public static void ButtonReplayCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ToggleButton toggleButton)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    toggleButton.Click += toggleButton_ReplayClick;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    toggleButton.Click -= toggleButton_ReplayClick;
                }
            }
        }

        public static void MoveTextCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ScrollViewer scrollViewer)
            {
                m_timerMoveText = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(250),
                    Tag = scrollViewer
                };
                m_timerMoveText.Tick += timerMoveText_Tick;
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

        public static void SetButtonClickRightCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(ButtonClickRightCommandProperty, value);
        }

        public static void SetButtonClickLeftCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(ButtonClickLeftCommandProperty, value);
        }

        public static void SetButtonRandomCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(ButtonRandomCommandProperty, value);
        }

        public static void SetButtonReplayCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(ButtonReplayCommandProperty, value);
        }

        public static void SetMoveTextCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(MoveTextCommandProperty, value);
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

        public static ICommand GetButtonClickRightCommand(UIElement element)
           => (ICommand)element.GetValue(ButtonClickRightCommandProperty);

        public static ICommand GetButtonClickLeftCommand(UIElement element)
           => (ICommand)element.GetValue(ButtonClickLeftCommandProperty);

        public static ICommand GetButtonRandomCommand(UIElement element)
           => (ICommand)element.GetValue(ButtonRandomCommandProperty);

        public static ICommand GetButtonReplayCommand(UIElement element)
           => (ICommand)element.GetValue(ButtonReplayCommandProperty);

        public static ICommand GetMoveTextCommand(UIElement element)
            => (ICommand)element.GetValue(MoveTextCommandProperty);

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
                    m_timerMoveText.Stop();
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
                    m_timerMoveText.Start();
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

        private static void timerMoveText_Tick(object sender, EventArgs e)
        {
            if (sender is DispatcherTimer dispatcherTimer && dispatcherTimer.Tag is ScrollViewer scrollViewer)
            {
                var command = GetMoveTextCommand(scrollViewer);
                if (command != null)
                {
                    command.Execute(scrollViewer);
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

        private static void toggleButton_RandomClick(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                var command = GetButtonRandomCommand(toggleButton);
                if (command != null)
                {
                    command.Execute(toggleButton.IsChecked);
                }
            }
        }

        private static void toggleButton_ReplayClick(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                var command = GetButtonReplayCommand(toggleButton);
                if (command != null)
                {
                    command.Execute(toggleButton.IsChecked);
                }
            }
        }

        #endregion // Events
    }
}
