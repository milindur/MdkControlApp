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

        private Point _joystickCurrentPoint;

        private bool _joystickIsRunning = false;
        private Task _joystickTask;
        private CancellationTokenSource _joystickTaskCancellationTokenSource;

        private RelayCommand _setStartCommand;
        private RelayCommand _setStopCommand;
        private RelayCommand _swapStartStopCommand;
        private RelayCommand _setRefStartCommand;
        private RelayCommand _setRefStopCommand;
        private RelayCommand _startProgramCommand;
        private RelayCommand _pauseProgramCommand;
        private RelayCommand _stopProgramCommand;
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
                        Debug.WriteLine("MoveJoystickTask: {0}", ex);
                    }

                    _joystickIsRunning = false;
                }, _joystickTaskCancellationTokenSource.Token);
        }

        public async void StopJoystick(object unit)
        {
            if (_joystickTaskCancellationTokenSource == null)
                return;

            Debug.WriteLine("Stop Joystick");

            _joystickTaskCancellationTokenSource.Cancel();
            await _joystickTask;

            _joystickTaskCancellationTokenSource = null;
            _joystickTask = null;
        }

        public void MoveJoystick(Point point)
        {
            if (!_joystickIsRunning)
                return;

            _joystickCurrentPoint = point;
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
            //_navigationService.NavigateTo(ViewModelLocator.RunningViewKey, this);

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

        private async void SetModeSms()
        {
            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.ShootMoveShoot);
        }

        private async void SetModePano()
        {
            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Panorama);
        }

        private async void SetModeAstro()
        {
            await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Astro);
        }

        public override void Cleanup()
        {
            StopJoystick(null);
            IsConnected = false;

            base.Cleanup();
        }
    }
}
