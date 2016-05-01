using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MDKControl.iOS
{
    [DesignTimeVisible(true)]
    internal partial class JoystickView : UIView
    {
        private const int Padding = 10;
        
        private readonly EventLoopScheduler _scheduler = new EventLoopScheduler();
        private readonly Subject<Core.Models.Point> _joystickStartSubject = new Subject<Core.Models.Point>();
        private readonly Subject<Unit> _joystickStopSubject = new Subject<Unit>();
        private readonly Subject<Core.Models.Point> _joystickMoveSubject = new Subject<Core.Models.Point>();

        private CGRect _bounds;

        private bool _isActive;
        private CGPoint _joystickPositionRaw;
        private Core.Models.Point _joystickPosition = new Core.Models.Point(0, 0);

        public JoystickView(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        public IObservable<Core.Models.Point> JoystickStart => _joystickStartSubject.ObserveOn(_scheduler);

        public IObservable<Unit> JoystickStop => _joystickStopSubject.ObserveOn(_scheduler);

        public IObservable<Core.Models.Point> JoystickMove => _joystickMoveSubject.ObserveOn(_scheduler);

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            _bounds = new CGRect(rect.X + Padding, rect.Y + Padding, rect.Width - 2 * Padding, rect.Height - 2 * Padding);
            
            using (var ctx = UIGraphics.GetCurrentContext())
            {
                ctx.SaveState();

                UIColor.DarkGray.SetStroke();
                ctx.SetLineWidth(3);
                ctx.StrokeRect(_bounds);

                ctx.RestoreState();

                ctx.SaveState();
                
                UIColor.DarkGray.SetStroke();
                ctx.SetLineWidth(2);
                ctx.SetLineDash(0, new nfloat[] { 4, 4 });

                ctx.StrokeLineSegments(new [] 
                    { 
                        new CGPoint(_bounds.GetMidX(), _bounds.Top), 
                        new CGPoint(_bounds.GetMidX(), _bounds.Bottom) 
                    });
                ctx.StrokeLineSegments(new [] 
                    { 
                        new CGPoint(_bounds.Left, _bounds.GetMidY()), 
                        new CGPoint(_bounds.Right, _bounds.GetMidY()) 
                    });

                ctx.RestoreState();

                ctx.SaveState();

                UIColor.DarkGray.SetStroke();
                ctx.SetLineWidth(2);
                ctx.SetLineDash(0, new nfloat[] { 2, 2 });

                ctx.StrokeLineSegments(new [] 
                    { 
                        new CGPoint(_bounds.Left + _bounds.Width / 4f, _bounds.Top), 
                        new CGPoint(_bounds.Left + _bounds.Width / 4f, _bounds.Bottom) 
                    });
                ctx.StrokeLineSegments(new [] 
                    { 
                        new CGPoint(_bounds.Left + 3f * _bounds.Width / 4f, _bounds.Top), 
                        new CGPoint(_bounds.Left + 3f * _bounds.Width / 4f, _bounds.Bottom) 
                    });
                ctx.StrokeLineSegments(new []
                    { 
                        new CGPoint(_bounds.Left, _bounds.Top + _bounds.Height / 4f), 
                        new CGPoint(_bounds.Right, _bounds.Top + _bounds.Height / 4f) 
                    });
                ctx.StrokeLineSegments(new []
                    { 
                        new CGPoint(_bounds.Left, _bounds.Top + 3f * _bounds.Height / 4f), 
                        new CGPoint(_bounds.Right, _bounds.Top + 3f * _bounds.Height / 4f) 
                    });

                ctx.RestoreState();

                if (_isActive)
                {
                    ctx.SaveState();

                    UIColor.Blue.SetFill();
                    ctx.FillEllipseInRect(new CGRect(_joystickPositionRaw.X - 20, _joystickPositionRaw.Y - 20, 40, 40));

                    ctx.RestoreState();
                }
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
                _isActive = true;
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
                _isActive = true;
                OnJoystickMove(point.X, point.Y);
                SetNeedsDisplay();
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            _isActive = false;

            System.Diagnostics.Debug.WriteLine("TouchesEnded");
            OnJoystickStop();
            SetNeedsDisplay();
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            _isActive = false;

            System.Diagnostics.Debug.WriteLine("TouchesCancelled");
            OnJoystickStop();
            SetNeedsDisplay();
        }

        private Core.Models.Point GetTouchPoint(UITouch touch)
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

            var scaleX = _bounds.Width / 2f;
            var scaleY = _bounds.Height / 2f;

            x = (x - scaleX) * 100f / scaleX;
            y = (y - scaleY) * 100f / scaleY;

            System.Diagnostics.Debug.WriteLine($"GetTouchPoint: x={x}, y={y}");

            return new Core.Models.Point((float)x, (float)y);
        }

        protected void OnJoystickStart(float x, float y)
        {
            _joystickPosition = new Core.Models.Point(x, y);
            _joystickStartSubject.OnNext(_joystickPosition);
        }

        protected void OnJoystickStop()
        {
            _joystickStopSubject.OnNext(Unit.Default);
        }

        protected void OnJoystickMove(float x, float y)
        {
            _joystickPosition = new Core.Models.Point(x, y);
            _joystickMoveSubject.OnNext(_joystickPosition);
        }
    }
}
