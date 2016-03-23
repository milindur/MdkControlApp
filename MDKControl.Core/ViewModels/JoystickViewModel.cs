﻿using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MDKControl.Core.Helpers;
using MDKControl.Core.Models;
using MDKControl.Core.Services;
using Reactive.Bindings;

namespace MDKControl.Core.ViewModels
{
    public class JoystickViewModel : ViewModelBase
    {
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly DeviceViewModel _deviceViewModel;
        private readonly IMoCoBusProtocolService _protocolService;

        private Point _joystickCurrentPoint;
        private float _sliderCurrentPoint;

        private bool _sliderOrJoystickIsRunning = false;
        private bool _joystickIsRunning = false;
        private bool _sliderIsRunning = false;

        private Task _joystickTask;
        private CancellationTokenSource _joystickTaskCancellationTokenSource;

        public JoystickViewModel(IDispatcherHelper dispatcherHelper, DeviceViewModel deviceViewModel, IMoCoBusProtocolService protocolService)
        {
            _dispatcherHelper = dispatcherHelper;
            _deviceViewModel = deviceViewModel;
            _protocolService = protocolService;

            StartJoystickCommand = new ReactiveCommand<Point>();
            StartJoystickCommand.Subscribe(StartJoystick);
            StopJoystickCommand = new ReactiveCommand();
            StopJoystickCommand.Subscribe(StopJoystick);
            MoveJoystickCommand = new ReactiveCommand<Point>();
            MoveJoystickCommand.Sample(TimeSpan.FromMilliseconds(60)).Throttle(TimeSpan.FromMilliseconds(50)).Subscribe(MoveJoystick);

            StartSliderCommand = new ReactiveCommand<float>();
            StartSliderCommand.Subscribe(StartSlider);
            StopSliderCommand = new ReactiveCommand();
            StopSliderCommand.Subscribe(StopSlider);
            MoveSliderCommand = new ReactiveCommand<float>();
            MoveSliderCommand.Sample(TimeSpan.FromMilliseconds(60)).Throttle(TimeSpan.FromMilliseconds(50)).Subscribe(MoveSlider);
        }

        public DeviceViewModel DeviceViewModel { get { return _deviceViewModel; } }

        public ReactiveCommand<Point> StartJoystickCommand { get; private set; }

        public ReactiveCommand StopJoystickCommand { get; private set; }

        public ReactiveCommand<Point> MoveJoystickCommand { get; private set; }

        public ReactiveCommand<float> StartSliderCommand { get; private set; }

        public ReactiveCommand StopSliderCommand { get; private set; }

        public ReactiveCommand<float> MoveSliderCommand { get; private set; }

        public void StartJoystick(Point point)
        {
            if (_joystickIsRunning)
                return;

            _joystickIsRunning = true;

            Debug.WriteLine($"Start Joystick X={point.X} Y={point.Y}");

            _joystickCurrentPoint = point;

            if (_sliderOrJoystickIsRunning)
                return;

            StartSliderOrJoystickTask();
        }

        public void StartSlider(float point)
        {
            if (_sliderIsRunning)
                return;

            _sliderIsRunning = true;

            Debug.WriteLine($"Start Slider Z={point}");

            _sliderCurrentPoint = point;

            if (_sliderOrJoystickIsRunning)
                return;

            StartSliderOrJoystickTask();
        }

