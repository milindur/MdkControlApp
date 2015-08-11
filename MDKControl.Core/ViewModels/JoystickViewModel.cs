using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MDKControl.Core.Helpers;
using MDKControl.Core.Models;
using MDKControl.Core.Services;
using Reactive.Bindings;
using System.Diagnostics;

namespace MDKControl.Core.ViewModels
{
    public class JoystickViewModel : ViewModelBase
    {
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly IMoCoBusProtocolService _protocolService;

        private Point _joystickCurrentPoint;

        private bool _joystickIsRunning = false;
        private Task _joystickTask;
        private CancellationTokenSource _joystickTaskCancellationTokenSource;

        public JoystickViewModel(IDispatcherHelper dispatcherHelper, IMoCoBusProtocolService protocolService)
        {
            _dispatcherHelper = dispatcherHelper;

            _protocolService = protocolService;

            StartJoystickCommand = new ReactiveCommand<Point>();
            StartJoystickCommand.Subscribe(StartJoystick);

            StopJoystickCommand = new ReactiveCommand();
            StopJoystickCommand.Subscribe(StopJoystick);

            MoveJoystickCommand = new ReactiveCommand<Point>();
            MoveJoystickCommand.Sample(TimeSpan.FromMilliseconds(60)).Throttle(TimeSpan.FromMilliseconds(50)).Subscribe(MoveJoystick);
        }

        public ReactiveCommand<Point> StartJoystickCommand { get; private set; }

        public ReactiveCommand StopJoystickCommand { get; private set; }

        public ReactiveCommand<Point> MoveJoystickCommand { get; private set; }

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
    }
}
