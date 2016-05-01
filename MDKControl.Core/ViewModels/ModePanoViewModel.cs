using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDKControl.Core.Helpers;
using MDKControl.Core.Models;
using MDKControl.Core.Services;
using Xamarin;

namespace MDKControl.Core.ViewModels
{
    public class ModePanoViewModel : ViewModelBase, IState
    {
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly DeviceViewModel _deviceViewModel;
        private readonly IMoCoBusProtocolService _protocolService;

        private RelayCommand _setStartCommand;
        private RelayCommand _setStopCommand;
        private RelayCommand _swapStartStopCommand;
        private RelayCommand _setRefStartCommand;
        private RelayCommand _setRefStopCommand;
        private RelayCommand _startProgramCommand;
        private RelayCommand _pauseProgramCommand;
        private RelayCommand _stopProgramCommand;

        private decimal _preDelayTime = 0.1m;
        private decimal _exposureTime = 0.1m;
        private decimal _postDelayTime = 0.5m;
        private decimal _pauseTime = 60.0m;
        private ushort _repetitions = 1;

        private int _panStartPos;
        private int _panStopPos;
        private int _tiltStartPos;
        private int _tiltStopPos;

        private float _progress;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private int _elapsedShots;
        private int _overallShots;
        private uint _overallCols;
        private uint _overallRows;

        public ModePanoViewModel(IDispatcherHelper dispatcherHelper, DeviceViewModel deviceViewModel, IMoCoBusProtocolService protocolService)
        {
            _dispatcherHelper = dispatcherHelper;
            _deviceViewModel = deviceViewModel;
            _protocolService = protocolService;
        }

        public DeviceViewModel DeviceViewModel => _deviceViewModel;

