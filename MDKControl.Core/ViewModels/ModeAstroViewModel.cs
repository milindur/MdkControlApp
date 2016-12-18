using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDKControl.Core.Helpers;
using MDKControl.Core.Models;
using MDKControl.Core.Services;
using Xamarin;

namespace MDKControl.Core.ViewModels
{
    public class ModeAstroViewModel : ViewModelBase, IState
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

        public DeviceViewModel DeviceViewModel => _deviceViewModel;

        public RelayCommand ResumeProgramCommand => _resumeProgramCommand ?? (_resumeProgramCommand = new RelayCommand(ResumeProgram));

        private bool _startProgramRunning;
        public RelayCommand StartProgramCommand
        {
            get
            {
                return _startProgramCommand ?? (_startProgramCommand = new RelayCommand(async () =>
                    {
                        try
                        {
                            _startProgramRunning = true;
                            _startProgramCommand.RaiseCanExecuteChanged();

                            await StartProgram();
                        }
                        finally
                        {
                            _startProgramRunning = false;
                            _startProgramCommand.RaiseCanExecuteChanged();
                        }
                    }, () => !_startProgramRunning));
            }
        }

        public RelayCommand PauseProgramCommand => _pauseProgramCommand ?? (_pauseProgramCommand = new RelayCommand(PauseProgram));

        public RelayCommand StopProgramCommand => _stopProgramCommand ?? (_stopProgramCommand = new RelayCommand(StopProgram));

        private async void ResumeProgram()
        {
            try
            {
                await _protocolService.Main.Start().ConfigureAwait(false);

                _deviceViewModel.StartUpdateTask();
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        private async Task StartProgram()
        {
            try
            {
                await _protocolService.Main.SetProgramMode(MoCoBusProgramMode.Astro).ConfigureAwait(false);
                if (GearType != null)
                {
                    if (GearType.Value == Models.GearType.MdkV5 && Motors == Motors.MotorSlider)
                    {
                        await _protocolService.Main.StartAstro(Direction, Speed).ConfigureAwait(false);
                    }
                    else
                    {
                        await _protocolService.Main.StartAstro(Motors, Direction, Speed, GearType.Value).ConfigureAwait(false);
                    }
                }
                else if (GearReduction != null)
                {
                    await _protocolService.Main.StartAstro(Motors, Direction, Speed, GearReduction.Value).ConfigureAwait(false);
                }
                else
                {
                    await _protocolService.Main.StartAstro(Direction, Speed).ConfigureAwait(false);
                }

                _deviceViewModel.StartUpdateTask();
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        private async void PauseProgram()
        {
            try
            {
                await _protocolService.Main.Pause().ConfigureAwait(false);
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        private async void StopProgram()
        {
            try
            {
                await _protocolService.Main.Stop().ConfigureAwait(false);
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
            try
            {
                await _deviceViewModel.StopUpdateTask().ConfigureAwait(false);
                await _deviceViewModel.UpdateState().ConfigureAwait(false);
            }
            catch (TimeoutException toe)
            {
                Insights.Report(toe);
            }
        }

        private Motors _motors;

        public Motors Motors
        {
            get
            {
                return _motors;
            }
            set
            {
                _motors = value;
                _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => Motors);
                });
            }
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

        private GearType? _gearType;

        public GearType? GearType
        {
            get
            {
                return _gearType;
            }
            set
            {
                _gearType = value;
                if (_gearType != null) _gearReduction = null;
                _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => GearType);
                    RaisePropertyChanged(() => GearReduction);
                });
            }
        }

        private float? _gearReduction;

        public float? GearReduction
        {
            get
            {
                return _gearReduction;
            }
            set
            {
                _gearReduction = value;
                if (_gearReduction != null) _gearType = null;
                _dispatcherHelper.RunOnUIThread(() =>
                {
                    RaisePropertyChanged(() => GearType);
                    RaisePropertyChanged(() => GearReduction);
                });
            }
        }

        public Task SaveState()
        {
            return Task.FromResult(0);
        }

        public Task UpdateState()
        {
            return Task.FromResult(0);
        }

        public Task InitState()
        {
            return Task.FromResult(0);
        }
    }
}
