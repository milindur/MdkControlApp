using System;
using UIKit;
using Foundation;
using System.ComponentModel;
using CoreGraphics;

namespace MDKControl.iOS
{
    [Register("SliderView"), DesignTimeVisible(true)]
    public class SliderView : UIView
    {
        public SliderView(IntPtr handle)
            : base(handle)
        {
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);

            using (var ctx = UIGraphics.GetCurrentContext())
            {
                ctx.SetLineWidth(10);
                UIColor.Red.SetFill();
                UIColor.Green.SetStroke();
                ctx.SetLineDash(0, new nfloat[] { 10, 10 });
                ctx.FillEllipseInRect(rect);
                ctx.StrokeEllipseInRect(rect);
            }
        }
    }
}
