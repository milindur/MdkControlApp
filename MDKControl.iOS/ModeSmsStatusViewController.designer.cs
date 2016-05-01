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
    [Register ("ModeSmsStatusViewController")]
    partial class ModeSmsStatusViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem CancelButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ElapsedShotsValueLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ElapsedTimeValueLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField OverallShotsValueLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField OverallTimeValueLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem PauseResumeButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView ProgressBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField RemainingShotsValueLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField RemainingTimeValueLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField VideoLength24ValueLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField VideoLength25ValueLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField VideoLength30ValueLabel { get; set; }

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

            if (ElapsedTimeValueLabel != null) {
                ElapsedTimeValueLabel.Dispose ();
                ElapsedTimeValueLabel = null;
            }

            if (OverallShotsValueLabel != null) {
                OverallShotsValueLabel.Dispose ();
                OverallShotsValueLabel = null;
            }

            if (OverallTimeValueLabel != null) {
                OverallTimeValueLabel.Dispose ();
                OverallTimeValueLabel = null;
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

            if (RemainingTimeValueLabel != null) {
                RemainingTimeValueLabel.Dispose ();
                RemainingTimeValueLabel = null;
            }

            if (VideoLength24ValueLabel != null) {
                VideoLength24ValueLabel.Dispose ();
                VideoLength24ValueLabel = null;
            }

            if (VideoLength25ValueLabel != null) {
                VideoLength25ValueLabel.Dispose ();
                VideoLength25ValueLabel = null;
            }

            if (VideoLength30ValueLabel != null) {
                VideoLength30ValueLabel.Dispose ();
                VideoLength30ValueLabel = null;
            }
        }
    }
}