using System;
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
    public class ModePanoViewModel : ViewModelBase
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

        private decimal _exposureTime = 0.1m;
        private decimal _delayTime = 0.5m;

        private int _panStartPos = 0;
        private int _panStopPos = 0;
        private int _tiltStartPos = 0;
        private int _tiltStopPos = 0;

        private float _progress = 0;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private int _elapsedShots = 0;
        private int _overallShots = 0;
        private uint _overallCols = 0;
        private uint _overallRows = 0;

        public ModePanoViewModel(IDispatcherHelper dispatcherHelper, DeviceViewModel deviceViewModel, IMoCoBusProtocolService protocolService)
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
                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => ExposureTime);
                        RaisePropertyChanged(() => DelayTime);
                    });
            }
        }

        public decimal DelayTime
        {
            get { return _delayTime; }
            set
            {
                _delayTime = value;
                if (_delayTime < 0.1m)
                    _delayTime = 0.1m;
                if (_delayTime > 60m)
                    _delayTime = 60m;
                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => ExposureTime);
                        RaisePropertyChanged(() => DelayTime);
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
            get { return OverallTime - ElapsedTime; }
        }

        public int RemainingShots
        {
            get { return OverallShots - ElapsedShots; }
        }

        public TimeSpan OverallTime
        {
            get { return TimeSpan.Zero; }
        }

        public int OverallShots
        {
            get { return _overallShots; }
        }

        public uint OverallCols
        {
            get { return _overallCols; }
        }

        public uint OverallRows
        {
            get { return _overallRows; }
        }

        public int PanStartPosition
        {
            get { return _panStartPos; }
        }

        public int PanStopPosition
        {
            get { return _panStopPos; }
        }

        public int PanSize
        {
            get { return _panStopPos - _panStartPos; }
        }

        public int TiltStartPosition
        {
            get { return _tiltStartPos; }
        }

        public int TiltStopPosition
        {
            get { return _tiltStopPos; }
        }

        public int TiltSize
        {
            get { return _tiltStopPos - _tiltStartPos; }
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
            get { return _swapStartStopCommand ?? (_swapStartStopCommand = new RelayCommand(SwapStartStop, () => false)); }
        }

        public RelayCommand SetRefStartCommand
        {
            get { return _setRefStartCommand ?? (_setRefStartCommand = new RelayCommand(SetRefStart)); }
        }

        public RelayCommand SetRefStopCommand
        {
            get { return _setRefStopCommand ?? (_setRefStopCommand = new RelayCommand(SetRefStop)); }
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

            _panStartPos = await _protocolService.Motor2.GetProgramStartPoint().ConfigureAwait(false);
            _tiltStartPos = await _protocolService.Motor3.GetProgramStartPoint().ConfigureAwait(false);

            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => PanStartPosition);
                    RaisePropertyChanged(() => TiltStartPosition);
                    RaisePropertyChanged(() => PanSize);
                    RaisePropertyChanged(() => TiltSize);
                });
        }

        private async void SetStop()
        {
            await _protocolService.Main.SetProgramStopPoint().ConfigureAwait(false);

            _panStopPos = await _protocolService.Motor2.GetProgramStopPoint().ConfigureAwait(false);
            _tiltStopPos = await _protocolService.Motor3.GetProgramStopPoint().ConfigureAwait(false);

            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => PanStopPosition);
                    RaisePropertyChanged(() => TiltStopPosition);
                    RaisePropertyChanged(() => PanSize);
                    RaisePropertyChanged(() => TiltSize);
                });
        }

        private async  void SwapStartStop()
        {
            await _protocolService.Main.ReverseAllMotorsStartStopPoints().ConfigureAwait(false);

            _panStartPos = await _protocolService.Motor2.GetProgramStartPoint().ConfigureAwait(false);
            _tiltStartPos = await _protocolService.Motor3.GetProgramStartPoint().ConfigureAwait(false);
            _panStopPos = await _protocolService.Motor2.GetProgramStopPoint().ConfigureAwait(false);
            _tiltStopPos = await _protocolService.Motor3.GetProgramStopPoint().ConfigureAwait(false);

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

        private async void SetRefStart()
        {
            await _protocolService.Motor2.SetStartHere().ConfigureAwait(false);
            await _protocolService.Motor3.SetStartHere().ConfigureAwait(false);
        }

        private async void SetRefStop()
        {
            await _protocolService.Motor2.SetStopHere().ConfigureAwait(false);
            await _protocolService.Motor3.SetStopHere().ConfigureAwait(false);
        }

        private async void StartProgram()
        {
            ushort preDelay = 100;
            ushort focusTime = 100;
            var exposureTime = ExposureTime * 1000m;
            var postDelay = DelayTime * 1000m;

            if (postDelay > ushort.MaxValue)
                postDelay = 60000m;

            await _protocolService.Camera.SetFocusTime(focusTime).ConfigureAwait(false);
            await _protocolService.Camera.SetTriggerTime((uint)exposureTime).ConfigureAwait(false);
            await _protocolService.Camera.SetExposureDelayTime((ushort)postDelay).ConfigureAwait(false);

            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Panorama).ConfigureAwait(false);
            await _protocolService.Main.Start().ConfigureAwait(false);

            _overallCols = await _protocolService.Motor2.GetTravelShots().ConfigureAwait(false);
            _overallRows = await _protocolService.Motor3.GetTravelShots().ConfigureAwait(false);

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
        }

        public async Task UpdateState()
        {
            _progress = await _protocolService.Main.GetProgramPercentComplete().ConfigureAwait(false);
            _elapsedTime = await _protocolService.Main.GetRunTime().ConfigureAwait(false);
            _elapsedShots = await _protocolService.Camera.GetCurrentShots().ConfigureAwait(false);
            _overallShots = await _protocolService.Camera.GetMaxShots().ConfigureAwait(false);
            //_overallCols = await _protocolService.Motor2.GetTravelShots().ConfigureAwait(false);
            //_overallRows = await _protocolService.Motor3.GetTravelShots().ConfigureAwait(false);

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

        public async Task InitState()
        {
            _panStartPos = await _protocolService.Motor2.GetProgramStartPoint().ConfigureAwait(false);
            _tiltStartPos = await _protocolService.Motor3.GetProgramStartPoint().ConfigureAwait(false);
            _panStopPos = await _protocolService.Motor2.GetProgramStopPoint().ConfigureAwait(false);
            _tiltStopPos = await _protocolService.Motor3.GetProgramStopPoint().ConfigureAwait(false);

            _exposureTime = (decimal)await _protocolService.Camera.GetTriggerTime().ConfigureAwait(false) / 1000m;
            _delayTime = (decimal)await _protocolService.Camera.GetExposureDelayTime().ConfigureAwait(false) / 1000m;

            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => PanStartPosition);
                    RaisePropertyChanged(() => TiltStartPosition);
                    RaisePropertyChanged(() => PanStopPosition);
                    RaisePropertyChanged(() => TiltStopPosition);
                    RaisePropertyChanged(() => PanSize);
                    RaisePropertyChanged(() => TiltSize);
                    RaisePropertyChanged(() => ExposureTime);
                    RaisePropertyChanged(() => DelayTime);
                });
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