        public decimal ExposureTime
        {
            get { return _exposureTime; }
            set
            {
                _exposureTime = value;
                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => ExposureTime);
                        RaisePropertyChanged(() => PreDelayTime);
                        RaisePropertyChanged(() => DelayTime);
                    });
            }
        }

        public decimal PreDelayTime
        {
            get { return _preDelayTime; }
            set
            {
                _preDelayTime = value;
                if (_preDelayTime < 0.1m)
                    _preDelayTime = 0.1m;
                if (_preDelayTime > 60m)
                    _preDelayTime = 60m;
                _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => ExposureTime);
                    RaisePropertyChanged(() => PreDelayTime);
                    RaisePropertyChanged(() => DelayTime);
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
                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => ExposureTime);
                        RaisePropertyChanged(() => PreDelayTime);
                        RaisePropertyChanged(() => DelayTime);
                    });
            }
        }

        public decimal PauseTime
        {
            get { return _pauseTime; }
            set
            {
                _pauseTime = value;
                if (_pauseTime < 0.0m)
                    _pauseTime = 0.0m;
                _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => PauseTime);
                });
            }
        }

        public ushort Repititions
        {
            get { return _repetitions; }
            set
            {
                _repetitions = value;
                if (_repetitions < 1)
                    _repetitions = 1;
                _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => Repititions);
                });
            }
        }

        public float Progress => _progress;

        public TimeSpan ElapsedTime => _elapsedTime;

        public int ElapsedShots => _elapsedShots;

        public TimeSpan RemainingTime => OverallTime - ElapsedTime;

        public int RemainingShots => OverallShots - ElapsedShots;

        public TimeSpan OverallTime => TimeSpan.Zero;

        public int OverallShots => _overallShots;

        public uint OverallCols => _overallCols;

        public uint OverallRows => _overallRows;

        public int PanRefSize => 0;

        public int TiltRefSize => 0;

        public int PanStartPosition => _panStartPos;

        public int PanStopPosition => _panStopPos;

        public int PanSize => _panStopPos - _panStartPos;

        public int TiltStartPosition => _tiltStartPos;

        public int TiltStopPosition => _tiltStopPos;

        public int TiltSize => _tiltStopPos - _tiltStartPos;

        public RelayCommand SetStartCommand => _setStartCommand ?? (_setStartCommand = new RelayCommand(SetStart));

        public RelayCommand SetStopCommand => _setStopCommand ?? (_setStopCommand = new RelayCommand(SetStop));

        public RelayCommand SwapStartStopCommand
        {
            get { return _swapStartStopCommand ?? (_swapStartStopCommand = new RelayCommand(SwapStartStop, () => false)); }
        }

        public RelayCommand SetRefStartCommand => _setRefStartCommand ?? (_setRefStartCommand = new RelayCommand(SetRefStart));

        public RelayCommand SetRefStopCommand => _setRefStopCommand ?? (_setRefStopCommand = new RelayCommand(SetRefStop));

        private bool _startProgramRunning;
        public RelayCommand StartProgramCommand
        {
            get { 
                return _startProgramCommand ?? (_startProgramCommand = new RelayCommand(async () =>
                    {
                        try
                        {
                            _startProgramRunning = true;
                            _startProgramCommand.RaiseCanExecuteChanged();

                            await StartProgram();
                        }
                        finally
                        {
                            _startProgramRunning = false;
                            _startProgramCommand.RaiseCanExecuteChanged();
                        }
                    }, () => !_startProgramRunning));
            }
        }

        public RelayCommand PauseProgramCommand => _pauseProgramCommand ?? (_pauseProgramCommand = new RelayCommand(PauseProgram));

        public RelayCommand StopProgramCommand => _stopProgramCommand ?? (_stopProgramCommand = new RelayCommand(StopProgram));

        private async void SetStart()
        {
            try
            {
                await _protocolService.Main.SetProgramStartPoint().ConfigureAwait(false);

                _panStartPos = await _protocolService.MotorPan.GetProgramStartPoint().ConfigureAwait(false);
                _tiltStartPos = await _protocolService.MotorTilt.GetProgramStartPoint().ConfigureAwait(false);

                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => PanStartPosition);
                        RaisePropertyChanged(() => TiltStartPosition);
                        RaisePropertyChanged(() => PanSize);
                        RaisePropertyChanged(() => TiltSize);
                    });
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        private async void SetStop()
        {
            try
            {
                await _protocolService.Main.SetProgramStopPoint().ConfigureAwait(false);

                _panStopPos = await _protocolService.MotorPan.GetProgramStopPoint().ConfigureAwait(false);
                _tiltStopPos = await _protocolService.MotorTilt.GetProgramStopPoint().ConfigureAwait(false);

                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => PanStopPosition);
                        RaisePropertyChanged(() => TiltStopPosition);
                        RaisePropertyChanged(() => PanSize);
                        RaisePropertyChanged(() => TiltSize);
                    });
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        private async  void SwapStartStop()
        {
            try
            {
                await _protocolService.Main.ReverseAllMotorsStartStopPoints().ConfigureAwait(false);

                _panStartPos = await _protocolService.MotorPan.GetProgramStartPoint().ConfigureAwait(false);
                _tiltStartPos = await _protocolService.MotorTilt.GetProgramStartPoint().ConfigureAwait(false);
                _panStopPos = await _protocolService.MotorPan.GetProgramStopPoint().ConfigureAwait(false);
                _tiltStopPos = await _protocolService.MotorTilt.GetProgramStopPoint().ConfigureAwait(false);

                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => PanStartPosition);
                        RaisePropertyChanged(() => TiltStartPosition);
                        RaisePropertyChanged(() => PanStopPosition);
                        RaisePropertyChanged(() => TiltStopPosition);
                        RaisePropertyChanged(() => PanSize);
                        RaisePropertyChanged(() => TiltSize);
                    });
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        private async void SetRefStart()
        {
            try
            {
                await _protocolService.MotorPan.SetStartHere().ConfigureAwait(false);
                await _protocolService.MotorTilt.SetStartHere().ConfigureAwait(false);
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        private async void SetRefStop()
        {
            try
            {
                await _protocolService.MotorPan.SetStopHere().ConfigureAwait(false);
                await _protocolService.MotorTilt.SetStopHere().ConfigureAwait(false);
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        private async Task StartProgram()
        {
            try
            {
                var preDelay = PreDelayTime * 1000m;
                ushort focusTime = 100;
                var exposureTime = ExposureTime * 1000m;
                var postDelay = DelayTime * 1000m;
                var pause = _pauseTime * 1000m;

                if (preDelay > ushort.MaxValue)
                    preDelay = 60000m;
                if (postDelay > ushort.MaxValue)
                    postDelay = 60000m;

                await _protocolService.Camera.SetPreDelayTime((ushort)preDelay).ConfigureAwait(false);
                await _protocolService.Camera.SetFocusTime(focusTime).ConfigureAwait(false);
                await _protocolService.Camera.SetTriggerTime((uint)exposureTime).ConfigureAwait(false);
                await _protocolService.Camera.SetExposureDelayTime((ushort)postDelay).ConfigureAwait(false);
                await _protocolService.Camera.SetInterval((uint)pause).ConfigureAwait(false);
                await _protocolService.Camera.SetPanoRepititions(Repititions).ConfigureAwait(false);

                await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Panorama).ConfigureAwait(false);
                await _protocolService.Main.Start().ConfigureAwait(false);

                _overallCols = await _protocolService.MotorPan.GetTravelShots().ConfigureAwait(false);
                _overallRows = await _protocolService.MotorTilt.GetTravelShots().ConfigureAwait(false);

                _deviceViewModel.StartUpdateTask();
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        private async void PauseProgram()
        {
            try
            {
                await _protocolService.Main.Pause().ConfigureAwait(false);
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        private async void StopProgram()
        {
            try
            {
                await _protocolService.Main.Stop().ConfigureAwait(false);
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
            try
            {
                await _deviceViewModel.StopUpdateTask().ConfigureAwait(false);
                await _deviceViewModel.UpdateState().ConfigureAwait(false);
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        public async Task SaveState()
        {
            try
            {
                var preDelay = PreDelayTime * 1000m;
                ushort focusTime = 100;
                var exposureTime = ExposureTime * 1000m;
                var postDelay = DelayTime * 1000m;
                var pause = _pauseTime * 1000m;

                if (preDelay > ushort.MaxValue)
                    preDelay = 60000m;
                if (postDelay > ushort.MaxValue)
                    postDelay = 60000m;

                await _protocolService.Camera.SetPreDelayTime((ushort)preDelay).ConfigureAwait(false);
                await _protocolService.Camera.SetFocusTime(focusTime).ConfigureAwait(false);
                await _protocolService.Camera.SetTriggerTime((uint)exposureTime).ConfigureAwait(false);
                await _protocolService.Camera.SetExposureDelayTime((ushort)postDelay).ConfigureAwait(false);
                await _protocolService.Camera.SetInterval((uint)pause).ConfigureAwait(false);
                await _protocolService.Camera.SetPanoRepititions(Repititions).ConfigureAwait(false);
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        public async Task UpdateState()
        {
            try
            {
                _progress = await _protocolService.Main.GetProgramPercentComplete().ConfigureAwait(false);
                _elapsedTime = await _protocolService.Main.GetRunTime().ConfigureAwait(false);
                _elapsedShots = await _protocolService.Camera.GetCurrentShots().ConfigureAwait(false);
                _overallShots = await _protocolService.Camera.GetMaxShots().ConfigureAwait(false);
                _overallCols = await _protocolService.MotorPan.GetTravelShots().ConfigureAwait(false);
                _overallRows = await _protocolService.MotorTilt.GetTravelShots().ConfigureAwait(false);

                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => Progress);
                        RaisePropertyChanged(() => ElapsedTime);
                        RaisePropertyChanged(() => ElapsedShots);
                        RaisePropertyChanged(() => RemainingTime);
                        RaisePropertyChanged(() => RemainingShots);
                        RaisePropertyChanged(() => OverallTime);
                        RaisePropertyChanged(() => OverallShots);
                        RaisePropertyChanged(() => OverallCols);
                        RaisePropertyChanged(() => OverallRows);
                    });
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        public async Task InitState()
        {
            try
            {
                _panStartPos = await _protocolService.MotorPan.GetProgramStartPoint().ConfigureAwait(false);
                _tiltStartPos = await _protocolService.MotorTilt.GetProgramStartPoint().ConfigureAwait(false);
                _panStopPos = await _protocolService.MotorPan.GetProgramStopPoint().ConfigureAwait(false);
                _tiltStopPos = await _protocolService.MotorTilt.GetProgramStopPoint().ConfigureAwait(false);

                _preDelayTime = await _protocolService.Camera.GetPreDelayTime().ConfigureAwait(false) / 1000m;
                _exposureTime = await _protocolService.Camera.GetTriggerTime().ConfigureAwait(false) / 1000m;
                _postDelayTime = await _protocolService.Camera.GetExposureDelayTime().ConfigureAwait(false) / 1000m;
                _pauseTime = await _protocolService.Camera.GetInterval().ConfigureAwait(false) / 1000m;
                _repetitions = await _protocolService.Camera.GetPanoRepititions().ConfigureAwait(false);

                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => PanStartPosition);
                        RaisePropertyChanged(() => TiltStartPosition);
                        RaisePropertyChanged(() => PanStopPosition);
                        RaisePropertyChanged(() => TiltStopPosition);
                        RaisePropertyChanged(() => PanSize);
                        RaisePropertyChanged(() => TiltSize);
                        RaisePropertyChanged(() => ExposureTime);
                        RaisePropertyChanged(() => PreDelayTime);
                        RaisePropertyChanged(() => DelayTime);
                        RaisePropertyChanged(() => PauseTime);
                        RaisePropertyChanged(() => Repititions);
                    });
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }
    }
}
