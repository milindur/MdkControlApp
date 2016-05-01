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
    internal partial class ModePanoViewController : SubDeviceTableViewControllerBase, INavigationTarget
    {
        private Binding _runStatusBinding;

        private Binding _exposureTimeBinding;
        private Binding _preDelayTimeBinding;
        private Binding _delayTimeBinding;
        private Binding _pauseBinding;
        private Binding _repititionsBinding;

        private Binding _panStartPosBinding;
        private Binding _panStopPosBinding;
        private Binding _tiltStartPosBinding;
        private Binding _tiltStopPosBinding;
        private Binding _panSizeBinding;
        private Binding _tiltSizeBinding;

        private Binding _panFovSizeBinding;
        private Binding _tiltFovSizeBinding;

        private bool _navigatedToStatusView;

        private bool _editingPreDelay;
        private bool _editingExposure;
        private bool _editingPostDelay;
        private bool _editingPause;
        private bool _editingRepititions;

        public ModePanoViewController(IntPtr handle)
            : base(handle, MoCoBusProgramMode.Panorama, AppDelegate.ModePanoViewKey)
        {
        }

        public object NavigationParameter { get; set; }

        public ModePanoViewModel Vm { get; private set; }

        public override DeviceViewModel DeviceVm => Vm.DeviceViewModel;

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

            PauseValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (decimal)PauseValuePickerTableViewCell.Model.SelectedTime.TotalSeconds;
                if (Vm.PauseTime != t)
                    Vm.PauseTime = t;
            };

            RepititionsValuePickerTableViewCell.Model.ValueChanged += (sender, e) =>
            {
                var t = (ushort)RepititionsValuePickerTableViewCell.Model.SelectedNumber;
                if (Vm.Repititions != t)
                    Vm.Repititions = t;
            };

            StartButton.Clicked += (sender, e) => 
                {
                    _navigatedToStatusView = true;
                    Vm.StartProgramCommand.Execute(null);
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo(AppDelegate.ModePanoStatusViewKey, Vm);
                };

            SetupBindings();
            
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            System.Diagnostics.Debug.WriteLine("ModePanoViewController ViewWillAppear");

            _navigatedToStatusView = false;

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

            _pauseBinding = this.SetBinding(() => Vm.PauseTime).WhenSourceChanges(() =>
            {
                PauseValueLabel.Text = $"{Vm.PauseTime:F1}s";
                if ((decimal)PauseValuePickerTableViewCell.Model.SelectedTime.TotalSeconds != Vm.PauseTime)
                {
                    PauseValuePickerTableViewCell.Model.SelectedTime = TimeSpan.FromSeconds((double)Vm.PauseTime);
                }
            });
            _pauseBinding.ForceUpdateValueFromSourceToTarget();

            _repititionsBinding = this.SetBinding(() => Vm.Repititions).WhenSourceChanges(() =>
            {
                RepititionsValueLabel.Text = $"{Vm.Repititions}";
                if ((ushort)RepititionsValuePickerTableViewCell.Model.SelectedNumber != Vm.Repititions)
                {
                    RepititionsValuePickerTableViewCell.Model.SelectedNumber = Vm.Repititions;
                }
            });
            _repititionsBinding.ForceUpdateValueFromSourceToTarget();

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

            _panSizeBinding = this.SetBinding(() => Vm.PanSize).WhenSourceChanges(() => 
                {
                    PanSizeValueLabel.Text = $"{(double) Vm.PanSize/(190*200*16)*360:F1}°";
                });
            _panSizeBinding.ForceUpdateValueFromSourceToTarget();

            _tiltSizeBinding = this.SetBinding(() => Vm.TiltSize).WhenSourceChanges(() => 
                {
                    TiltSizeValueLabel.Text = $"{(double) Vm.TiltSize/(190*200*16)*360:F1}°";
                });
            _tiltSizeBinding.ForceUpdateValueFromSourceToTarget();

            _panFovSizeBinding = this.SetBinding(() => Vm.PanRefSize).WhenSourceChanges(() => 
                {
                    PanFovSizeValueLabel.Text = $"{(double) Vm.PanRefSize/(190*200*16)*360:F1}°";
                });
            _panFovSizeBinding.ForceUpdateValueFromSourceToTarget();

            _tiltFovSizeBinding = this.SetBinding(() => Vm.TiltRefSize).WhenSourceChanges(() => 
                {
                    TiltFovSizeValueLabel.Text = $"{(double) Vm.TiltRefSize/(190*200*16)*360:F1}°";
                });
            _tiltFovSizeBinding.ForceUpdateValueFromSourceToTarget();

            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus).WhenSourceChanges(() => 
            {
                var nav = ServiceLocator.Current.GetInstance<INavigationService>();

                System.Diagnostics.Debug.WriteLine($"ModePanoViewController RunStatusBinding: CurrentPageKey={nav.CurrentPageKey}, RunStatus={DeviceVm.RunStatus}, _navigatedToStatusView={_navigatedToStatusView}");

                if (nav.CurrentPageKey != AppDelegate.ModePanoViewKey)
                    return;
                if (DeviceVm.RunStatus != MoCoBusRunStatus.Stopped && nav.CurrentPageKey != AppDelegate.ModePanoStatusViewKey && !_navigatedToStatusView && !DeviceVm.IsUpdateTaskRunning)
                {
                    System.Diagnostics.Debug.WriteLine($"ModePanoViewController RunStatusBinding: Start update task and navigate to status view");
                    _navigatedToStatusView = true;
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

            _preDelayTimeBinding?.Detach();
            _preDelayTimeBinding = null;

            _delayTimeBinding?.Detach();
            _delayTimeBinding = null;

            _pauseBinding?.Detach();
            _pauseBinding = null;

            _repititionsBinding?.Detach();
            _repititionsBinding = null;

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
                            case 7: // pause picker
                                if (_editingPause)
                                {
                                    return 219;
                                }
                                else
                                {
                                    return 0;
                                }
                            case 9: // repititions picker
                                if (_editingRepititions)
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
                            case 6: // pause
                                _editingPause = !_editingPause;
                                tableView.CellAt(NSIndexPath.FromRowSection(indexPath.Row + 1, indexPath.Section)).Hidden = !_editingPause;
                                break;
                            case 8: // repititions
                                _editingRepititions = !_editingRepititions;
                                tableView.CellAt(NSIndexPath.FromRowSection(indexPath.Row + 1, indexPath.Section)).Hidden = !_editingRepititions;
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
