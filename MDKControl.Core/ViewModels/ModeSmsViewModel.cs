﻿using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDKControl.Core.Helpers;
using MDKControl.Core.Models;
using MDKControl.Core.Services;
using Reactive.Bindings;

namespace MDKControl.Core.ViewModels
{
    public class ModeSmsViewModel : ViewModelBase
    {
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly DeviceViewModel _deviceViewModel;
        private readonly IMoCoBusProtocolService _protocolService;

        private RelayCommand _setStartCommand;
        private RelayCommand _setStopCommand;
        private RelayCommand _swapStartStopCommand;
        private RelayCommand _startProgramCommand;
        private RelayCommand _pauseProgramCommand;
        private RelayCommand _stopProgramCommand;

        private const decimal _minMotionTime = 1.5m;
        private const decimal _preDelayTime = 0.1m;
        private const decimal _focusTime = 0.1m;
        private decimal _exposureTime = 0.1m;
        private decimal _postDelayTime = 1.2m;
        private decimal _intervalTime = 3.0m;
        private decimal _durationTime = 300.0m;

        private float _progress = 0;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private int _elapsedShots = 0;

        public ModeSmsViewModel(IDispatcherHelper dispatcherHelper, DeviceViewModel deviceViewModel, IMoCoBusProtocolService protocolService)
        {
            _dispatcherHelper = dispatcherHelper;
            _deviceViewModel = deviceViewModel;
            _protocolService = protocolService;
        }

