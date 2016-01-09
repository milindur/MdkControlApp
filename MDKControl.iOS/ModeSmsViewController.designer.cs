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
	[Register ("ModeSmsViewController")]
	partial class ModeSmsViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableViewCell ExposureTableViewCell { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableViewCell PreDelayTableViewCell { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem StartButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableViewCell StartTableViewCell { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableViewCell StopTableViewCell { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton SwapStartStopButon { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIDatePicker TimePicker { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ExposureTableViewCell != null) {
				ExposureTableViewCell.Dispose ();
				ExposureTableViewCell = null;
			}
			if (PreDelayTableViewCell != null) {
				PreDelayTableViewCell.Dispose ();
				PreDelayTableViewCell = null;
			}
			if (StartButton != null) {
				StartButton.Dispose ();
				StartButton = null;
			}
			if (StartTableViewCell != null) {
				StartTableViewCell.Dispose ();
				StartTableViewCell = null;
			}
			if (StopTableViewCell != null) {
				StopTableViewCell.Dispose ();
				StopTableViewCell = null;
			}
			if (SwapStartStopButon != null) {
				SwapStartStopButon.Dispose ();
				SwapStartStopButon = null;
			}
			if (TimePicker != null) {
				TimePicker.Dispose ();
				TimePicker = null;
			}
		}
	}
}
