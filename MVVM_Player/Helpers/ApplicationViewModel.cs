using MVVM_Player.Helpers;
using MVVM_Player.Helpers.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MVVM_Player
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        #region Commands

        private RelayCommand m_openMediaFiles;
        private RelayCommand m_play;
        private RelayCommand m_pause;
        private RelayCommand m_stop;
        private RelayCommand m_mouseDoubleCommand;
        private RelayCommand m_powerClickCommand;
        private RelayCommand m_visibilityClickCommand;
        private RelayCommand m_scrollViewerCommand;

        private Uri m_currentTrack;
        private string m_currentNameTrack;
        private MediaState m_loadedMode;
        private string m_startTimeTrackPosition;
        private string m_endTimeTrackPosition;
        private TimeSpan m_positionTrack;
        private double m_sliderValue;
        private double m_sliderMinimum;
        private double m_sliderMaximum;
        private Track m_selectedTrack;
        private bool m_randomTracks;
        private bool m_replayTracks;
        private double m_scrollViewerNum;

        #region Media

        private RelayCommand m_mediaEndedCommand;
        private RelayCommand m_mediaOpenedCommand;
        private RelayCommand m_timerTrackCommand;
        private RelayCommand m_buttonRandomCommand;
        private RelayCommand m_buttonReplayCommand;
        //private RelayCommand m_sliderValueChangedCommand;
        //private RelayCommand m_moveRightCommand;
        //private RelayCommand m_moveLeftCommand;

        #endregion // Media

        #endregion // Commands

        #region Constructor

        public ApplicationViewModel()
        {
            StartTimeTrackPosition = "00:00";
            EndTimeTrackPosition = "00:00";
            SliderMinimum = 0.0;
            m_scrollViewerNum = 0.0;

            PlayList = new ObservableCollection<Track>();
        }

        #endregion // Constructor

        #region Properties

        public ObservableCollection<Track> PlayList { get; set; }

        public RelayCommand PlayCommand => m_play ?? (m_play = new RelayCommand(obj =>
        {
            if (obj is Track track)
            {
                PlayTrack(track.Name);
            }
        }));     

        public RelayCommand PauseCommand => m_pause ?? (m_pause = new RelayCommand(obj =>
        {
            LoadedMode = MediaState.Pause;
        }));

        public RelayCommand StopCommand => m_stop ?? (m_stop = new RelayCommand(obj =>
        {
            LoadedMode = MediaState.Stop;
        }));

        public RelayCommand AddMediaFilesCommand => m_openMediaFiles ?? (m_openMediaFiles = new RelayCommand(obj =>
        {
            using (var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Media files (*.mp3)|*.mp3|Paylist open (*.playlist)|*.playlist",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
            })
            {
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                switch (Path.GetExtension(openFileDialog.FileName))
                {
                    default:
                    case ".mp3":
                        foreach (var fileName in openFileDialog.FileNames)
                        {
                            if (PlayList.FirstOrDefault(f => f.FullName == fileName) != null)
                                continue;

                            PlayList.Add(new Track
                            {
                                Name = Path.GetFileNameWithoutExtension(fileName),
                                FullName = fileName,
                                Source = new Uri(fileName)
                            });
                        }
                        break;
                    case ".playlist":
                        OpenPlayList(openFileDialog.FileName);
                        break;
                }

                SelectedTrack = PlayList.FirstOrDefault();
            }
        }));

        public RelayCommand MouseDoubleClickCommand => m_mouseDoubleCommand ?? (m_mouseDoubleCommand = new RelayCommand(obj =>
        {
            if (obj is Track track)
            {
                PlayTrack(track.Name);
            }
        }));

        public RelayCommand PowerClickCommand => m_powerClickCommand ?? (m_powerClickCommand = new RelayCommand(obj =>
        {
            if (obj is ClickMode clickMode && clickMode == ClickMode.Release)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }));

        public RelayCommand VisibilityClickCommand => m_visibilityClickCommand ?? (m_visibilityClickCommand = new RelayCommand(obj =>
        {
            if (obj is ClickMode clickMode && clickMode == ClickMode.Release)
            {
                switch (System.Windows.Application.Current.MainWindow.WindowState)
                {
                    case WindowState.Normal:
                        System.Windows.Application.Current.MainWindow.WindowState = WindowState.Minimized;
                        break;
                    case WindowState.Maximized:
                        System.Windows.Application.Current.MainWindow.WindowState = WindowState.Normal;
                        break;
                }
            }
        }));

        public RelayCommand ScrollViewerCommand => m_scrollViewerCommand ?? (m_scrollViewerCommand = new RelayCommand(obj =>
        {
            if (obj is ScrollViewer scrollViewer || LoadedMode != MediaState.Stop || LoadedMode !=)
            {
                try
                {
                    scrollViewer.ScrollToHorizontalOffset(m_scrollViewerNum);
                    m_scrollViewerNum++;
                }
                catch { }
            }
        }));

        public RelayCommand MediaEndedCommand => m_mediaEndedCommand ?? (m_mediaEndedCommand = new RelayCommand(obj =>
        {
            if (obj is Track track)
            {
                LoadedMode = MediaState.Close;
                NextPlayTrack(track);
            }
        }));

        public RelayCommand MediaOpenedCommand => m_mediaOpenedCommand ?? (m_mediaOpenedCommand = new RelayCommand(obj =>
        {
            if (obj is Duration duration && duration.HasTimeSpan)
            {
                var timespan = duration.TimeSpan;
                EndTimeTrackPosition = timespan.ToString("mm\\:ss");

                if (SliderMaximum != timespan.TotalSeconds)
                    SliderMaximum = timespan.TotalSeconds;
            }
        }));

        public RelayCommand TimerTrackCommand => m_timerTrackCommand ?? (m_timerTrackCommand = new RelayCommand(obj =>
        {
            if (obj is TimeSpan position)
            {
                StartTimeTrackPosition = position.ToString("mm\\:ss");
                SliderValue = position.TotalSeconds;
            }
        }));

        public RelayCommand ButtonRandomCommand => m_buttonRandomCommand ?? (m_buttonRandomCommand = new RelayCommand(obj =>
        {
            if (obj is bool isRandomTracks)
            {
                m_randomTracks = isRandomTracks;
            }
        }));

        public RelayCommand ButtonReplayCommand => m_buttonReplayCommand ?? (m_buttonReplayCommand = new RelayCommand(obj =>
        {
            if (obj is bool isReplayTracks)
            {
                m_replayTracks = isReplayTracks;
            }
        }));

        //public RelayCommand SliderValueChangedCommand => m_sliderValueChangedCommand ?? (m_sliderValueChangedCommand = new RelayCommand(obj =>
        //{
        //    if (obj is TimeSpan position)
        //    {
        //        StartTimeTrackPosition = position.ToString("mm\\:ss");
        //        SliderValue = position.TotalSeconds;
        //    }
        //}));

        //public RelayCommand MoveRightCommand => m_moveRightCommand ?? (m_moveRightCommand = new RelayCommand(obj =>
        //{
        //    if (obj is TimeSpan position)
        //    {
        //        StartTimeTrackPosition = position.ToString("mm\\:ss");
        //        SliderValue = position.TotalSeconds;
        //    }
        //}));

        //public RelayCommand MoveLeftCommand => m_moveLeftCommand ?? (m_moveLeftCommand = new RelayCommand(obj =>
        //{
        //    if (obj is TimeSpan position)
        //    {
        //        StartTimeTrackPosition = position.ToString("mm\\:ss");
        //        SliderValue = position.TotalSeconds;
        //    }
        //}));

        public Uri CurrentTrack
        {
            get { return m_currentTrack; }
            set
            {
                if (m_currentTrack == value)
                    return;

                m_currentTrack = value;
                OnPropetyChanged("CurrentTrack");
            }
        }

        public string CurrentNameTrack
        {
            get { return m_currentNameTrack; }
            set
            {
                if (m_currentNameTrack == value)
                    return;

                m_currentNameTrack = value;
                OnPropetyChanged("CurrentNameTrack");
            }
        }

        public string StartTimeTrackPosition
        {
            get { return m_startTimeTrackPosition; }
            set
            {
                if (m_startTimeTrackPosition == value)
                    return;

                m_startTimeTrackPosition = value;
                OnPropetyChanged("StartTimeTrackPosition");
            }
        }

        public string EndTimeTrackPosition
        {
            get { return m_endTimeTrackPosition; }
            set
            {
                if (m_endTimeTrackPosition == value)
                    return;

                m_endTimeTrackPosition = value;
                OnPropetyChanged("EndTimeTrackPosition");
            }
        }

        public MediaState LoadedMode
        {
            get { return m_loadedMode; }
            set
            {
                if (m_loadedMode == value)
                    return;

                m_loadedMode = value;
                OnPropetyChanged("LoadedMode");
            }
        }

        public TimeSpan PositionTrack
        {
            get { return m_positionTrack; }
            set
            {
                if (m_positionTrack == value)
                    return;

                m_positionTrack = value;
                OnPropetyChanged("PositionTrack");
            }
        }

        public double SliderValue
        {
            get { return m_sliderValue; }
            set
            {
                if (m_sliderValue == value)
                    return;

                m_sliderValue = value;
                OnPropetyChanged("SliderValue");
            }
        }

        public double SliderMinimum
        {
            get { return m_sliderMinimum; }
            set
            {
                if (m_sliderMinimum == value)
                    return;

                m_sliderMinimum = value;
                OnPropetyChanged("SliderMinimum");
            }
        }

        public double SliderMaximum
        {
            get { return m_sliderMaximum; }
            set
            {
                if (m_sliderMaximum == value)
                    return;

                m_sliderMaximum = value;
                OnPropetyChanged("SliderMaximum");
            }
        }
        public Track SelectedTrack
        {
            get { return m_selectedTrack; }
            set
            {
                if (m_selectedTrack == value)
                    return;

                m_selectedTrack = value;
                OnPropetyChanged("SelectedTrack");
            }
        }

        #endregion // Properties

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropetyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion // INotifyPropertyChanged

        #region Helpers

        private void PlayTrack(string selectedNameTrack)
        {
            if (PlayList.Count > 0)
            {
                var currentTrack = PlayList.FirstOrDefault(f => f.Name == selectedNameTrack);
                if (currentTrack != null)
                {
                    CurrentTrack = currentTrack.Source;
                    CurrentNameTrack = Path.GetFileNameWithoutExtension(currentTrack.FullName);
                    LoadedMode = MediaState.Play;
                }
            }
        }

        private void NextPlayTrack(Track selectedTrack)
        {
            if (PlayList.Count > 0)
            {
                var nextTrack = default(Track);
                if (m_randomTracks)
                {
                    var randomTrack = new Random();
                    var nextIndexTrack = randomTrack.Next(0, PlayList.Count);
                    nextTrack = PlayList[nextIndexTrack];
                }
                else
                {
                    var currentIndexTrack = PlayList.IndexOf(selectedTrack);
                    var nextIndexTrack = currentIndexTrack + 1;
                    nextTrack = m_replayTracks ? 
                        selectedTrack : 
                        nextIndexTrack < PlayList.Count ? 
                        PlayList[nextIndexTrack] : 
                        PlayList.FirstOrDefault();
                }
              
                CurrentTrack = nextTrack.Source;
                CurrentNameTrack = Path.GetFileNameWithoutExtension(nextTrack.FullName);
                SelectedTrack = nextTrack;
                SliderMinimum = SliderMaximum = SliderValue = 0.0;
                LoadedMode = MediaState.Play;
            }
        }

        private void OpenPlayList(string fileName)
        {
            var tracksList = ReadPlayList(fileName);
            if (tracksList == null)
                return;

            foreach (var track in tracksList)
            {
                if (PlayList.FirstOrDefault(x => x.FullName == track) != null)
                    continue;

                PlayList.Add(new Track
                {
                    FullName = track,
                    Source = new Uri(track)
                });
                //textBox.Items.Add(System.IO.Path.GetFileNameWithoutExtension(track));
            }
        }

        public string[] ReadPlayList(string fileName)
        {
            using (var openFile = File.OpenRead(fileName))
            {

            }
            // string[] play = null;
            //if (file.Exists)
            //{
            //    play = new string[File.ReadAllLines("C:\\Users\\Volodymyr\\Desktop\\WPF\\WpfAudioAndVideoPlayer\\PlayList.playlist").Length];
            //    int count = 0;

            //    if (!string.IsNullOrEmpty(file.ToString()))
            //        using (var read = File.OpenText(file.ToString()))
            //        {
            //            var readLine = string.Empty;

            //            ((readLine = read.ReadLine()) != string.Empty)
            //            {
            //                for (int i = 0; i <= play.Length; i++)
            //                    play[i] = readLine;
            //                count++;
            //            }
            //        }
            //}
            return new string[] { };
        }

        #endregion // Heplpers
    }
}
