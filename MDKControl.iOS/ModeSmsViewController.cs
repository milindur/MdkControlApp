using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.ViewModels;
using Microsoft.Practices.ServiceLocation;
using MDKControl.iOS.Helpers;

namespace MDKControl.iOS
{
    partial class ModeSmsViewController : UITableViewController, INavigationTarget
    {
        private Binding _runStatusBinding;

        private Binding _exposureTimeBinding;
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

        private bool navigatedToStatusView = false;

        private bool _editingPreDelay;
        private bool _editingExposure;
        private bool _editingPostDelay;
        private bool _editingInterval;
        private bool _editingDuration;
        private bool _editingShots;

        public ModeSmsViewController(IntPtr handle)
            : base(handle)
        {
        }

        public object NavigationParameter { get; set; }

        public ModeSmsViewModel Vm { get; private set; }
        public DeviceViewModel DeviceVm { get { return Vm.DeviceViewModel; } }

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

            PreDelayValueLabel.Text = string.Format("{0:F1}s", 0.1f); 
            
            _exposureTimeBinding = this.SetBinding(() => Vm.ExposureTime)
                .WhenSourceChanges(() =>
                { 
                    ExposureValueLabel.Text = string.Format("{0:F1}s", Vm.ExposureTime);
                    if ((decimal)ExposureValuePickerTableViewCell.Model.SelectedTime.TotalSeconds != Vm.ExposureTime)
                    {
                        ExposureValuePickerTableViewCell.Model.SelectedTime = TimeSpan.FromSeconds((double)Vm.ExposureTime);
                    }
                });
            _exposureTimeBinding.ForceUpdateValueFromSourceToTarget();

            ExposureValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (decimal)ExposureValuePickerTableViewCell.Model.SelectedTime.TotalSeconds;
                if (Vm.ExposureTime != t)
                    Vm.ExposureTime = t;
            };

            _delayTimeBinding = this.SetBinding(() => Vm.DelayTime)
                .WhenSourceChanges(() =>
                { 
                    PostDelayValueLabel.Text = string.Format("{0:F1}s", Vm.DelayTime); 
                    if ((decimal)PostDelayValuePickerTableViewCell.Model.SelectedTime.TotalSeconds != Vm.DelayTime)
                    {
                        PostDelayValuePickerTableViewCell.Model.SelectedTime = TimeSpan.FromSeconds((double)Vm.DelayTime);
                    }
                });
            _delayTimeBinding.ForceUpdateValueFromSourceToTarget();

            PostDelayValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (decimal)PostDelayValuePickerTableViewCell.Model.SelectedTime.TotalSeconds;
                if (Vm.DelayTime != t)
                    Vm.DelayTime = t;
            };

            _intervalTimeBinding = this.SetBinding(() => Vm.IntervalTime)
                .WhenSourceChanges(() =>
                {
                    IntervalValueLabel.Text = string.Format("{0:F1}s", Vm.IntervalTime);
                    if ((decimal)IntervalValuePickerTableViewCell.Model.SelectedTime.TotalSeconds != Vm.IntervalTime)
                    {
                        IntervalValuePickerTableViewCell.Model.SelectedTime = TimeSpan.FromSeconds((double)Vm.IntervalTime);
                    }
                });
            _intervalTimeBinding.ForceUpdateValueFromSourceToTarget();

            IntervalValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (decimal)IntervalValuePickerTableViewCell.Model.SelectedTime.TotalSeconds;
                if (Vm.IntervalTime != t)
                    Vm.IntervalTime = t;
            };

            _durationTimeBinding = this.SetBinding(() => Vm.DurationTime)
                .WhenSourceChanges(() =>
                {
                    DurationValueLabel.Text = string.Format("{0}:{1:00}m", (int)(Vm.DurationTime / 60), (int)Vm.DurationTime % 60);
                    if ((decimal)DurationValuePickerTableViewCell.Model.SelectedTime.TotalSeconds != Vm.DurationTime)
                    {
                        DurationValuePickerTableViewCell.Model.SelectedTime = TimeSpan.FromSeconds((double)Vm.DurationTime);
                    }
                });
            _durationTimeBinding.ForceUpdateValueFromSourceToTarget();

            DurationValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (decimal)DurationValuePickerTableViewCell.Model.SelectedTime.TotalSeconds;
                if (Vm.DurationTime != t)
                    Vm.DurationTime = t;
            };

            _maxShotsBinding = this.SetBinding(() => Vm.MaxShots)
                .WhenSourceChanges(() =>
                {
                    ShotsValueLabel.Text = string.Format("{0}", Vm.MaxShots);
                    if (ShotsValuePickerTableViewCell.Model.SelectedNumber != Vm.MaxShots)
                    {
                        ShotsValuePickerTableViewCell.Model.SelectedNumber = Vm.MaxShots;
                    }
                });
            _maxShotsBinding.ForceUpdateValueFromSourceToTarget();

            ShotsValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
                {
                    var t = (ushort)ShotsValuePickerTableViewCell.Model.SelectedNumber;
                    if (Vm.MaxShots != t)
                        Vm.MaxShots = t;
                };

            _sliderStartPosBinding = this.SetBinding(() => Vm.SliderStartPosition)
                .WhenSourceChanges(() =>
                {
                    SliderStartPosValueLabel.Text = string.Format("{0}", Vm.SliderStartPosition);
                });
            _sliderStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _sliderStopPosBinding = this.SetBinding(() => Vm.SliderStopPosition)
                .WhenSourceChanges(() =>
                {
                    SliderStopPosValueLabel.Text = string.Format("{0}", Vm.SliderStopPosition);
                });
            _sliderStopPosBinding.ForceUpdateValueFromSourceToTarget();

            _panStartPosBinding = this.SetBinding(() => Vm.PanStartPosition)
                .WhenSourceChanges(() =>
                {
                    PanStartPosValueLabel.Text = string.Format("{0:F1}°", (double)Vm.PanStartPosition / (190 * 200 * 16) * 360);
                });
            _panStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _panStopPosBinding = this.SetBinding(() => Vm.PanStopPosition)
                .WhenSourceChanges(() =>
                {
                    PanStopPosValueLabel.Text = string.Format("{0:F1}°", (double)Vm.PanStopPosition / (190 * 200 * 16) * 360);
                });
            _panStopPosBinding.ForceUpdateValueFromSourceToTarget();

            _tiltStartPosBinding = this.SetBinding(() => Vm.TiltStartPosition)
                .WhenSourceChanges(() =>
                {
                    TiltStartPosValueLabel.Text = string.Format("{0:F1}°", (double)Vm.TiltStartPosition / (190 * 200 * 16) * 360);
                });
            _tiltStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _tiltStopPosBinding = this.SetBinding(() => Vm.TiltStopPosition)
                .WhenSourceChanges(() =>
                {
                    TiltStopPosValueLabel.Text = string.Format("{0:F1}°", (double)Vm.TiltStopPosition / (190 * 200 * 16) * 360);
                });
            _tiltStopPosBinding.ForceUpdateValueFromSourceToTarget();

            SwapStartStopButton.SetCommand(
                "TouchUpInside",
                Vm.SwapStartStopCommand);

            StartButton.Clicked += (sender, e) => 
                {
                    navigatedToStatusView = true;
                    Vm.StartProgramCommand.Execute(null);
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo(AppDelegate.ModeSmsStatusViewKey, Vm);
                };            
            
            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus).WhenSourceChanges(() =>
                {
                    var nav = ServiceLocator.Current.GetInstance<INavigationService>();
                    if (DeviceVm.RunStatus != MDKControl.Core.Models.MoCoBusRunStatus.Stopped && nav.CurrentPageKey != AppDelegate.ModeSmsStatusViewKey && !navigatedToStatusView)
                    {
                        navigatedToStatusView = true;
                        DeviceVm.StartUpdateTask();
                        nav.NavigateTo(AppDelegate.ModeSmsStatusViewKey, Vm);
                    }
                });
            _runStatusBinding.ForceUpdateValueFromSourceToTarget();

            base.ViewDidLoad();
        }

        public override void ViewDidAppear(bool animated)
        {
            navigatedToStatusView = false;
            
            base.ViewDidAppear(animated);
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
