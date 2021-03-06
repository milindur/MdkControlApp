using System;
using Foundation;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.Models;
using MDKControl.Core.ViewModels;
using MDKControl.iOS.Helpers;
using Microsoft.Practices.ServiceLocation;
using UIKit;

namespace MDKControl.iOS
{
    internal partial class ModeSmsViewController : SubDeviceTableViewControllerBase, INavigationTarget
    {
        private Binding _runStatusBinding;

        private Binding _exposureTimeBinding;
        private Binding _preDelayTimeBinding;
        private Binding _delayTimeBinding;
        private Binding _intervalTimeBinding;
        private Binding _durationTimeBinding;
        private Binding _maxShotsBinding;

        private Binding _sliderStartPosBinding;
        private Binding _sliderStopPosBinding;
        private Binding _panStartPosBinding;
        private Binding _panStopPosBinding;
        private Binding _tiltStartPosBinding;
        private Binding _tiltStopPosBinding;

        private bool _navigatedToStatusView;

        private bool _editingPreDelay;
        private bool _editingExposure;
        private bool _editingPostDelay;
        private bool _editingInterval;
        private bool _editingDuration;
        private bool _editingShots;

        public ModeSmsViewController(IntPtr handle)
            : base(handle, MoCoBusProgramMode.ShootMoveShoot, AppDelegate.ModeSmsViewKey)
        {
        }

        public object NavigationParameter { get; set; }

        public ModeSmsViewModel Vm { get; private set; }

        public override DeviceViewModel DeviceVm => Vm.DeviceViewModel;

        public override void ViewDidLoad()
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsViewController ViewDidLoad");

            Vm = (ModeSmsViewModel)NavigationParameter;

            Vm.PropertyChanged += (s, e) =>
            {
            };
            SwapStartStopButton.TouchUpInside += (s, e) =>
            {
            };

            var tableView = (UITableView)View;
            tableView.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            tableView.BackgroundView = null;
            tableView.AllowsSelection = true;

            PreDelayValueLabel.Text = $"{0.1f:F1}s"; 
            
            ExposureValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (decimal)ExposureValuePickerTableViewCell.Model.SelectedTime.TotalSeconds;
                if (Vm.ExposureTime != t)
                    Vm.ExposureTime = t;
            };

            PreDelayValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (decimal)PreDelayValuePickerTableViewCell.Model.SelectedTime.TotalSeconds;
                if (Vm.PreDelayTime != t)
                    Vm.PreDelayTime = t;
            };

            PostDelayValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (decimal)PostDelayValuePickerTableViewCell.Model.SelectedTime.TotalSeconds;
                if (Vm.DelayTime != t)
                    Vm.DelayTime = t;
            };

            IntervalValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (decimal)IntervalValuePickerTableViewCell.Model.SelectedTime.TotalSeconds;
                if (Vm.IntervalTime != t)
                    Vm.IntervalTime = t;
            };

            DurationValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (decimal)DurationValuePickerTableViewCell.Model.SelectedTime.TotalSeconds;
                if (Vm.DurationTime != t)
                    Vm.DurationTime = t;
            };

            ShotsValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
                {
                    var t = (ushort)ShotsValuePickerTableViewCell.Model.SelectedNumber;
                    if (Vm.MaxShots != t)
                        Vm.MaxShots = t;
                };

            SwapStartStopButton.SetCommand(
                "TouchUpInside",
                Vm.SwapStartStopCommand);

            StartButton.Clicked += (sender, e) => 
                {
                    _navigatedToStatusView = true;
                    Vm.StartProgramCommand.Execute(null);
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo(AppDelegate.ModeSmsStatusViewKey, Vm);
                };

            SetupBindings();
            
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsViewController ViewWillAppear");

            _navigatedToStatusView = false;

            SetupBindings();

            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsViewController ViewWillDisappear");

            DetachBindings();

            base.ViewWillDisappear(animated);
        }

        protected override void SetupBindings()
        {
            DetachBindings();

            base.SetupBindings();

            _exposureTimeBinding = this.SetBinding(() => Vm.ExposureTime).WhenSourceChanges(() => 
            {
                ExposureValueLabel.Text = $"{Vm.ExposureTime:F1}s";
                if ((decimal)ExposureValuePickerTableViewCell.Model.SelectedTime.TotalSeconds != Vm.ExposureTime)
                {
                    ExposureValuePickerTableViewCell.Model.SelectedTime = TimeSpan.FromSeconds((double)Vm.ExposureTime);
                }
            });
            _exposureTimeBinding.ForceUpdateValueFromSourceToTarget();

            _preDelayTimeBinding = this.SetBinding(() => Vm.PreDelayTime).WhenSourceChanges(() =>
            {
                PreDelayValueLabel.Text = $"{Vm.PreDelayTime:F1}s";
                if ((decimal)PreDelayValuePickerTableViewCell.Model.SelectedTime.TotalSeconds != Vm.PreDelayTime)
                {
                    PreDelayValuePickerTableViewCell.Model.SelectedTime = TimeSpan.FromSeconds((double)Vm.PreDelayTime);
                }
            });
            _preDelayTimeBinding.ForceUpdateValueFromSourceToTarget();

            _delayTimeBinding = this.SetBinding(() => Vm.DelayTime).WhenSourceChanges(() => 
            {
                PostDelayValueLabel.Text = $"{Vm.DelayTime:F1}s";
                if ((decimal)PostDelayValuePickerTableViewCell.Model.SelectedTime.TotalSeconds != Vm.DelayTime)
                {
                    PostDelayValuePickerTableViewCell.Model.SelectedTime = TimeSpan.FromSeconds((double)Vm.DelayTime);
                }
            });
            _delayTimeBinding.ForceUpdateValueFromSourceToTarget();

            _intervalTimeBinding = this.SetBinding(() => Vm.IntervalTime).WhenSourceChanges(() => 
            {
                IntervalValueLabel.Text = $"{Vm.IntervalTime:F1}s";
                if ((decimal)IntervalValuePickerTableViewCell.Model.SelectedTime.TotalSeconds != Vm.IntervalTime)
                {
                    IntervalValuePickerTableViewCell.Model.SelectedTime = TimeSpan.FromSeconds((double)Vm.IntervalTime);
                }
            });
            _intervalTimeBinding.ForceUpdateValueFromSourceToTarget();

            _durationTimeBinding = this.SetBinding(() => Vm.DurationTime).WhenSourceChanges(() => 
            {
                DurationValueLabel.Text = $"{(int) (Vm.DurationTime/60)}:{(int) Vm.DurationTime%60:00}m";
                if ((decimal)DurationValuePickerTableViewCell.Model.SelectedTime.TotalSeconds != Vm.DurationTime)
                {
                    DurationValuePickerTableViewCell.Model.SelectedTime = TimeSpan.FromSeconds((double)Vm.DurationTime);
                }
            });
            _durationTimeBinding.ForceUpdateValueFromSourceToTarget();

            _maxShotsBinding = this.SetBinding(() => Vm.MaxShots).WhenSourceChanges(() => 
            {
                ShotsValueLabel.Text = $"{Vm.MaxShots}";
                if (ShotsValuePickerTableViewCell.Model.SelectedNumber != Vm.MaxShots)
                {
                    ShotsValuePickerTableViewCell.Model.SelectedNumber = Vm.MaxShots;
                }
            });
            _maxShotsBinding.ForceUpdateValueFromSourceToTarget();

            _sliderStartPosBinding = this.SetBinding(() => Vm.SliderStartPosition).WhenSourceChanges(() => 
            {
                SliderStartPosValueLabel.Text = $"{Vm.SliderStartPosition}";
            });
            _sliderStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _sliderStopPosBinding = this.SetBinding(() => Vm.SliderStopPosition).WhenSourceChanges(() => 
            {
                SliderStopPosValueLabel.Text = $"{Vm.SliderStopPosition}";
            });
            _sliderStopPosBinding.ForceUpdateValueFromSourceToTarget();

            _panStartPosBinding = this.SetBinding(() => Vm.PanStartPosition).WhenSourceChanges(() => 
            {
                PanStartPosValueLabel.Text = $"{(double) Vm.PanStartPosition/(190*200*16)*360:F1}°";
            });
            _panStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _panStopPosBinding = this.SetBinding(() => Vm.PanStopPosition).WhenSourceChanges(() => 
            {
                PanStopPosValueLabel.Text = $"{(double) Vm.PanStopPosition/(190*200*16)*360:F1}°";
            });
            _panStopPosBinding.ForceUpdateValueFromSourceToTarget();

            _tiltStartPosBinding = this.SetBinding(() => Vm.TiltStartPosition).WhenSourceChanges(() => 
            {
                TiltStartPosValueLabel.Text = $"{(double) Vm.TiltStartPosition/(190*200*16)*360:F1}°";
            });
            _tiltStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _tiltStopPosBinding = this.SetBinding(() => Vm.TiltStopPosition).WhenSourceChanges(() => 
            {
                TiltStopPosValueLabel.Text = $"{(double) Vm.TiltStopPosition/(190*200*16)*360:F1}°";
            });
            _tiltStopPosBinding.ForceUpdateValueFromSourceToTarget();

            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus).WhenSourceChanges(() => 
            {
                var nav = ServiceLocator.Current.GetInstance<INavigationService>();

                System.Diagnostics.Debug.WriteLine($"ModeSmsViewController RunStatusBinding: CurrentPageKey={nav.CurrentPageKey}, RunStatus={DeviceVm.RunStatus}, _navigatedToStatusView={_navigatedToStatusView}");

                if (nav.CurrentPageKey != AppDelegate.ModeSmsViewKey)
                    return;
                if (DeviceVm.RunStatus != MoCoBusRunStatus.Stopped && nav.CurrentPageKey != AppDelegate.ModeSmsStatusViewKey && !_navigatedToStatusView && !DeviceVm.IsUpdateTaskRunning)
                {
                    System.Diagnostics.Debug.WriteLine($"ModeSmsViewController RunStatusBinding: Start update task and navigate to status view");
                    _navigatedToStatusView = true;
                    DeviceVm.StartUpdateTask();
                    nav.NavigateTo(AppDelegate.ModeSmsStatusViewKey, Vm);
                }
            });
            _runStatusBinding.ForceUpdateValueFromSourceToTarget();
        }

        protected override void DetachBindings()
        {
            _exposureTimeBinding?.Detach();
            _exposureTimeBinding = null;

            _preDelayTimeBinding?.Detach();
            _preDelayTimeBinding = null;

            _delayTimeBinding?.Detach();
            _delayTimeBinding = null;

            _intervalTimeBinding?.Detach();
            _intervalTimeBinding = null;

            _durationTimeBinding?.Detach();
            _durationTimeBinding = null;

            _maxShotsBinding?.Detach();
            _maxShotsBinding = null;

            _sliderStartPosBinding?.Detach();
            _sliderStartPosBinding = null;

            _sliderStopPosBinding?.Detach();
            _sliderStopPosBinding = null;

            _panStartPosBinding?.Detach();
            _panStartPosBinding = null;

            _panStopPosBinding?.Detach();
            _panStopPosBinding = null;

            _tiltStartPosBinding?.Detach();
            _tiltStartPosBinding = null;

            _tiltStopPosBinding?.Detach();
            _tiltStopPosBinding = null;

            _runStatusBinding?.Detach();
            _runStatusBinding = null;

            base.DetachBindings();
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            switch (indexPath.Section)
            {
                case 1:
                    {
                        switch (indexPath.Row)
                        {
                            case 1: // pre-delay picker                                
                                if (_editingPreDelay)
                                {
                                    return 219;
                                }
                                else
                                {
                                    return 0;
                                }
                            case 3: // exposure picker
                                if (_editingExposure)
                                {
                                    return 219;
                                }
                                else
                                {
                                    return 0;
                                }
                            case 5: // post-delay picker
                                if (_editingPostDelay)
                                {
                                    return 219;
                                }
                                else
                                {
                                    return 0;
                                }
                            case 7: // interval picker
                                if (_editingInterval)
                                {
                                    return 219;
                                }
                                else
                                {
                                    return 0;
                                }
                            case 9: // duration picker
                                if (_editingDuration)
                                {
                                    return 219;
                                }
                                else
                                {
                                    return 0;
                                }
                            case 11: // shots picker
                                if (_editingShots)
                                {
                                    return 219;
                                }
                                else
                                {
                                    return 0;
                                }
                        }
                    }
                    break;
            }

            return base.GetHeightForRow(tableView, indexPath);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            System.Diagnostics.Debug.WriteLine(tableView.CellAt(indexPath));

            tableView.BeginUpdates();

            switch (indexPath.Section)
            {
                case 0:
                    {
                        switch (indexPath.Row)
                        {
                            case 0: // start position
                                ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo(
                                    AppDelegate.JoystickViewKey, 
                                    new JoystickNavigationHelper(Vm.DeviceViewModel.JoystickViewModel, () =>
                                        {
                                            Vm.SetStartCommand.Execute(null);
                                        }));
                                break;
                            case 1: // stop position
                                ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo(
                                    AppDelegate.JoystickViewKey, 
                                    new JoystickNavigationHelper(Vm.DeviceViewModel.JoystickViewModel, () =>
                                        {
                                            Vm.SetStopCommand.Execute(null);
                                        }));
                                break;
                        }
                    }
                    break;
                case 1:
                    {
                        switch (indexPath.Row)
                        {
                            case 0: // pre-delay
                                _editingPreDelay = !_editingPreDelay;
                                tableView.CellAt(NSIndexPath.FromRowSection(indexPath.Row + 1, indexPath.Section)).Hidden = !_editingPreDelay;
                                break;
                            case 2: // exposure
                                _editingExposure = !_editingExposure;
                                tableView.CellAt(NSIndexPath.FromRowSection(indexPath.Row + 1, indexPath.Section)).Hidden = !_editingExposure;
                                break;
                            case 4: // post-delay
                                _editingPostDelay = !_editingPostDelay;
                                tableView.CellAt(NSIndexPath.FromRowSection(indexPath.Row + 1, indexPath.Section)).Hidden = !_editingPostDelay;
                                break;
                            case 6: // interval
                                _editingInterval = !_editingInterval;
                                tableView.CellAt(NSIndexPath.FromRowSection(indexPath.Row + 1, indexPath.Section)).Hidden = !_editingInterval;
                                break;
                            case 8: // duration
                                _editingDuration = !_editingDuration;
                                tableView.CellAt(NSIndexPath.FromRowSection(indexPath.Row + 1, indexPath.Section)).Hidden = !_editingDuration;
                                break;
                            case 10: // shots
                                _editingShots = !_editingShots;
                                tableView.CellAt(NSIndexPath.FromRowSection(indexPath.Row + 1, indexPath.Section)).Hidden = !_editingShots;
                                break;
                        }
                    }
                    break;
            }

            tableView.EndUpdates();
            tableView.DeselectRow(indexPath, true);
        }
    }
}
