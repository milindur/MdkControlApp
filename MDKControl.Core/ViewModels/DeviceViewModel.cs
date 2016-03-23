﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.Helpers;
using MDKControl.Core.Models;
using MDKControl.Core.Services;
using Robotics.Mobile.Core.Bluetooth.LE;
using Xamarin;

namespace MDKControl.Core.ViewModels
{
    public class DeviceViewModel : ViewModelBase, IState
    {
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly INavigationService _navigationService;
        private readonly IMoCoBusCommService _commService;
        private readonly IMoCoBusProtocolService _protocolService;

        private readonly ModeAstroViewModel _modeAstroViewModel;
        private readonly ModePanoViewModel _modePanoViewModel;
        private readonly ModeSmsViewModel _modeSmsViewModel;
        private readonly JoystickViewModel _joystickViewModel;

        private MoCoBusProgramMode _programMode = MoCoBusProgramMode.Invalid;
        private MoCoBusRunStatus _runStatus;
        private RelayCommand _setModeSmsCommand;
        private RelayCommand _setModePanoCommand;
        private RelayCommand _setModeAstroCommand;

        private Task _updateTask;
        private CancellationTokenSource _updateTaskCancellationTokenSource;

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

            _modeAstroViewModel = new ModeAstroViewModel(_dispatcherHelper, this, _protocolService);
            _modePanoViewModel = new ModePanoViewModel(_dispatcherHelper, this, _protocolService);
            _modeSmsViewModel = new ModeSmsViewModel(_dispatcherHelper, this, _protocolService);

            _joystickViewModel = new JoystickViewModel(_dispatcherHelper, this, _protocolService);
        }

        private async void CommServiceOnConnectionChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("CommServiceOnConnectionChanged: {0}", _commService.ConnectionState);
            
            _programMode = MoCoBusProgramMode.Invalid;
            if (_commService.ConnectionState == ConnectionState.Connected)
            {
                await UpdateState().ConfigureAwait(false);
            }
            else
            {                
                await StopUpdateTask().ConfigureAwait(false);
            }

