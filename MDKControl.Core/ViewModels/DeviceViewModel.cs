using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MDKControl.Core.Services;
using Robotics.Mobile.Core.Bluetooth.LE;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDKControl.Core.Helpers;
using Reactive.Bindings;
using MDKControl.Core.Models;
using System.Reactive.Linq;
using System.Diagnostics;

namespace MDKControl.Core.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly IDevice _device;
        private readonly IMoCoBusCommService _commService;
        private readonly IMoCoBusProtocolService _protocolService;

        private ReactiveProperty<Point> _joystickPosition = new ReactiveProperty<Point>();

        public DeviceViewModel(IDispatcherHelper dispatcherHelper, 
            IDevice device, 
            Func<IDevice, IMoCoBusCommService> moCoBusCommServiceFactory, 
            Func<IMoCoBusCommService, byte, IMoCoBusProtocolService> moCoBusProtocolServiceFactory)
        {
            _dispatcherHelper = dispatcherHelper;
            _device = device;

            _commService = moCoBusCommServiceFactory(device);
            _commService.ConnectionChanged += CommServiceOnConnectionChanged;

            _protocolService = moCoBusProtocolServiceFactory(_commService, 3);

            JoystickCommand = new ReactiveCommand<Point>();
            JoystickCommand.Sample(TimeSpan.FromMilliseconds(100)).Subscribe(Joystick);
        }

        private void CommServiceOnConnectionChanged(object sender, EventArgs e)
        {
            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => IsConnected);
                    RaisePropertyChanged(() => IsConnecting);
                    RaisePropertyChanged(() => IsDisconnected);
                });
        }

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

        public int FirmwareVersion { get; private set; }

        private RelayCommand _testCommand;

        public RelayCommand TestCommand { get { return _testCommand ?? (_testCommand = new RelayCommand(async () => await Test())); } }

        private async Task Test()
        {
            FirmwareVersion = await _protocolService.Main.GetFirmwareVersion();
            RaisePropertyChanged(() => FirmwareVersion);
        }

        public ReactiveCommand<Point> JoystickCommand { get; private set; }

        public void Joystick(Point point)
        {
            Debug.WriteLine("Joystick: {0}", point);
            
            //await _protocolService.SetJoystickWatchdog(true);
            _protocolService.Motor1.SetContinuousSpeed(point.Z).Wait();
            _protocolService.Motor2.SetContinuousSpeed(point.X).Wait();
            _protocolService.Motor3.SetContinuousSpeed(point.Y).Wait();
        }
    }
}
