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

        private int _sliderStartPos = 0;
        private int _sliderStopPos = 0;
        private int _panStartPos = 0;
        private int _panStopPos = 0;
        private int _tiltStartPos = 0;
        private int _tiltStopPos = 0;

        private float _progress = 0;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private int _elapsedShots = 0;

        public ModeSmsViewModel(IDispatcherHelper dispatcherHelper, DeviceViewModel deviceViewModel, IMoCoBusProtocolService protocolService)
        {
            _dispatcherHelper = dispatcherHelper;
            _deviceViewModel = deviceViewModel;
            _protocolService = protocolService;
        }

        public DeviceViewModel DeviceViewModel { get { return _deviceViewModel; } }

        public decimal ExposureTime
        {
            get { return _exposureTime; }
            set
            {
                _exposureTime = value;

                if (_exposureTime < 0.1m)
                    _exposureTime = 0.1m;
                if (_intervalTime < _preDelayTime + _focusTime + _exposureTime + _postDelayTime + _minMotionTime)
                    _intervalTime = _preDelayTime + _focusTime + _exposureTime + _postDelayTime + _minMotionTime;                

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
                if (_postDelayTime > 60m)
                    _postDelayTime = 60m;
                if (_intervalTime < _preDelayTime + _focusTime + _exposureTime + _postDelayTime + _minMotionTime)
                    _intervalTime = _preDelayTime + _focusTime + _exposureTime + _postDelayTime + _minMotionTime;

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

                if (_intervalTime < _preDelayTime + _focusTime + _exposureTime + _postDelayTime + _minMotionTime)
                {
                    _postDelayTime = _intervalTime - _preDelayTime - _focusTime - _exposureTime - _minMotionTime;
                    if (_postDelayTime < 0.1m)
                        _postDelayTime = 0.1m;
                    if (_postDelayTime > 60m)
                        _postDelayTime = 60m;
                    if (_intervalTime < _preDelayTime + _focusTime + _exposureTime + _postDelayTime + _minMotionTime)
                        _intervalTime = _preDelayTime + _focusTime + _exposureTime + _postDelayTime + _minMotionTime;
                }
                else
                {
                    _postDelayTime = _intervalTime - _preDelayTime - _focusTime - _exposureTime - _minMotionTime;                
                    if (_postDelayTime < 0.1m)
                        _postDelayTime = 0.1m;
                    if (_postDelayTime > 60m)
                        _postDelayTime = 60m;
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
            get { return _intervalTime > 0 ? (ushort)(Math.Ceiling(_durationTime / _intervalTime) + 1) : (ushort)0; }
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

        public TimeSpan RemainingTime
        {
            get { return TimeSpan.FromSeconds((double)DurationTime) - ElapsedTime; }
        }

        public int RemainingShots
        {
            get { return MaxShots - ElapsedShots; }
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

        public int SliderStartPosition
        {
            get { return _sliderStartPos; }
        }

        public int SliderStopPosition
        {
            get { return _sliderStopPos; }
        }

        public int PanStartPosition
        {
            get { return _panStartPos; }
        }

        public int PanStopPosition
        {
            get { return _panStopPos; }
        }

        public int TiltStartPosition
        {
            get { return _tiltStartPos; }
        }

        public int TiltStopPosition
        {
            get { return _tiltStopPos; }
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
            await _protocolService.Main.SetProgramStartPoint().ConfigureAwait(false);

            _sliderStartPos = await _protocolService.Motor1.GetProgramStartPoint().ConfigureAwait(false);
            _panStartPos = await _protocolService.Motor2.GetProgramStartPoint().ConfigureAwait(false);
            _tiltStartPos = await _protocolService.Motor3.GetProgramStartPoint().ConfigureAwait(false);
        
            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => SliderStartPosition);
                    RaisePropertyChanged(() => PanStartPosition);
                    RaisePropertyChanged(() => TiltStartPosition);
                });
        }

        private async void SetStop()
        {
            await _protocolService.Main.SetProgramStopPoint().ConfigureAwait(false);

            _sliderStopPos = await _protocolService.Motor1.GetProgramStopPoint().ConfigureAwait(false);
            _panStopPos = await _protocolService.Motor2.GetProgramStopPoint().ConfigureAwait(false);
            _tiltStopPos = await _protocolService.Motor3.GetProgramStopPoint().ConfigureAwait(false);

            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => SliderStopPosition);
                    RaisePropertyChanged(() => PanStopPosition);
                    RaisePropertyChanged(() => TiltStopPosition);
                });
        }

        private async  void SwapStartStop()
        {
            await _protocolService.Main.ReverseAllMotorsStartStopPoints().ConfigureAwait(false);

            _sliderStartPos = await _protocolService.Motor1.GetProgramStartPoint().ConfigureAwait(false);
            _panStartPos = await _protocolService.Motor2.GetProgramStartPoint().ConfigureAwait(false);
            _tiltStartPos = await _protocolService.Motor3.GetProgramStartPoint().ConfigureAwait(false);
            _sliderStopPos = await _protocolService.Motor1.GetProgramStopPoint().ConfigureAwait(false);
            _panStopPos = await _protocolService.Motor2.GetProgramStopPoint().ConfigureAwait(false);
            _tiltStopPos = await _protocolService.Motor3.GetProgramStopPoint().ConfigureAwait(false);

            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => SliderStartPosition);
                    RaisePropertyChanged(() => PanStartPosition);
                    RaisePropertyChanged(() => TiltStartPosition);
                    RaisePropertyChanged(() => SliderStopPosition);
                    RaisePropertyChanged(() => PanStopPosition);
                    RaisePropertyChanged(() => TiltStopPosition);
                });
        }

        private async void StartProgram()
        {
            var preDelay = _preDelayTime * 1000m;
            var focusTime = _focusTime * 1000m;
            var exposureTime = _exposureTime * 1000m;
            var postDelay = _postDelayTime * 1000m;
            var interval = _intervalTime * 1000m;

            if (preDelay > ushort.MaxValue)
                preDelay = 60000m;
            if (focusTime > ushort.MaxValue)
                focusTime = 60000m;
            if (postDelay > ushort.MaxValue)
                postDelay = 60000m;

            await _protocolService.Camera.SetFocusTime((ushort)focusTime).ConfigureAwait(false);
            await _protocolService.Camera.SetTriggerTime((uint)exposureTime).ConfigureAwait(false);
            await _protocolService.Camera.SetExposureDelayTime((ushort)postDelay).ConfigureAwait(false);
            await _protocolService.Camera.SetInterval((uint)interval).ConfigureAwait(false);
            await _protocolService.Camera.SetMaxShots(MaxShots).ConfigureAwait(false);

            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.ShootMoveShoot).ConfigureAwait(false);
            await _protocolService.Main.Start().ConfigureAwait(false);

            _deviceViewModel.StartUpdateTask();
        }

        private async void PauseProgram()
        {
            await _protocolService.Main.Pause().ConfigureAwait(false);
        }

        private async void StopProgram()
        {
            await _protocolService.Main.Stop().ConfigureAwait(false);
            await _deviceViewModel.StopUpdateTask().ConfigureAwait(false);
            await _deviceViewModel.UpdateState().ConfigureAwait(false);
        }

        public async Task UpdateState()
        {
            _progress = await _protocolService.Main.GetProgramPercentComplete().ConfigureAwait(false);
            _elapsedTime = await _protocolService.Main.GetRunTime().ConfigureAwait(false);
            _elapsedShots = await _protocolService.Camera.GetCurrentShots().ConfigureAwait(false);

            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => Progress);
                    RaisePropertyChanged(() => ElapsedTime);
                    RaisePropertyChanged(() => ElapsedShots);
                    RaisePropertyChanged(() => RemainingTime);
                    RaisePropertyChanged(() => RemainingShots);
                    RaisePropertyChanged(() => DurationTime);
                    RaisePropertyChanged(() => MaxShots);
                    RaisePropertyChanged(() => VideoLength24);
                    RaisePropertyChanged(() => VideoLength25);
                    RaisePropertyChanged(() => VideoLength30);
                });
        }

        public async Task InitState()
        {
            _sliderStartPos = await _protocolService.Motor1.GetProgramStartPoint().ConfigureAwait(false);
            _panStartPos = await _protocolService.Motor2.GetProgramStartPoint().ConfigureAwait(false);
            _tiltStartPos = await _protocolService.Motor3.GetProgramStartPoint().ConfigureAwait(false);
            _sliderStopPos = await _protocolService.Motor1.GetProgramStopPoint().ConfigureAwait(false);
            _panStopPos = await _protocolService.Motor2.GetProgramStopPoint().ConfigureAwait(false);
            _tiltStopPos = await _protocolService.Motor3.GetProgramStopPoint().ConfigureAwait(false);

            _exposureTime = (decimal)await _protocolService.Camera.GetTriggerTime().ConfigureAwait(false) / 1000m;
            _postDelayTime = (decimal)await _protocolService.Camera.GetExposureDelayTime().ConfigureAwait(false) / 1000m;
            _intervalTime = (decimal)await _protocolService.Camera.GetInterval().ConfigureAwait(false) / 1000m;
            _durationTime = ((decimal)await _protocolService.Camera.GetMaxShots().ConfigureAwait(false) - 1) * _intervalTime;
            if (_durationTime < 0) _durationTime = 0;

            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => SliderStartPosition);
                    RaisePropertyChanged(() => PanStartPosition);
                    RaisePropertyChanged(() => TiltStartPosition);
                    RaisePropertyChanged(() => SliderStopPosition);
                    RaisePropertyChanged(() => PanStopPosition);
                    RaisePropertyChanged(() => TiltStopPosition);
                    RaisePropertyChanged(() => ExposureTime);
                    RaisePropertyChanged(() => DelayTime);
                    RaisePropertyChanged(() => IntervalTime);
                    RaisePropertyChanged(() => DurationTime);
                    RaisePropertyChanged(() => MaxShots);
                });
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
