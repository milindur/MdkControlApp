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
	[Register ("ModePanoStatusViewController")]
	partial class ModePanoStatusViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem CancelButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField ElapsedShotsValueLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField OverallColsValueLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField OverallRowsValueLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField OverallShotsValueLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem PauseResumeButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIProgressView ProgressBar { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField RemainingShotsValueLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}
			if (ElapsedShotsValueLabel != null) {
				ElapsedShotsValueLabel.Dispose ();
				ElapsedShotsValueLabel = null;
			}
			if (OverallColsValueLabel != null) {
				OverallColsValueLabel.Dispose ();
				OverallColsValueLabel = null;
			}
			if (OverallRowsValueLabel != null) {
				OverallRowsValueLabel.Dispose ();
				OverallRowsValueLabel = null;
			}
			if (OverallShotsValueLabel != null) {
				OverallShotsValueLabel.Dispose ();
				OverallShotsValueLabel = null;
			}
			if (PauseResumeButton != null) {
				PauseResumeButton.Dispose ();
				PauseResumeButton = null;
			}
			if (ProgressBar != null) {
				ProgressBar.Dispose ();
				ProgressBar = null;
			}
			if (RemainingShotsValueLabel != null) {
				RemainingShotsValueLabel.Dispose ();
				RemainingShotsValueLabel = null;
			}
		}
	}
}
