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
	[Register ("DeviceViewController")]
	partial class DeviceViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableViewCell ModeAstroTableViewCell { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableViewCell ModePanoramaTableViewCell { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableViewCell ModeSmsTableViewCell { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ModeAstroTableViewCell != null) {
				ModeAstroTableViewCell.Dispose ();
				ModeAstroTableViewCell = null;
			}
			if (ModePanoramaTableViewCell != null) {
				ModePanoramaTableViewCell.Dispose ();
				ModePanoramaTableViewCell = null;
			}
			if (ModeSmsTableViewCell != null) {
				ModeSmsTableViewCell.Dispose ();
				ModeSmsTableViewCell = null;
			}
		}
	}
}
