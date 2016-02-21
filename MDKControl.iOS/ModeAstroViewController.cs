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
    partial class ModeAstroViewController : UITableViewController, INavigationTarget
    {
        private Binding _runStatusBinding;

        private Binding _hemisphereBinding;
        private Binding _speedBinding;

        private bool navigatedToStatusView = false;

        private bool _editingHemisphere;
        private bool _editingSpeed;

        private ListPickerViewModel<string> _hemispherePickerViewModel;
        private ListPickerViewModel<string> _speedPickerViewModel;

        public ModeAstroViewController(IntPtr handle)
            : base(handle)
        {
            
        }

        public object NavigationParameter { get; set; }

        public ModeAstroViewModel Vm { get; private set; }
        public DeviceViewModel DeviceVm { get { return Vm.DeviceViewModel; } }

        public override void ViewDidLoad()
        {
            System.Diagnostics.Debug.WriteLine("ModeAstroViewController ViewDidLoad");

            Vm = (ModeAstroViewModel)NavigationParameter;

            Vm.PropertyChanged += (s, e) =>
            {
            };

            var tableView = (UITableView)View;
            tableView.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            tableView.BackgroundView = null;
            tableView.AllowsSelection = true;

            _hemispherePickerViewModel = new ListPickerViewModel<string>(new[] { "North", "South" });
            _hemispherePickerViewModel.ValueChanged += (sender, e) =>
                {
                    var t = (MDKControl.Core.Models.AstroDirection)_hemispherePickerViewModel.SelectedIndex;
                    if (Vm.Direction != t)
                        Vm.Direction = t;
                };
            HemisphereValuePickerView.Model = _hemispherePickerViewModel;

            _hemisphereBinding = this.SetBinding(() => Vm.Direction)
                .WhenSourceChanges(() =>
                    { 
                        HemisphereValueLabel.Text = _hemispherePickerViewModel.Items[(int)Vm.Direction];
                        if ((MDKControl.Core.Models.AstroDirection)_hemispherePickerViewModel.SelectedIndex != Vm.Direction)
                        {
                            HemisphereValuePickerView.ReloadAllComponents();
                            HemisphereValuePickerView.Select((int)Vm.Direction, 0, !HemisphereValuePickerView.Hidden);
                        }
                    });
            _hemisphereBinding.ForceUpdateValueFromSourceToTarget();

            _speedPickerViewModel = new ListPickerViewModel<string>(new[] { "Sidereal/Stars", "Lunar" });
            _speedPickerViewModel.ValueChanged += (sender, e) =>
                {
                    var t = (MDKControl.Core.Models.AstroSpeed)_speedPickerViewModel.SelectedIndex;
                    if (Vm.Speed != t)
                        Vm.Speed = t;
                };
            SpeedValuePickerView.Model = _speedPickerViewModel;

            _speedBinding = this.SetBinding(() => Vm.Speed)
                .WhenSourceChanges(() =>
                    { 
                        SpeedValueLabel.Text = _speedPickerViewModel.Items[(int)Vm.Speed];
                        if ((MDKControl.Core.Models.AstroSpeed)_speedPickerViewModel.SelectedIndex != Vm.Speed)
                        {
                            SpeedValuePickerView.ReloadAllComponents();
                            SpeedValuePickerView.Select((int)Vm.Speed, 0, !SpeedValuePickerView.Hidden);
                        }
                    });
            _speedBinding.ForceUpdateValueFromSourceToTarget();

            StartButton.Clicked += (sender, e) => 
                {
                    navigatedToStatusView = true;
                    Vm.StartProgramCommand.Execute(null);
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo(AppDelegate.ModeAstroStatusViewKey, Vm);
                };            
            
            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus).WhenSourceChanges(() =>
                {
                    var nav = ServiceLocator.Current.GetInstance<INavigationService>();
                    if (nav.CurrentPageKey != AppDelegate.ModeAstroViewKey) return;

                    if (DeviceVm.RunStatus != MDKControl.Core.Models.MoCoBusRunStatus.Stopped && nav.CurrentPageKey != AppDelegate.ModeAstroStatusViewKey && !navigatedToStatusView)
                    {
                        navigatedToStatusView = true;
                        DeviceVm.StartUpdateTask();
                        nav.NavigateTo(AppDelegate.ModeAstroStatusViewKey, Vm);
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
                case 0:
                    {
                        switch (indexPath.Row)
                        {
                            case 1: // hemisphere picker                                
                                if (_editingHemisphere)
                                {
                                    return 119;
                                }
                                else
                                {
                                    return 0;
                                }
                            case 3: // speed picker
                                if (_editingSpeed)
                                {
                                    return 119;
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
                            case 0: // hemisphere
                                _editingHemisphere = !_editingHemisphere;
                                tableView.CellAt(NSIndexPath.FromRowSection(indexPath.Row + 1, indexPath.Section)).Hidden = !_editingHemisphere;
                                break;
                            case 2: // speed
                                _editingSpeed = !_editingSpeed;
                                tableView.CellAt(NSIndexPath.FromRowSection(indexPath.Row + 1, indexPath.Section)).Hidden = !_editingSpeed;
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