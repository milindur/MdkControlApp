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
    partial class ModePanoViewController : SubDeviceTableViewControllerBase, INavigationTarget
    {
        private Binding _runStatusBinding;

        private Binding _exposureTimeBinding;
        private Binding _delayTimeBinding;

        private Binding _panStartPosBinding;
        private Binding _panStopPosBinding;
        private Binding _tiltStartPosBinding;
        private Binding _tiltStopPosBinding;
        private Binding _panSizeBinding;
        private Binding _tiltSizeBinding;

        private Binding _panFovSizeBinding;
        private Binding _tiltFovSizeBinding;

        private bool navigatedToStatusView = false;

        private bool _editingPreDelay;
        private bool _editingExposure;
        private bool _editingPostDelay;

        public ModePanoViewController(IntPtr handle)
            : base(handle, MoCoBusProgramMode.Panorama, AppDelegate.ModePanoViewKey)
        {
        }

        public object NavigationParameter { get; set; }

        public ModePanoViewModel Vm { get; private set; }

        public override DeviceViewModel DeviceVm
        {
            get
            {
                return Vm.DeviceViewModel;
            }
        }

        public override void ViewDidLoad()
        {
            System.Diagnostics.Debug.WriteLine("ModePanoViewController ViewDidLoad");

            Vm = (ModePanoViewModel)NavigationParameter;

            Vm.PropertyChanged += (s, e) =>
            {
            };

            var tableView = (UITableView)View;
            tableView.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            tableView.BackgroundView = null;
            tableView.AllowsSelection = true;

            PreDelayValueLabel.Text = string.Format("{0:F1}s", 0.1f); 
            
            ExposureValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (decimal)ExposureValuePickerTableViewCell.Model.SelectedTime.TotalSeconds;
                if (Vm.ExposureTime != t)
                    Vm.ExposureTime = t;
            };

            PostDelayValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (decimal)PostDelayValuePickerTableViewCell.Model.SelectedTime.TotalSeconds;
                if (Vm.DelayTime != t)
                    Vm.DelayTime = t;
            };

            StartButton.Clicked += (sender, e) => 
                {
                    navigatedToStatusView = true;
                    Vm.StartProgramCommand.Execute(null);
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo(AppDelegate.ModePanoStatusViewKey, Vm);
                };

            SetupBindings();
            
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            System.Diagnostics.Debug.WriteLine("ModePanoViewController ViewWillAppear");

            navigatedToStatusView = false;

            SetupBindings();

            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            System.Diagnostics.Debug.WriteLine("ModePanoViewController ViewWillDisappear");

            DetachBindings();

            base.ViewWillDisappear(animated);
        }

        protected override void SetupBindings()
        {
            DetachBindings();

            base.SetupBindings();

            _exposureTimeBinding = this.SetBinding(() => Vm.ExposureTime).WhenSourceChanges(() => 
            {
                ExposureValueLabel.Text = string.Format("{0:F1}s", Vm.ExposureTime);
                if ((decimal)ExposureValuePickerTableViewCell.Model.SelectedTime.TotalSeconds != Vm.ExposureTime)
                {
                    ExposureValuePickerTableViewCell.Model.SelectedTime = TimeSpan.FromSeconds((double)Vm.ExposureTime);
                }
            });
            _exposureTimeBinding.ForceUpdateValueFromSourceToTarget();

            _delayTimeBinding = this.SetBinding(() => Vm.DelayTime).WhenSourceChanges(() => 
            {
                PostDelayValueLabel.Text = string.Format("{0:F1}s", Vm.DelayTime);
                if ((decimal)PostDelayValuePickerTableViewCell.Model.SelectedTime.TotalSeconds != Vm.DelayTime)
                {
                    PostDelayValuePickerTableViewCell.Model.SelectedTime = TimeSpan.FromSeconds((double)Vm.DelayTime);
                }
            });
            _delayTimeBinding.ForceUpdateValueFromSourceToTarget();

            _panStartPosBinding = this.SetBinding(() => Vm.PanStartPosition).WhenSourceChanges(() => 
            {
                PanStartPosValueLabel.Text = string.Format("{0:F1}°", (double)Vm.PanStartPosition / (190 * 200 * 16) * 360);
            });
            _panStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _panStopPosBinding = this.SetBinding(() => Vm.PanStopPosition).WhenSourceChanges(() => 
            {
                PanStopPosValueLabel.Text = string.Format("{0:F1}°", (double)Vm.PanStopPosition / (190 * 200 * 16) * 360);
            });
            _panStopPosBinding.ForceUpdateValueFromSourceToTarget();

            _tiltStartPosBinding = this.SetBinding(() => Vm.TiltStartPosition).WhenSourceChanges(() => 
            {
                TiltStartPosValueLabel.Text = string.Format("{0:F1}°", (double)Vm.TiltStartPosition / (190 * 200 * 16) * 360);
            });
            _tiltStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _tiltStopPosBinding = this.SetBinding(() => Vm.TiltStopPosition).WhenSourceChanges(() => 
            {
                TiltStopPosValueLabel.Text = string.Format("{0:F1}°", (double)Vm.TiltStopPosition / (190 * 200 * 16) * 360);
            });
            _tiltStopPosBinding.ForceUpdateValueFromSourceToTarget();

            _panSizeBinding = this.SetBinding(() => Vm.PanSize).WhenSourceChanges(() => 
                {
                    PanSizeValueLabel.Text = string.Format("{0:F1}°", (double)Vm.PanSize / (190 * 200 * 16) * 360);
                });
            _panSizeBinding.ForceUpdateValueFromSourceToTarget();

            _tiltSizeBinding = this.SetBinding(() => Vm.TiltSize).WhenSourceChanges(() => 
                {
                    TiltSizeValueLabel.Text = string.Format("{0:F1}°", (double)Vm.TiltSize / (190 * 200 * 16) * 360);
                });
            _tiltSizeBinding.ForceUpdateValueFromSourceToTarget();

            _panFovSizeBinding = this.SetBinding(() => Vm.PanRefSize).WhenSourceChanges(() => 
                {
                    PanFovSizeValueLabel.Text = string.Format("{0:F1}°", (double)Vm.PanRefSize / (190 * 200 * 16) * 360);
                });
            _panFovSizeBinding.ForceUpdateValueFromSourceToTarget();

            _tiltFovSizeBinding = this.SetBinding(() => Vm.TiltRefSize).WhenSourceChanges(() => 
                {
                    TiltFovSizeValueLabel.Text = string.Format("{0:F1}°", (double)Vm.TiltRefSize / (190 * 200 * 16) * 360);
                });
            _tiltFovSizeBinding.ForceUpdateValueFromSourceToTarget();

            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus).WhenSourceChanges(() => 
            {
                var nav = ServiceLocator.Current.GetInstance<INavigationService>();
                if (nav.CurrentPageKey != AppDelegate.ModeSmsViewKey)
                    return;
                if (DeviceVm.RunStatus != MDKControl.Core.Models.MoCoBusRunStatus.Stopped && nav.CurrentPageKey != AppDelegate.ModePanoStatusViewKey && !navigatedToStatusView)
                {
                    navigatedToStatusView = true;
                    DeviceVm.StartUpdateTask();
                    nav.NavigateTo(AppDelegate.ModePanoStatusViewKey, Vm);
                }
            });
            _runStatusBinding.ForceUpdateValueFromSourceToTarget();
        }

        protected override void DetachBindings()
        {
            _exposureTimeBinding?.Detach();
            _exposureTimeBinding = null;

            _delayTimeBinding?.Detach();
            _delayTimeBinding = null;

            _panStartPosBinding?.Detach();
            _panStartPosBinding = null;

            _panStopPosBinding?.Detach();
            _panStopPosBinding = null;

            _tiltStartPosBinding?.Detach();
            _tiltStartPosBinding = null;

            _tiltStopPosBinding?.Detach();
            _tiltStopPosBinding = null;

            _panSizeBinding?.Detach();
            _panSizeBinding = null;

            _tiltSizeBinding?.Detach();
            _tiltSizeBinding = null;

            _panFovSizeBinding?.Detach();
            _panFovSizeBinding = null;

            _tiltFovSizeBinding?.Detach();
            _tiltFovSizeBinding = null;

            _runStatusBinding?.Detach();
            _runStatusBinding = null;

            base.DetachBindings();
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            switch (indexPath.Section)
            {
                case 0:
                    {
                        switch (indexPath.Row)
                        {
                            case 2: // fov size
                                return 0;
                        }
                    }
                    break;
                case 2:
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
                            case 0: // start fov position
                                ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo(
                                    AppDelegate.JoystickViewKey, 
                                    new JoystickNavigationHelper(Vm.DeviceViewModel.JoystickViewModel, () =>
                                        {
                                            Vm.SetRefStartCommand.Execute(null);
                                        }));
                                break;
                            case 1: // stop fov position
                                ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo(
                                    AppDelegate.JoystickViewKey, 
                                    new JoystickNavigationHelper(Vm.DeviceViewModel.JoystickViewModel, () =>
                                        {
                                            Vm.SetRefStopCommand.Execute(null);
                                        }));
                                break;
                        }
                    }
                    break;
                case 1:
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
                case 2:
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
                        }
                    }
                    break;
            }

            tableView.EndUpdates();
            tableView.DeselectRow(indexPath, true);
        }
    }
}
