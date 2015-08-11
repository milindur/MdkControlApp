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

        public ModeSmsViewModel(IDispatcherHelper dispatcherHelper, IMoCoBusProtocolService protocolService)
        {
            _dispatcherHelper = dispatcherHelper;
            _protocolService = protocolService;
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
