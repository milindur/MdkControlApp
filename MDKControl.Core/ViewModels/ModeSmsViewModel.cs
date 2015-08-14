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
        private readonly IMoCoBusProtocolService _protocolService;

        private RelayCommand _setStartCommand;
        private RelayCommand _setStopCommand;
        private RelayCommand _swapStartStopCommand;
        private RelayCommand _startProgramCommand;
        private RelayCommand _pauseProgramCommand;
        private RelayCommand _stopProgramCommand;

        private float _exposureTime = 1.0f;
        private float _delayTime = 2.0f;
        private float _intervalTime = 3.0f;
        private float _durationTime = 300.0f;

        public ModeSmsViewModel(IDispatcherHelper dispatcherHelper, IMoCoBusProtocolService protocolService)
        {
            _dispatcherHelper = dispatcherHelper;
            _protocolService = protocolService;
        }

        public float ExposureTime
        {
            get { return _exposureTime; }
            set
            {
                _exposureTime = value;
                if (_intervalTime < _exposureTime + _delayTime)
                    _intervalTime = _exposureTime + _delayTime;
                _durationTime = (float)(Math.Ceiling(_durationTime / _intervalTime) * _intervalTime);
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

        public float DelayTime
        {
            get { return _delayTime; }
            set
            {
                _delayTime = value;
                if (_delayTime < 1f)
                    _delayTime = 1f;
                if (_intervalTime < _exposureTime + _delayTime)
                    _intervalTime = _exposureTime + _delayTime;
                _durationTime = (float)(Math.Ceiling(_durationTime / _intervalTime) * _intervalTime);
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

        public float IntervalTime
        {
            get { return _intervalTime; }
            set
            {
                _intervalTime = value;
                if (_intervalTime < _exposureTime + _delayTime)
                {
                    if (_intervalTime < _exposureTime + 1f)
                    {
                        _delayTime = 1f;
                        _intervalTime = _exposureTime + _delayTime;
                    }
                }
                _delayTime = _intervalTime - _exposureTime;
                _durationTime = (float)(Math.Ceiling(_durationTime / _intervalTime) * _intervalTime);
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

        public float DurationTime
        {
            get { return _durationTime; }
            set
            {
                _durationTime = value;
                if (_durationTime < _intervalTime)
                    _durationTime = _intervalTime;
                _durationTime = (float)(Math.Ceiling(_durationTime / _intervalTime) * _intervalTime);
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
            ushort preDelay = 100;
            ushort focusTime = 100;
            uint exposureTime = (uint)(ExposureTime * 1000);
            ushort postDelay = (ushort)((DelayTime * 1000) - preDelay - focusTime);

            await _protocolService.Camera.SetFocusTime(focusTime);
            await _protocolService.Camera.SetTriggerTime(exposureTime);
            await _protocolService.Camera.SetExposureDelayTime(postDelay);
            await _protocolService.Camera.SetMaxShots(MaxShots);

            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.ShootMoveShoot);
            await _protocolService.Main.Start();
        }

        private async void PauseProgram()
        {
            await _protocolService.Main.Pause();
        }

        private async void StopProgram()
        {
            await _protocolService.Main.Stop();
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
