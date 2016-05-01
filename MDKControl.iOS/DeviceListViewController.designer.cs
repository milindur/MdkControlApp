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
    [Register ("DeviceListViewController")]
    partial class DeviceListViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView DevicesTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem ScanButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DevicesTableView != null) {
                DevicesTableView.Dispose ();
                DevicesTableView = null;
            }

            if (ScanButton != null) {
                ScanButton.Dispose ();
                ScanButton = null;
            }
        }
    }
}