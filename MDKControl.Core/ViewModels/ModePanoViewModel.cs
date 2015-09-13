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
            await _protocolService.Main.SetProgramStartPoint();

            _panStartPos = await _protocolService.Motor2.GetProgramStartPoint();
            _tiltStartPos = await _protocolService.Motor3.GetProgramStartPoint();

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
            await _protocolService.Main.SetProgramStopPoint();

            _panStopPos = await _protocolService.Motor2.GetProgramStopPoint();
            _tiltStopPos = await _protocolService.Motor3.GetProgramStopPoint();

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
            await _protocolService.Main.ReverseAllMotorsStartStopPoints();

            _panStartPos = await _protocolService.Motor2.GetProgramStartPoint();
            _tiltStartPos = await _protocolService.Motor3.GetProgramStartPoint();
            _panStopPos = await _protocolService.Motor2.GetProgramStopPoint();
            _tiltStopPos = await _protocolService.Motor3.GetProgramStopPoint();

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
            await _protocolService.Motor2.SetStartHere();
            await _protocolService.Motor3.SetStartHere();
        }

        private async void SetRefStop()
        {
            await _protocolService.Motor2.SetStopHere();
            await _protocolService.Motor3.SetStopHere();
        }

        private async void StartProgram()
        {
            ushort preDelay = 100;
            ushort focusTime = 100;
            var exposureTime = (uint)(ExposureTime * 1000m);
            var postDelay = (ushort)(DelayTime * 1000m);

            await _protocolService.Camera.SetFocusTime(focusTime);
            await _protocolService.Camera.SetTriggerTime(exposureTime);
            await _protocolService.Camera.SetExposureDelayTime(postDelay);

            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Panorama);
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
            _elapsedShots = await _protocolService.Camera.GetCurrentShots();
            _overallShots = await _protocolService.Camera.GetMaxShots();

            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => Progress);
                    RaisePropertyChanged(() => ElapsedTime);
                    RaisePropertyChanged(() => ElapsedShots);
                    RaisePropertyChanged(() => RemainingTime);
                    RaisePropertyChanged(() => RemainingShots);
                    RaisePropertyChanged(() => OverallTime);
                    RaisePropertyChanged(() => OverallShots);
                });
        }

        public async Task InitState()
        {
            _panStartPos = await _protocolService.Motor2.GetProgramStartPoint();
            _tiltStartPos = await _protocolService.Motor3.GetProgramStartPoint();
            _panStopPos = await _protocolService.Motor2.GetProgramStopPoint();
            _tiltStopPos = await _protocolService.Motor3.GetProgramStopPoint();

            _exposureTime = (decimal)await _protocolService.Camera.GetTriggerTime() / 1000m;
            _delayTime = (decimal)await _protocolService.Camera.GetExposureDelayTime() / 1000m;

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
