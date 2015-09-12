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

        private float _progress = 0;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private int _elapsedShots = 0;

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
            get { return TimeSpan.Zero; }
        }

        public int RemainingShots
        {
            get { return 0; }
        }

        public TimeSpan OverallTime
        {
            get { return TimeSpan.Zero; }
        }

        public int OverallShots
        {
            get { return 0; }
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
        }

        private async void SetStop()
        {
            await _protocolService.Main.SetProgramStopPoint();
        }

        private async  void SwapStartStop()
        {
            await _protocolService.Main.ReverseAllMotorsStartStopPoints();
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

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
