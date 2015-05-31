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

namespace MDKControl.Core.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly INavigationService _navigationService;
        private readonly IMoCoBusCommService _commService;
        private readonly IMoCoBusProtocolService _protocolService;

        private Point _joystickCurrentPoint;

        private bool _joystickIsRunning = false;
        private Task _joystickTask;
        private CancellationTokenSource _joystickTaskCancellationTokenSource;

        private RelayCommand _testCommand;

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

            StartJoystickCommand = new ReactiveCommand<Point>();
            StartJoystickCommand.Subscribe(StartJoystick);

            StopJoystickCommand = new ReactiveCommand();
            StopJoystickCommand.Subscribe(StopJoystick);

            MoveJoystickCommand = new ReactiveCommand<Point>();
            MoveJoystickCommand.Sample(TimeSpan.FromMilliseconds(100)).Throttle(TimeSpan.FromMilliseconds(80)).Subscribe(MoveJoystick);
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

        public ReactiveCommand<Point> StartJoystickCommand { get; private set; }

        public ReactiveCommand StopJoystickCommand { get; private set; }

        public ReactiveCommand<Point> MoveJoystickCommand { get; private set; }

        public RelayCommand TestCommand
        {
            get { return _testCommand ?? (_testCommand = new RelayCommand(Test)); }
        }

        public void StartJoystick(Point point)
        {
            if (_joystickIsRunning)
                return;

            _joystickIsRunning = true;
            
            Debug.WriteLine("Start Joystick");

            _joystickCurrentPoint = point;

            _joystickTaskCancellationTokenSource = new CancellationTokenSource();
            _joystickTask = Task.Factory.StartNew(async () =>
                {
                    var token = _joystickTaskCancellationTokenSource.Token;

                    try
                    {
                        while (true)
                        {
                            await Task.Delay(100, token);
                            token.ThrowIfCancellationRequested();

                            try
                            {
                                Debug.WriteLine("MoveJoystickTask: Trigger Watchdog!");
                                await _protocolService.Main.GetJoystickWatchdogStatus();
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("MoveJoystickTask: {0}", ex);
                            }

                            try
                            {
                                Debug.WriteLine("MoveJoystickTask: Move!");
                                var currentPoint = _joystickCurrentPoint;
                                await _protocolService.Motor2.SetContinuousSpeed(currentPoint.X).ConfigureAwait(false);
                                await _protocolService.Motor3.SetContinuousSpeed(currentPoint.Y).ConfigureAwait(false);
                                await _protocolService.Motor1.SetContinuousSpeed(currentPoint.Z).ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("MoveJoystickTask: {0}", ex);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        await _protocolService.Motor2.SetContinuousSpeed(0).ConfigureAwait(false);
                        await _protocolService.Motor3.SetContinuousSpeed(0).ConfigureAwait(false);
                        await _protocolService.Motor1.SetContinuousSpeed(0).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("StopJoystick: {0}", ex);
                    }

                    _joystickIsRunning = false;
                }, _joystickTaskCancellationTokenSource.Token);
        }

        public void StopJoystick(object unit)
        {
            if (_joystickTaskCancellationTokenSource == null)
                return;

            Debug.WriteLine("Stop Joystick");

            _joystickTaskCancellationTokenSource.Cancel();
            _joystickTaskCancellationTokenSource = null;

            _joystickTask.Wait();
            _joystickTask = null;
        }

        public void MoveJoystick(Point point)
        {
            if (!_joystickIsRunning)
                return;

            _joystickCurrentPoint = point;
        }

        private void Test()
        {
            _navigationService.NavigateTo(ViewModelLocator.RunningViewKey, this);
        }


        public override void Cleanup()
        {
            StopJoystick(null);
            IsConnected = false;

            base.Cleanup();
        }
    }
}
