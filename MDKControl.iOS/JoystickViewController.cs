using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using GalaSoft.MvvmLight.Views;

namespace MDKControl.iOS
{
    partial class JoystickViewController : UIViewController, INavigationTarget
	{
		public JoystickViewController (IntPtr handle) : base (handle)
		{
		}

        public object NavigationParameter { get; set; }
	}
}
