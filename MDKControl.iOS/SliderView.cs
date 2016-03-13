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
    [Register("SliderView"), DesignTimeVisible(true)]
    public class SliderView : UIView
    {
        private const int padding = 10;

        private readonly EventLoopScheduler _scheduler = new EventLoopScheduler();
        private readonly Subject<float> _sliderStartSubject = new Subject<float>();
        private readonly Subject<Unit> _sliderStopSubject = new Subject<Unit>();
        private readonly Subject<float> _sliderMoveSubject = new Subject<float>();

        private CGRect _bounds = new CGRect();

        private bool _isActive = false;
        private CGPoint _sliderPositionRaw = new CGPoint();
        private float _sliderPosition = 0;

        public SliderView(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        public IObservable<float> SliderStart { get { return _sliderStartSubject.ObserveOn(_scheduler); } }

        public IObservable<Unit> SliderStop { get { return _sliderStopSubject.ObserveOn(_scheduler); } }

        public IObservable<float> SliderMove { get { return _sliderMoveSubject.ObserveOn(_scheduler); } }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            _bounds = new CGRect(rect.X + padding, rect.Y + padding, rect.Width - 2 * padding, rect.Height - 2 * padding);

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

                ctx.RestoreState();

                if (_isActive)
                {
                    ctx.SaveState();

                    UIColor.Blue.SetFill();
                    ctx.FillEllipseInRect(new CGRect(_sliderPositionRaw.X - 20, _sliderPositionRaw.Y - 20, 40, 40));

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
                OnSliderStart(point);
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
                OnSliderMove(point);
                SetNeedsDisplay();
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            _isActive = false;

            System.Diagnostics.Debug.WriteLine("TouchesEnded");
            OnSliderStop();
            SetNeedsDisplay();
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            _isActive = false;

            System.Diagnostics.Debug.WriteLine("TouchesCancelled");
            OnSliderStop();
            SetNeedsDisplay();
        }

        private float GetTouchPoint(UITouch touch)
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

            _sliderPositionRaw = new CGPoint(x, _bounds.GetMidY());

            var scaleX = _bounds.Width / 2f;

            x = (x - scaleX) * 100f / scaleX;

            System.Diagnostics.Debug.WriteLine($"GetTouchPoint: x={x}");

            return (float)x;
        }

        protected void OnSliderStart(float x)
        {
            _sliderPosition = x;
            _sliderStartSubject.OnNext(_sliderPosition);
        }

        protected void OnSliderStop()
        {
            _sliderStopSubject.OnNext(Unit.Default);
        }

        protected void OnSliderMove(float x)
        {
            _sliderPosition = x;
            _sliderMoveSubject.OnNext(_sliderPosition);
        }
    }
}
