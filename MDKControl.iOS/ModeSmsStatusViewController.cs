using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using GalaSoft.MvvmLight.Views;

namespace MDKControl.iOS
{
	partial class ModeSmsStatusViewController : UIViewController, INavigationTarget
	{
		public ModeSmsStatusViewController (IntPtr handle) : base (handle)
		{
		}

        public object NavigationParameter { get; set; }
	}
}
