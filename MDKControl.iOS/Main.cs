using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Xamarin;

namespace MDKControl.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
            Nito.AsyncEx.EnlightenmentVerification.EnsureLoaded();

            Insights.HasPendingCrashReport += (sender, isStartupCrash) =>
                {
                    if (isStartupCrash) {
                        Insights.PurgePendingCrashReports().Wait();
                    }
                };

#if DEBUG
            Insights.Initialize("1532003910b06b50a709d0e691a8054e904922c8");
#else
            Insights.Initialize("76d51a30602c5fd9a5e64f263e25d14947533c61");
#endif

            Insights.Track("Initialize");

			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}