        public decimal ExposureTime
        {
            get { return _exposureTime; }
            set
            {
                _exposureTime = value;

                if (_exposureTime < 0.1m)
                    _exposureTime = 0.1m;
                if (_intervalTime < _preDelayTime + _focusTime + _exposureTime + _postDelayTime - _minMotionTime)
                    _intervalTime = _preDelayTime + _focusTime + _exposureTime + _postDelayTime - _minMotionTime;

                _durationTime = Math.Ceiling(_durationTime / _intervalTime) * _intervalTime;

                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => ExposureTime);
                        RaisePropertyChanged(() => DelayTime);
                        RaisePropertyChanged(() => IntervalTime);
                        RaisePropertyChanged(() => DurationTime);
                        RaisePropertyChanged(() => MaxShots);
                    });
            }
        }

        public decimal DelayTime
        {
            get { return _postDelayTime; }
            set
            {
                _postDelayTime = value;

                if (_postDelayTime < 0.1m)
                    _postDelayTime = 0.1m;
                if (_intervalTime < _preDelayTime + _focusTime + _exposureTime + _postDelayTime - _minMotionTime)
                    _intervalTime = _preDelayTime + _focusTime + _exposureTime + _postDelayTime - _minMotionTime;

                _durationTime = Math.Ceiling(_durationTime / _intervalTime) * _intervalTime;

                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => ExposureTime);
                        RaisePropertyChanged(() => DelayTime);
                        RaisePropertyChanged(() => IntervalTime);
                        RaisePropertyChanged(() => DurationTime);
                        RaisePropertyChanged(() => MaxShots);
                    });
            }
        }

        public decimal IntervalTime
        {
            get { return _intervalTime; }
            set
            {
                _intervalTime = value;

                if (_intervalTime < _preDelayTime + _focusTime + _exposureTime + _postDelayTime - _minMotionTime)
                {
                    _postDelayTime = _intervalTime - _preDelayTime - _focusTime - _exposureTime - _minMotionTime;
                    if (_postDelayTime < 0.1m)
                        _postDelayTime = 0.1m;
                    if (_intervalTime < _preDelayTime + _focusTime + _exposureTime + _postDelayTime - _minMotionTime)
                        _intervalTime = _preDelayTime + _focusTime + _exposureTime + _postDelayTime - _minMotionTime;
                }
                else
                {
                    _postDelayTime = _intervalTime - _preDelayTime - _focusTime - _exposureTime - _minMotionTime;                
                }

                _durationTime = Math.Ceiling(_durationTime / _intervalTime) * _intervalTime;

                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => ExposureTime);
                        RaisePropertyChanged(() => DelayTime);
                        RaisePropertyChanged(() => IntervalTime);
                        RaisePropertyChanged(() => DurationTime);
                        RaisePropertyChanged(() => MaxShots);
                    });
            }
        }

        public decimal DurationTime
        {
            get { return _durationTime; }
            set
            {
                _durationTime = value;

                if (_durationTime < _intervalTime)
                    _durationTime = _intervalTime;

                _durationTime = Math.Ceiling(_durationTime / _intervalTime) * _intervalTime;

                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => ExposureTime);
                        RaisePropertyChanged(() => DelayTime);
                        RaisePropertyChanged(() => IntervalTime);
                        RaisePropertyChanged(() => DurationTime);
                        RaisePropertyChanged(() => MaxShots);
                    });
            }
        }

        public ushort MaxShots
        {
            get { return (ushort)(Math.Ceiling(_durationTime / _intervalTime) + 1); }
            set
            {
                var tmp = value;

                if (tmp < 2)
                    tmp = 2;
                
                _durationTime = (tmp - 1) * _intervalTime;

                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => ExposureTime);
                        RaisePropertyChanged(() => DelayTime);
                        RaisePropertyChanged(() => IntervalTime);
                        RaisePropertyChanged(() => DurationTime);
                        RaisePropertyChanged(() => MaxShots);
                    });
            }
        }

        public float Progress
        {
            get { return _progress; }
        }

        public TimeSpan ElapsedTime
        {
            get { return _elapsedTime; }
        }

        public int ElapsedShots
        {
            get { return _elapsedShots; }
        }

        public TimeSpan VideoLength24
        {
            get { return TimeSpan.FromSeconds(_elapsedShots / 24f); }
        }

        public TimeSpan VideoLength25
        {
            get { return TimeSpan.FromSeconds(_elapsedShots / 25f); }
        }

        public TimeSpan VideoLength30
        {
            get { return TimeSpan.FromSeconds(_elapsedShots / 30f); }
        }

        public RelayCommand SetStartCommand
        {
            get { return _setStartCommand ?? (_setStartCommand = new RelayCommand(SetStart)); }
        }

        public RelayCommand SetStopCommand
        {
            get { return _setStopCommand ?? (_setStopCommand = new RelayCommand(SetStop)); }
        }

        public RelayCommand SwapStartStopCommand
        {
            get { return _swapStartStopCommand ?? (_swapStartStopCommand = new RelayCommand(SwapStartStop)); }
        }

        public RelayCommand StartProgramCommand
        {
            get { return _startProgramCommand ?? (_startProgramCommand = new RelayCommand(StartProgram)); }
        }

        public RelayCommand PauseProgramCommand
        {
            get { return _pauseProgramCommand ?? (_pauseProgramCommand = new RelayCommand(PauseProgram)); }
        }

        public RelayCommand StopProgramCommand
        {
            get { return _stopProgramCommand ?? (_stopProgramCommand = new RelayCommand(StopProgram)); }
        }

        private async void SetStart()
        {
            await _protocolService.Main.SetProgramStartPoint();
        }

        private async void SetStop()
        {
            await _protocolService.Main.SetProgramStopPoint();
        }

        private async  void SwapStartStop()
        {
            await _protocolService.Main.ReverseAllMotorsStartStopPoints();
        }

        private async void StartProgram()
        {
            var preDelay = (ushort)(_preDelayTime * 1000m);
            var focusTime = (ushort)(_focusTime * 1000m);
            var exposureTime = (uint)(_exposureTime * 1000m);
            var postDelay = (ushort)(_postDelayTime * 1000m);
            var interval = (uint)(_intervalTime * 1000m);

            await _protocolService.Camera.SetFocusTime(focusTime);
            await _protocolService.Camera.SetTriggerTime(exposureTime);
            await _protocolService.Camera.SetExposureDelayTime(postDelay);
            await _protocolService.Camera.SetInterval(interval);

            await _protocolService.Camera.SetMaxShots(MaxShots);

            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.ShootMoveShoot);
            await _protocolService.Main.Start();

            _deviceViewModel.StartUpdateTask();
        }

        private async void PauseProgram()
        {
            await _protocolService.Main.Pause();
        }

        private async void StopProgram()
        {
            await _protocolService.Main.Stop();
            await _deviceViewModel.StopUpdateTask();
        }

        public async Task UpdateState()
        {
            _progress = await _protocolService.Main.GetProgramPercentComplete();
            _elapsedTime = await _protocolService.Main.GetRunTime();

            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => Progress);
                    RaisePropertyChanged(() => ElapsedTime);
                    RaisePropertyChanged(() => ElapsedShots);
                    /*RaisePropertyChanged(() => RemainingTime);
                    RaisePropertyChanged(() => RemainingShots);
                    RaisePropertyChanged(() => OverallTime);
                    RaisePropertyChanged(() => OverallShots);*/
                    RaisePropertyChanged(() => VideoLength24);
                    RaisePropertyChanged(() => VideoLength25);
                    RaisePropertyChanged(() => VideoLength30);
                });
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
