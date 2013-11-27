﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using VLC_WINRT.Common;
using VLC_WINRT.Utility.Commands;
using VLC_WINRT.Utility.Services.RunTime;
using VLC_Wrapper;

namespace VLC_WINRT.ViewModels.PlayVideo
{
    public class PlayVideoViewModel : BindableBase, IDisposable
    {
        private readonly DispatcherTimer _sliderPositionTimer = new DispatcherTimer();
        private readonly DispatcherTimer _fiveSecondTimer = new DispatcherTimer();
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private string _fileToken;
        private bool _isPlaying;
        private PlayPauseCommand _playOrPause;
        private RelayCommand _skipAhead;
        private RelayCommand _skipBack;
        private StopVideoCommand _stopVideoCommand;
        private TimeSpan _timeTotal = TimeSpan.Zero;
        private string _title;
        private Player _vlcPlayer;
        private bool _isVLCInitialized = false;
        private readonly DisplayRequest _displayAlwaysOnRequest;
        private HistoryService _historyService;


        public PlayVideoViewModel()
        {
            _playOrPause = new PlayPauseCommand();
            _historyService = ServiceLocator.Current.GetInstance<HistoryService>();
            _skipAhead = new RelayCommand(() =>
            {
                TimeSpan seekTo = ElapsedTime + TimeSpan.FromSeconds(10);
                double relativePosition = seekTo.TotalMilliseconds/TimeTotal.TotalMilliseconds;
                if (relativePosition > 1.0f)
                {
                    relativePosition = 1.0f;
                }
                _vlcPlayer.Seek((float)relativePosition);
            });
            _skipBack = new RelayCommand(() =>
            {
                TimeSpan seekTo = ElapsedTime - TimeSpan.FromSeconds(10);
                double relativePosition = seekTo.TotalMilliseconds / TimeTotal.TotalMilliseconds;
                if (relativePosition < 0.0f)
                {
                    relativePosition = 0.0f;
                }
                _vlcPlayer.Seek((float)relativePosition);
            });
            _stopVideoCommand = new StopVideoCommand();

            _sliderPositionTimer.Tick += UpdatePosition;
            _sliderPositionTimer.Interval = TimeSpan.FromMilliseconds((1.0d/60.0d));

            _fiveSecondTimer.Tick += UpdateDate;
            _fiveSecondTimer.Interval = TimeSpan.FromSeconds(5);
            _fiveSecondTimer.Start();

            _displayAlwaysOnRequest = new DisplayRequest();
        }

        private void UpdateDate(object sender, object e)
        {
            if (!string.IsNullOrEmpty(_fileToken))
            {
                _historyService.UpdateMediaHistory(_fileToken, ElapsedTime);
            }
            
            OnPropertyChanged("Now");
        }

        public double Position
        {
            get 
            {
                return _isVLCInitialized ? _vlcPlayer.GetPosition() : 0.0d;
            }
            set { _vlcPlayer.Seek((float) value); }
        }

        public string Now
        {
            get
            {
                return DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
            }
        }

        public PlayPauseCommand PlayOrPause
        {
            get { return _playOrPause; }
            set { SetProperty(ref _playOrPause, value); }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                SetProperty(ref _isPlaying, value);
                var mouseService = ServiceLocator.Current.GetInstance<MouseService>();
                if (value)
                {
                    _sliderPositionTimer.Start();
                    mouseService.HideMouse();

                    if (_displayAlwaysOnRequest != null)
                    {
                        _displayAlwaysOnRequest.RequestActive();
                    }
                }
                else
                {
                    _sliderPositionTimer.Stop();
                    mouseService.RestoreMouse();

                    if (_displayAlwaysOnRequest != null)
                    {
                        _displayAlwaysOnRequest.RequestRelease();
                    }
                }
            }
        }

        public RelayCommand SkipAhead
        {
            get { return _skipAhead; }
            set { SetProperty(ref _skipAhead, value); }
        }

        public RelayCommand SkipBack
        {
            get { return _skipBack; }
            set { SetProperty(ref _skipBack, value); }
        }

        public string Title
        {
            get { return _title; }
            private set { SetProperty(ref _title, value); }
        }

        public void SetActiveVideoInfo(string token, string title)
        {
            _fileToken = token;
            Title = title;
        }

        public StopVideoCommand StopVideo
        {
            get { return _stopVideoCommand; }
            set { SetProperty(ref _stopVideoCommand, value); }
        }

        public TimeSpan TimeTotal
        {
            get { return _timeTotal; }
            set { SetProperty(ref _timeTotal, value); }
        }

        public TimeSpan ElapsedTime
        {
            get { return _elapsedTime; }
            set { SetProperty(ref _elapsedTime, value); }
        }

        public void Dispose()
        {
            if (_vlcPlayer != null)
            {
                _vlcPlayer.Dispose();
                _vlcPlayer = null;
            }

            IsPlaying = false;
        }

        public async Task InitializeVLC(SwapChainBackgroundPanel renderPanel)
        {
            _vlcPlayer = new Player(renderPanel);
            await _vlcPlayer.Initialize();
            _isVLCInitialized = true;
            string token =  _historyService.GetTokenAtPosition(0);
            _vlcPlayer.Open("winrt://" + token);
        }

        private void UpdatePosition(object sender, object e)
        {
            OnPropertyChanged("Position");

            if (_timeTotal == TimeSpan.Zero)
            {
                TimeTotal = TimeSpan.FromMilliseconds(_vlcPlayer.GetLength());
            }

            ElapsedTime = TimeSpan.FromMilliseconds(TimeTotal.TotalMilliseconds*Position);
        }

        public void Play()
        {
            _vlcPlayer.Play();
            IsPlaying = true;
        }

        public void Pause()
        {
            _vlcPlayer.Pause();
            IsPlaying = false;
        }

        public void Stop()
        {
            _vlcPlayer.Stop();
            IsPlaying = false;
        }

        public void Seek(float position)
        {
            _vlcPlayer.Seek(position);
        }
    }
}