            _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => IsConnected);
                    RaisePropertyChanged(() => IsConnecting);
                    RaisePropertyChanged(() => IsDisconnected);
                    RaisePropertyChanged(() => ProgramMode);
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
                Debug.WriteLine("Setting ProgramMode");
                _programMode = value;
                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        Debug.WriteLine("RaisePropertyChanged ProgramMode");
                        RaisePropertyChanged(() => ProgramMode);
                    });
            }
        }

        public MoCoBusRunStatus RunStatus
        {
            get { return _runStatus; }
            set
            {
                Debug.WriteLine("Setting RunStatus");
                _runStatus = value;
                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        Debug.WriteLine("RaisePropertyChanged RunStatus");
                        RaisePropertyChanged(() => RunStatus);
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

        public Task InitState()
        {
            return Task.FromResult(0);
        }

        public async Task SaveState()
        {
            Debug.WriteLine("SaveState");

            try
            {
                switch (ProgramMode)
                {
                    case MoCoBusProgramMode.ShootMoveShoot:
                        await ModeSmsViewModel.SaveState().ConfigureAwait(false);
                        break;
                    case MoCoBusProgramMode.Panorama:
                        await ModePanoViewModel.SaveState().ConfigureAwait(false);
                        break;
                    case MoCoBusProgramMode.Astro:
                        await ModeAstroViewModel.SaveState().ConfigureAwait(false);
                        break;
                }
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe, Insights.Severity.Error);
            }
        }

        public async Task UpdateState()
        {
            Debug.WriteLine("UpdateState");

            try
            {
                _runStatus = await _protocolService.Main.GetRunStatus().ConfigureAwait(false);

                var tmpProgramMode = _programMode;
                _programMode = await _protocolService.Main.GetProgramMode().ConfigureAwait(false);
                if (tmpProgramMode != _programMode)
                {
                    switch (_programMode)
                    {
                        case MoCoBusProgramMode.ShootMoveShoot:
                            System.Diagnostics.Debug.WriteLine("Updatestate: Call ModeSmsViewModel.InitState()");
                            await ModeSmsViewModel.InitState().ConfigureAwait(false);
                            break;
                        case MoCoBusProgramMode.Panorama:
                            System.Diagnostics.Debug.WriteLine("Updatestate: Call ModePanoViewModel.InitState()");
                            await ModePanoViewModel.InitState().ConfigureAwait(false);
                            break;
                        case MoCoBusProgramMode.Astro:
                            System.Diagnostics.Debug.WriteLine("Updatestate: Call ModeAstroViewModel.InitState()");
                            await ModeAstroViewModel.InitState().ConfigureAwait(false);
                            break;
                    }
                }

                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => ProgramMode);
                        RaisePropertyChanged(() => RunStatus);
                    });
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe, Insights.Severity.Error);
            }
        }

        private async void SetModeSms()
        {
            try
            {
                await SaveState().ConfigureAwait(false);
                await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.ShootMoveShoot).ConfigureAwait(false);
                await UpdateState().ConfigureAwait(false);
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe, Insights.Severity.Error);
            }
        }

        private async void SetModePano()
        {
            try
            {
                await SaveState().ConfigureAwait(false);
                await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Panorama).ConfigureAwait(false);
                await UpdateState().ConfigureAwait(false);
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe, Insights.Severity.Error);
            }
        }

        private async void SetModeAstro()
        {
            try
            {
                await SaveState().ConfigureAwait(false);
                await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Astro).ConfigureAwait(false);
                await UpdateState().ConfigureAwait(false);
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe, Insights.Severity.Error);
            }
        }

        public void StartUpdateTask()
        {
            if (_updateTask != null)
                return;
            
            _updateTaskCancellationTokenSource = new CancellationTokenSource();
            _updateTask = Task.Run(async () =>
                {
                    var token = _updateTaskCancellationTokenSource.Token;
                    try
                    {
                        while (true)
                        {
                            await Task.Delay(500, token).ConfigureAwait(false);
                            token.ThrowIfCancellationRequested();

                            try
                            {
                                Debug.WriteLine("UpdateTask: Updating...");
                                await UpdateState().ConfigureAwait(false);
                                switch (ProgramMode)
                                {
                                    case MoCoBusProgramMode.ShootMoveShoot:
                                        await ModeSmsViewModel.UpdateState().ConfigureAwait(false);
                                        break;
                                    case MoCoBusProgramMode.Panorama:
                                        await ModePanoViewModel.UpdateState().ConfigureAwait(false);
                                        break;
                                    case MoCoBusProgramMode.Astro:
                                        await ModeAstroViewModel.UpdateState().ConfigureAwait(false);
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("UpdateTask: {0}", ex);
                            }
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        Debug.WriteLine("UpdateTask: outer operation canceled {0}", ex);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("UpdateTask: {0}", ex);
                    }
                }, _updateTaskCancellationTokenSource.Token);
        }

        public async Task StopUpdateTask()
        {
            if (_updateTaskCancellationTokenSource == null)
                return;

            Debug.WriteLine("Stop UpdateTask");

            _updateTaskCancellationTokenSource.Cancel();
            await _updateTask.ConfigureAwait(false);

            _updateTaskCancellationTokenSource.Dispose();
            _updateTaskCancellationTokenSource = null;
            _updateTask = null;
        }

        public override void Cleanup()
        {
            Debug.WriteLine("DeviceViewModel.Cleanup: StopUpdateTask");
            StopUpdateTask().Wait();
            Debug.WriteLine("DeviceViewModel.Cleanup: StopUpdateTask done");

            Debug.WriteLine("DeviceViewModel.Cleanup: Mode*ViewModel");
            ModeAstroViewModel.Cleanup();
            ModePanoViewModel.Cleanup();
            ModeSmsViewModel.Cleanup();
            Debug.WriteLine("DeviceViewModel.Cleanup: Mode*ViewModel done");

            Debug.WriteLine("DeviceViewModel.Cleanup: JoystickViewModel");
            JoystickViewModel.Cleanup();
            Debug.WriteLine("DeviceViewModel.Cleanup: JoystickViewModel done");

            base.Cleanup();
        }
    }
}