        void StartSliderOrJoystickTask()
        {
            _sliderOrJoystickIsRunning = true;

            _joystickTaskCancellationTokenSource = new CancellationTokenSource();
            _joystickTask = Task.Run(async () =>
                {
                    var taskName = "SliderOrJoystickTask_" + Guid.NewGuid().ToString("N");
                    var token = _joystickTaskCancellationTokenSource.Token;
                    try
                    {
                        while (true)
                        {
                            token.ThrowIfCancellationRequested();

                            await Task.Delay(100, token).ConfigureAwait(false);
                            try
                            {
                                Debug.WriteLine("SliderOrJoystickTask: Trigger Watchdog!");
                                await _protocolService.Main.GetJoystickWatchdogStatus().ConfigureAwait(false);
                                await Task.Delay(20, token).ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("SliderOrJoystickTask: Exception while triggering: {0}", ex);
                            }
                            try
                            {
                                var currentJoystick = _joystickCurrentPoint;
                                var currentSlider = _sliderCurrentPoint;
                                Debug.WriteLine($"SliderOrJoystickTask: Move! X={currentJoystick.X} Y={currentJoystick.Y} Z={currentSlider}");
                                await _protocolService.Motor2.SetContinuousSpeed(currentJoystick.X).ConfigureAwait(false);
                                await Task.Delay(20, token).ConfigureAwait(false);
                                await _protocolService.Motor3.SetContinuousSpeed(currentJoystick.Y).ConfigureAwait(false);
                                await Task.Delay(20, token).ConfigureAwait(false);
                                await _protocolService.Motor1.SetContinuousSpeed(currentSlider).ConfigureAwait(false);
                                await Task.Delay(20, token).ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("SliderOrJoystickTask: Exception while moving: {0}", ex);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.WriteLine("SliderOrJoystickTask: Canceled!");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("SliderOrJoystickTask: Exception overall: {0}", ex);
                    }
                    try
                    {
                        Debug.WriteLine("SliderOrJoystickTask: Stop!");
                        await _protocolService.Motor2.SetContinuousSpeed(0).ConfigureAwait(false);
                        await Task.Delay(20).ConfigureAwait(false);
                        await _protocolService.Motor3.SetContinuousSpeed(0).ConfigureAwait(false);
                        await Task.Delay(20).ConfigureAwait(false);
                        await _protocolService.Motor1.SetContinuousSpeed(0).ConfigureAwait(false);
                        await Task.Delay(20).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("SliderOrJoystickTask: Exception while stopping: {0}", ex);
                    }

                    _joystickCurrentPoint = new Point(0, 0);
                    _sliderCurrentPoint = 0;

                    _sliderIsRunning = false;
                    _joystickIsRunning = false;
                    _sliderOrJoystickIsRunning = false;
                }, _joystickTaskCancellationTokenSource.Token);
        }

        public async void StopJoystick(object unit)
        {
            if (_joystickTaskCancellationTokenSource == null)
                return;

            Debug.WriteLine("Stop Joystick");

            _joystickCurrentPoint = new Point(0, 0);
            _joystickIsRunning = false;

            if (_sliderIsRunning)
                return;

            _joystickTaskCancellationTokenSource.Cancel();
            await _joystickTask.ConfigureAwait(false);

            _joystickTaskCancellationTokenSource.Dispose();
            _joystickTaskCancellationTokenSource = null;
            _joystickTask = null;
        }

        public async void StopSlider(object unit)
        {
            if (_joystickTaskCancellationTokenSource == null)
                return;

            Debug.WriteLine("Stop Slider");

            _sliderCurrentPoint = 0;
            _sliderIsRunning = false;

            if (_joystickIsRunning)
                return;

            _joystickTaskCancellationTokenSource.Cancel();
            await _joystickTask.ConfigureAwait(false);

            _joystickTaskCancellationTokenSource.Dispose();
            _joystickTaskCancellationTokenSource = null;
            _joystickTask = null;
        }

        public void MoveJoystick(Point point)
        {
            if (!_joystickIsRunning)
                return;
            
            Debug.WriteLine($"Move Joystick X={point.X} Y={point.Y}");

            _joystickCurrentPoint = point;
        }

        public void MoveSlider(float point)
        {
            if (!_sliderIsRunning)
                return;

            Debug.WriteLine($"Move Slider Z={point}");

            _sliderCurrentPoint = point;
        }

        public override void Cleanup()
        {
            StopJoystick(null);
            StopSlider(null);
            
            base.Cleanup();
        }
    }
}
