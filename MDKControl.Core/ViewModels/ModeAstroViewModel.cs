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
    public class ModeAstroViewModel : ViewModelBase
    {
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly IMoCoBusProtocolService _protocolService;

        private RelayCommand _startProgramNorthCommand;
        private RelayCommand _startProgramSouthCommand;
        private RelayCommand _pauseProgramCommand;
        private RelayCommand _stopProgramCommand;

        public ModeAstroViewModel(IDispatcherHelper dispatcherHelper, DeviceViewModel deviceViewModel, IMoCoBusProtocolService protocolService)
        {
            _dispatcherHelper = dispatcherHelper;
            _protocolService = protocolService;
        }

        public RelayCommand StartProgramNorthCommand
        {
            get { return _startProgramNorthCommand ?? (_startProgramNorthCommand = new RelayCommand(StartProgramNorth)); }
        }

        public RelayCommand StartProgramSouthCommand
        {
            get { return _startProgramSouthCommand ?? (_startProgramSouthCommand = new RelayCommand(StartProgramSouth)); }
        }

        public RelayCommand PauseProgramCommand
        {
            get { return _pauseProgramCommand ?? (_pauseProgramCommand = new RelayCommand(PauseProgram)); }
        }

        public RelayCommand StopProgramCommand
        {
            get { return _stopProgramCommand ?? (_stopProgramCommand = new RelayCommand(StopProgram)); }
        }

        private async void StartProgramNorth()
        {
            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Astro);
            await _protocolService.Main.Start(0);
        }

        private async void StartProgramSouth()
        {
            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Astro);
            await _protocolService.Main.Start(1);
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
