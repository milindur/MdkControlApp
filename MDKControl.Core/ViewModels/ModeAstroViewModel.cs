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
        private readonly DeviceViewModel _deviceViewModel;
        private readonly IMoCoBusProtocolService _protocolService;

        private RelayCommand _resumeProgramCommand;
        private RelayCommand _startProgramCommand;
        private RelayCommand _pauseProgramCommand;
        private RelayCommand _stopProgramCommand;

        public ModeAstroViewModel(IDispatcherHelper dispatcherHelper, DeviceViewModel deviceViewModel, IMoCoBusProtocolService protocolService)
        {
            _dispatcherHelper = dispatcherHelper;
            _deviceViewModel = deviceViewModel;
            _protocolService = protocolService;
        }

        public DeviceViewModel DeviceViewModel { get { return _deviceViewModel; } }

        public RelayCommand ResumeProgramCommand
        {
            get { return _resumeProgramCommand ?? (_resumeProgramCommand = new RelayCommand(ResumeProgram)); }
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

        private async void ResumeProgram()
        {
            await _protocolService.Main.Start().ConfigureAwait(false);

            _deviceViewModel.StartUpdateTask();
        }

        private async void StartProgram()
        {
            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Astro).ConfigureAwait(false);
            await _protocolService.Main.Start((byte)Direction, (byte)Speed).ConfigureAwait(false);

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

        private AstroDirection _direction;
        public AstroDirection Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => Direction);
                    });
            }
        }

        private AstroSpeed _speed;
        public AstroSpeed Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => Speed);
                    });
            }
        }

        public Task UpdateState()
        {
            return Task.FromResult(0);
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
