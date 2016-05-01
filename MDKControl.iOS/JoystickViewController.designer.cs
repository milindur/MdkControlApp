// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MDKControl.iOS
{
    [Register ("JoystickViewController")]
    partial class JoystickViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem CancelButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MDKControl.iOS.JoystickView Joystick { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem SetButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MDKControl.iOS.SliderView Slider { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
            }

            if (Joystick != null) {
                Joystick.Dispose ();
                Joystick = null;
            }

            if (SetButton != null) {
                SetButton.Dispose ();
                SetButton = null;
            }

            if (Slider != null) {
                Slider.Dispose ();
                Slider = null;
            }
        }
    }
}