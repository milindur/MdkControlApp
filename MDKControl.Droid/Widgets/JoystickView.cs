using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace MDKControl.Droid.Widgets
{
    public class JoystickView : View
    {
        private readonly EventLoopScheduler _scheduler = new EventLoopScheduler(); 
        private readonly Subject<Unit> _joystickStartSubject = new Subject<Unit>();
        private readonly Subject<Unit> _joystickStopSubject = new Subject<Unit>();
        private readonly Subject<MDKControl.Core.Models.Point> _joystickMoveSubject = new Subject<MDKControl.Core.Models.Point>();

        private RectF _bounds = new RectF();
        private Paint _paintGrid;

        private MDKControl.Core.Models.Point _joystickPosition = new MDKControl.Core.Models.Point(0, 0, 0);

        public JoystickView(Context context)
            : base(context)
        {
            Initialize();
            Focusable = true;
        }

        public JoystickView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Initialize();
            Focusable = true;
        }

        public JoystickView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize();
            Focusable = true;
        }

        private void Initialize()
        {
            _paintGrid = new Paint(PaintFlags.AntiAlias) { Color = Color.Red };
            _paintGrid.SetStyle(Paint.Style.Stroke);
            _paintGrid.SetPathEffect(new DashPathEffect(new float[] { 10, 20 }, 0));
            _paintGrid.StrokeWidth = 5;
        }

        public MDKControl.Core.Models.Point JoystickPosition
        {
            get { return _joystickPosition; }
            set { _joystickPosition = value; }
        }

        public IObservable<Unit> JoystickStart { get { return _joystickStartSubject.ObserveOn(_scheduler); } }
        public IObservable<Unit> JoystickStop { get { return _joystickStopSubject.ObserveOn(_scheduler); } }
        public IObservable<MDKControl.Core.Models.Point> JoystickMove { get { return _joystickMoveSubject.ObserveOn(_scheduler); } }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, widthMeasureSpec);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            var xpad = (float)(PaddingLeft + PaddingRight);
            var ypad = (float)(PaddingTop + PaddingBottom);

            var ww = (float)w - xpad;
            var hh = (float)h - ypad;

            _bounds = new RectF(0.0f, 0.0f, ww, hh);
            _bounds.OffsetTo(PaddingLeft, PaddingTop);
        }

        protected override void OnDraw(Canvas canvas)
        {
            canvas.DrawRect(_bounds, _paintGrid);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            var x = 0f;
            var y = 0f;
            var z = 0f;

            if (e.PointerCount == 1)
            {
                x = e.GetX();
                y = e.GetY();

                if (x < _bounds.Left) x = _bounds.Left;
                if (x > _bounds.Right) x = _bounds.Right;
                if (y < _bounds.Top) y = _bounds.Top;
                if (y > _bounds.Bottom) y = _bounds.Bottom;

                var scaleX = _bounds.Width() / 2f;
                var scaleY = _bounds.Height() / 2f;

                x = (x - scaleX) * 100f / scaleX;
                y = (y - scaleY) * 100f / scaleY;
            }
            else if (e.PointerCount == 2)
            {
                z = (e.GetX(0) + e.GetX(1)) / 2f;

                if (z < _bounds.Left) z = _bounds.Left;
                if (z > _bounds.Right) z = _bounds.Right;

                var scaleZ = _bounds.Width() / 2f;

                z = (z - scaleZ) * 100f / scaleZ;
            }
            else
            {
                return true;
            }

            /*x = (float)Math.Round(x / 5f) * 5f;
            y = (float)Math.Round(y / 5f) * 5f;
            z = (float)Math.Round(z / 5f) * 5f;*/

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    OnJoystickStart();
                    OnJoystickMove(x, y, z);
                    break;
                case MotionEventActions.Up:
                    OnJoystickMove(0, 0, 0);
                    OnJoystickStop();
                    break;
                case MotionEventActions.Move:
                    OnJoystickMove(x, y, z);
                    break;
            }

            Invalidate();
            return true;
        }

        protected void OnJoystickStart()
        {
            _joystickStartSubject.OnNext(Unit.Default);
        }

        protected void OnJoystickStop()
        {
            _joystickStopSubject.OnNext(Unit.Default);
        }

        protected void OnJoystickMove(float x, float y, float z)
        {
            _joystickPosition = new MDKControl.Core.Models.Point(x, y, z);
            _joystickMoveSubject.OnNext(_joystickPosition);
        }
    }
}
