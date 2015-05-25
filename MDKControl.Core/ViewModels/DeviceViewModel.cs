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
using System.Reactive;
using System.Threading;

namespace MDKControl.Core.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly IDevice _device;
        private readonly IMoCoBusCommService _commService;
        private readonly IMoCoBusProtocolService _protocolService;
        private CancellationTokenSource _joystickWatchdogTaskTokenSource;

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

            StartJoystickCommand = new ReactiveCommand();
            StartJoystickCommand.Subscribe(StartJoystick);

            StopJoystickCommand = new ReactiveCommand();
            StopJoystickCommand.Subscribe(StopJoystick);

            MoveJoystickCommand = new ReactiveCommand<Point>();
            MoveJoystickCommand.Sample(TimeSpan.FromMilliseconds(50)).Throttle(TimeSpan.FromMilliseconds(50)).Subscribe(MoveJoystick);
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

        public ReactiveCommand StartJoystickCommand { get; private set; }

        public ReactiveCommand StopJoystickCommand { get; private set; }

        public ReactiveCommand<Point> MoveJoystickCommand { get; private set; }

        public void StartJoystick(object unit)
        {
            Debug.WriteLine("Start Joystick");

            _joystickWatchdogTaskTokenSource = new CancellationTokenSource();
            var token = _joystickWatchdogTaskTokenSource.Token;
            Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        try 
                        {
                            await Task.Delay(300);
                            Debug.WriteLine("Trigger Watchdog!");
                            await _protocolService.Main.GetJoystickWatchdogStatus();
                        }
                        catch (Exception ex) 
                        {
                            Debug.WriteLine("Trigger Watchdog: {0}", ex);
                        }
                    }
                }, _joystickWatchdogTaskTokenSource.Token);
        }

        public void StopJoystick(object unit)
        {
            if (_joystickWatchdogTaskTokenSource == null)
                return;

            Debug.WriteLine("Stop Joystick");

            _joystickWatchdogTaskTokenSource.Cancel();
            _joystickWatchdogTaskTokenSource = null;
        }

        public async void MoveJoystick(Point point)
        {
            Debug.WriteLine("Joystick: {0}", point);
            
            try
            {
                await _protocolService.Motor1.SetContinuousSpeed(point.Z).ConfigureAwait(false);
                await _protocolService.Motor2.SetContinuousSpeed(point.X).ConfigureAwait(false);
                await _protocolService.Motor3.SetContinuousSpeed(point.Y).ConfigureAwait(false);
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("MoveJoystick: {0}", ex);
            }
        }
    }
}
