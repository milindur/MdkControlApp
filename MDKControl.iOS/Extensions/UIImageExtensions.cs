using System;
using UIKit;
using CoreGraphics;

namespace MDKControl.iOS.Extensions
{
    public static class UIImageExtensions
    {
        public static UIImage MakeThumb(this UIImage image, CGSize size)
        {
            UIGraphics.BeginImageContextWithOptions(size, false, UIScreen.MainScreen.Scale);
            image.Draw(new CGRect(0, 0, size.Width, size.Height));
            var thumb = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return thumb;
        }
    }
}
