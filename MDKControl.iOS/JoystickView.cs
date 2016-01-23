using System;
using UIKit;
using Foundation;
using System.ComponentModel;
using CoreGraphics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace MDKControl.iOS
{
    [Register("JoystickView"), DesignTimeVisible(true)]
    public class JoystickView : UIView
    {
        private readonly EventLoopScheduler _scheduler = new EventLoopScheduler();
        private readonly Subject<MDKControl.Core.Models.Point> _joystickStartSubject = new Subject<MDKControl.Core.Models.Point>();
        private readonly Subject<Unit> _joystickStopSubject = new Subject<Unit>();
        private readonly Subject<MDKControl.Core.Models.Point> _joystickMoveSubject = new Subject<MDKControl.Core.Models.Point>();

        private CGRect _bounds = new CGRect();

        private bool _isActive = false;
        private CGPoint _joystickPositionRaw = new CGPoint();
        private MDKControl.Core.Models.Point _joystickPosition = new MDKControl.Core.Models.Point(0, 0);

        public JoystickView(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        public IObservable<MDKControl.Core.Models.Point> JoystickStart { get { return _joystickStartSubject.ObserveOn(_scheduler); } }

        public IObservable<Unit> JoystickStop { get { return _joystickStopSubject.ObserveOn(_scheduler); } }

        public IObservable<MDKControl.Core.Models.Point> JoystickMove { get { return _joystickMoveSubject.ObserveOn(_scheduler); } }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);

            _bounds = new CGRect(rect.Location, rect.Size);
            
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

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            var touch = touches.AnyObject as UITouch;
            if (touch != null)
            {
                System.Diagnostics.Debug.WriteLine("TouchesBegan");
                var point = GetTouchPoint(touch);
                OnJoystickStart(point.X, point.Y);
                SetNeedsDisplay();
            }
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            var touch = touches.AnyObject as UITouch;
            if (touch != null)
            {
                System.Diagnostics.Debug.WriteLine("TouchesMoved");
                var point = GetTouchPoint(touch);
                OnJoystickMove(point.X, point.Y);
                SetNeedsDisplay();
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            System.Diagnostics.Debug.WriteLine("TouchesEnded");
            OnJoystickStop();
            SetNeedsDisplay();
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            System.Diagnostics.Debug.WriteLine("TouchesCancelled");
            OnJoystickStop();
            SetNeedsDisplay();
        }

        private MDKControl.Core.Models.Point GetTouchPoint(UITouch touch)
        {
            var location = touch.LocationInView(this);

            var x = location.X;
            var y = location.Y;

            if (x < _bounds.Left)
                x = _bounds.Left;
            if (x > _bounds.Right)
                x = _bounds.Right;
            if (y < _bounds.Top)
                y = _bounds.Top;
            if (y > _bounds.Bottom)
                y = _bounds.Bottom;

            _joystickPositionRaw = new CGPoint(x, y);
            _isActive = true;

            var scaleX = _bounds.Width / 2f;
            var scaleY = _bounds.Height / 2f;

            x = (x - scaleX) * 100f / scaleX;
            y = (y - scaleY) * 100f / scaleY;

            System.Diagnostics.Debug.WriteLine($"GetTouchPoint: x={x}, y={y}");

            return new MDKControl.Core.Models.Point((float)x, (float)y);
        }

        protected void OnJoystickStart(float x, float y)
        {
            _joystickPosition = new MDKControl.Core.Models.Point(x, y);
            _joystickStartSubject.OnNext(_joystickPosition);
        }

        protected void OnJoystickStop()
        {
            _joystickStopSubject.OnNext(Unit.Default);
        }

        protected void OnJoystickMove(float x, float y)
        {
            _joystickPosition = new MDKControl.Core.Models.Point(x, y);
            _joystickMoveSubject.OnNext(_joystickPosition);
        }
    }
}
