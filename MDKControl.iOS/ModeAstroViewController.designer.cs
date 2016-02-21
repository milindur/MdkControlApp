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
	[Register ("ModeAstroViewController")]
	partial class ModeAstroViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel HemisphereValueLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIPickerView HemisphereValuePickerView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel SpeedValueLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIPickerView SpeedValuePickerView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem StartButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (HemisphereValueLabel != null) {
				HemisphereValueLabel.Dispose ();
				HemisphereValueLabel = null;
			}
			if (HemisphereValuePickerView != null) {
				HemisphereValuePickerView.Dispose ();
				HemisphereValuePickerView = null;
			}
			if (SpeedValueLabel != null) {
				SpeedValueLabel.Dispose ();
				SpeedValueLabel = null;
			}
			if (SpeedValuePickerView != null) {
				SpeedValuePickerView.Dispose ();
				SpeedValuePickerView = null;
			}
			if (StartButton != null) {
				StartButton.Dispose ();
				StartButton = null;
			}
		}
	}
}
