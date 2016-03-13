using System;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.Models;
using MDKControl.Core.ViewModels;
using MDKControl.iOS.Helpers;
using Microsoft.Practices.ServiceLocation;
using Reactive.Bindings;

namespace MDKControl.iOS
{
    partial class JoystickViewController : SubDeviceViewControllerBase, INavigationTarget
	{
        public JoystickViewController(IntPtr handle)
            : base(handle, MoCoBusProgramMode.Invalid, AppDelegate.JoystickViewKey)
		{
		}

        public object NavigationParameter { get; set; }

        public JoystickViewModel Vm { get; private set; }

        public override DeviceViewModel DeviceVm
        {
            get
            {
                return Vm.DeviceViewModel;
            }
        }

        private Action Dismissed { get; set; }

        public override void ViewDidLoad()
        {
            var helper = (JoystickNavigationHelper)NavigationParameter;
            Vm = helper.JoystickViewModel;
            Dismissed = helper.Dismissed;
            
            CancelButton.Clicked += (sender, e) => 
                {
                    ServiceLocator.Current.GetInstance<INavigationService>().GoBack();
                };
            
            SetButton.Clicked += (sender, e) => 
                {
                    Dismissed.Invoke();
                    ServiceLocator.Current.GetInstance<INavigationService>().GoBack();
                };

            Joystick.JoystickStart.SetCommand(Vm.StartJoystickCommand);
            Joystick.JoystickStop.SetCommand(Vm.StopJoystickCommand);
            Joystick.JoystickMove.SetCommand(Vm.MoveJoystickCommand);

            Slider.SliderStart.SetCommand(Vm.StartSliderCommand);
            Slider.SliderStop.SetCommand(Vm.StopSliderCommand);
            Slider.SliderMove.SetCommand(Vm.MoveSliderCommand);

            SetupBindings();

            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            System.Diagnostics.Debug.WriteLine("JoystickViewController ViewWillAppear");

            SetupBindings();

            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            System.Diagnostics.Debug.WriteLine("JoystickViewController ViewWillDisappear");

            DetachBindings();

            base.ViewWillDisappear(animated);
        }

        protected override void SetupBindings()
        {
            DetachBindings();

            base.SetupBindings();
        }

        protected override void DetachBindings()
        {
            base.DetachBindings();
        }
	}
}
