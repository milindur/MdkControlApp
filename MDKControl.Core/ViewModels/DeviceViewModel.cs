using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.Helpers;
using MDKControl.Core.Models;
using MDKControl.Core.Services;
using Reactive.Bindings;
using Robotics.Mobile.Core.Bluetooth.LE;
using Microsoft.Practices.ServiceLocation;

namespace MDKControl.Core.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly INavigationService _navigationService;
        private readonly IMoCoBusCommService _commService;
        private readonly IMoCoBusProtocolService _protocolService;

        private readonly ModeAstroViewModel _modeAstroViewModel;
        private readonly ModePanoViewModel _modePanoViewModel;
        private readonly ModeSmsViewModel _modeSmsViewModel;
        private readonly JoystickViewModel _joystickViewModel;

        private MoCoBusProgramMode _programMode = MoCoBusProgramMode.ShootMoveShoot;
        private RelayCommand _setModeSmsCommand;
        private RelayCommand _setModePanoCommand;
        private RelayCommand _setModeAstroCommand;

        public DeviceViewModel(IDispatcherHelper dispatcherHelper, 
                               INavigationService navigationService, 
                               IDevice device, 
                               Func<IDevice, IMoCoBusCommService> moCoBusCommServiceFactory, 
                               Func<IMoCoBusCommService, byte, IMoCoBusProtocolService> moCoBusProtocolServiceFactory)
        {
            _dispatcherHelper = dispatcherHelper;
            _navigationService = navigationService;

            _commService = moCoBusCommServiceFactory(device);
            _commService.ConnectionChanged += CommServiceOnConnectionChanged;

            _protocolService = moCoBusProtocolServiceFactory(_commService, 3);

            _modeAstroViewModel = new ModeAstroViewModel(_dispatcherHelper, _protocolService);
            _modePanoViewModel = new ModePanoViewModel(_dispatcherHelper, _protocolService);
            _modeSmsViewModel = new ModeSmsViewModel(_dispatcherHelper, _protocolService);

            _joystickViewModel = new JoystickViewModel(_dispatcherHelper, _protocolService);
        }

        private async void CommServiceOnConnectionChanged(object sender, EventArgs e)
        {
            if (_commService.ConnectionState == ConnectionState.Connected)
            {
                await UpdateDeviceState().ConfigureAwait(false);
            }
            
            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => IsConnected);
                    RaisePropertyChanged(() => IsConnecting);
                    RaisePropertyChanged(() => IsDisconnected);
                });
        }

        public ModeAstroViewModel ModeAstroViewModel { get { return _modeAstroViewModel; } }
        public ModePanoViewModel ModePanoViewModel { get { return _modePanoViewModel; } }
        public ModeSmsViewModel ModeSmsViewModel { get { return _modeSmsViewModel; } }

        public JoystickViewModel JoystickViewModel { get { return _joystickViewModel; } }

        public bool IsConnected
        {
            get
            {
                return _commService.ConnectionState == ConnectionState.Connecting
                    || _commService.ConnectionState == ConnectionState.Connected;
            }
            set
            {
                if (_commService.ConnectionState == ConnectionState.Disconnected && value)
                {
                    _commService.Connect();
                }
                else if (_commService.ConnectionState != ConnectionState.Disconnected && !value)
                {
                    _commService.Disconnect();
                }
            }
        }

        public bool IsConnecting { get { return _commService.ConnectionState == ConnectionState.Connecting; } }

        public bool IsDisconnected { get { return _commService.ConnectionState == ConnectionState.Disconnected; } }

        public MoCoBusProgramMode ProgramMode
        {
            get { return _programMode; }
            set
            {
                _programMode = value;
                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => ProgramMode);
                    });
            }
        }

        public int FirmwareVersion { get; private set; }

        public RelayCommand SetModeSmsCommand
        {
            get { return _setModeSmsCommand ?? (_setModeSmsCommand = new RelayCommand(SetModeSms)); }
        }

        public RelayCommand SetModePanoCommand
        {
            get { return _setModePanoCommand ?? (_setModePanoCommand = new RelayCommand(SetModePano)); }
        }

        public RelayCommand SetModeAstroCommand
        {
            get { return _setModeAstroCommand ?? (_setModeAstroCommand = new RelayCommand(SetModeAstro)); }
        }

        private async Task UpdateDeviceState()
        {
            ProgramMode = await _protocolService.Main.GetProgramMode().ConfigureAwait(false);
        }

        private async void SetModeSms()
        {
            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.ShootMoveShoot);
            await UpdateDeviceState();
        }

        private async void SetModePano()
        {
            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Panorama);
            await UpdateDeviceState();
        }

        private async void SetModeAstro()
        {
            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Astro);
            await UpdateDeviceState();
        }

        public override void Cleanup()
        {
            ModeAstroViewModel.Cleanup();
            ModePanoViewModel.Cleanup();
            ModeSmsViewModel.Cleanup();
            JoystickViewModel.Cleanup();

            IsConnected = false;

            base.Cleanup();
        }
    }
}
