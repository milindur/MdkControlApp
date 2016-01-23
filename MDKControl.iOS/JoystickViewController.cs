using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using Reactive.Bindings;
using MDKControl.Core.ViewModels;
using MDKControl.iOS.Helpers;

namespace MDKControl.iOS
{
    partial class JoystickViewController : UIViewController, INavigationTarget
	{
		public JoystickViewController (IntPtr handle) : base (handle)
		{
		}

        public object NavigationParameter { get; set; }

        public JoystickViewModel Vm { get; private set; }
        private Action Dismissed { get; set; }

        public override void ViewDidLoad()
        {
            var helper = (JoystickNavigationHelper)NavigationParameter;
            Vm = helper.JoystickViewModel;
            Dismissed = helper.Dismissed;
            
            CancelButton.Clicked += (sender, e) => 
                {
                    ServiceLocator.Current.GetInstance<INavigationService>().GoBack();
                    //NavigationController.PopViewController(true);
                };
            
            SetButton.Clicked += (sender, e) => 
                {
                    Dismissed.Invoke();
                    ServiceLocator.Current.GetInstance<INavigationService>().GoBack();
                    //NavigationController.PopViewController(true);
                };

            Joystick.JoystickStart.SetCommand(Vm.StartJoystickCommand);
            Joystick.JoystickStop.SetCommand(Vm.StopJoystickCommand);
            Joystick.JoystickMove.SetCommand(Vm.MoveJoystickCommand);

            base.ViewDidLoad();
        }
	}
}
