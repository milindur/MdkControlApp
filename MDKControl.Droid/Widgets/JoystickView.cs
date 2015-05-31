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
        private readonly Subject<MDKControl.Core.Models.Point> _joystickStartSubject = new Subject<MDKControl.Core.Models.Point>();
        private readonly Subject<Unit> _joystickStopSubject = new Subject<Unit>();
        private readonly Subject<MDKControl.Core.Models.Point> _joystickMoveSubject = new Subject<MDKControl.Core.Models.Point>();

        private RectF _bounds = new RectF();
        private Paint _paintBorder;
        private Paint _paintGrid;
        private Paint _paintSubGrid;
        private Paint _paintGridText;
        private Paint _paintPos;

        private bool _isActive = false;
        private bool _isSlider = false;
        private PointF _joystickPositionRaw = new PointF();
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
            float density = Context.Resources.DisplayMetrics.Density;
            float scaledDensity = Context.Resources.DisplayMetrics.ScaledDensity;

            SetLayerType(LayerType.Software, null);

            _paintBorder = new Paint(PaintFlags.AntiAlias) { Color = Color.DarkGray };

            _paintGrid = new Paint(PaintFlags.AntiAlias) { Color = Color.LightGray };
            _paintGrid.SetStyle(Paint.Style.Stroke);
            _paintGrid.SetPathEffect(new DashPathEffect(new float[] { 5, 5 }, 0));
            _paintGrid.StrokeWidth = 5;

            _paintSubGrid = new Paint(PaintFlags.AntiAlias) { Color = Color.LightGray };
            _paintSubGrid.SetStyle(Paint.Style.Stroke);
            _paintSubGrid.SetPathEffect(new DashPathEffect(new float[] { 5, 5 }, 0));
            _paintSubGrid.StrokeWidth = 2;

            _paintPos = new Paint(PaintFlags.AntiAlias) { Color = Color.CornflowerBlue };

            _paintGridText = new Paint(PaintFlags.AntiAlias) { Color = Color.LightGray };
            _paintGridText.TextSize = 20 * scaledDensity;
        }

        public MDKControl.Core.Models.Point JoystickPosition
        {
            get { return _joystickPosition; }
            set { _joystickPosition = value; }
        }

        public IObservable<MDKControl.Core.Models.Point> JoystickStart { get { return _joystickStartSubject.ObserveOn(_scheduler); } }
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

            _bounds = new RectF(0f, 0f, ww, hh);
            _bounds.OffsetTo(PaddingLeft, PaddingTop);
        }

        protected override void OnDraw(Canvas canvas)
        {
            canvas.DrawRect(_bounds, _paintBorder);

            canvas.DrawLine(_bounds.CenterX(), _bounds.Top, _bounds.CenterX(), _bounds.Bottom, _paintGrid);
            canvas.DrawLine(_bounds.Left, _bounds.CenterY(), _bounds.Right, _bounds.CenterY(), _paintGrid);

            canvas.DrawLine(_bounds.Left + _bounds.Width() / 4f, _bounds.Top, _bounds.Left + _bounds.Width() / 4f, _bounds.Bottom, _paintSubGrid);
            canvas.DrawLine(_bounds.Left + 3f * _bounds.Width() / 4f, _bounds.Top, _bounds.Left + 3f * _bounds.Width() / 4f, _bounds.Bottom, _paintSubGrid);
            canvas.DrawLine(_bounds.Left, _bounds.Top + _bounds.Height() / 4f, _bounds.Right, _bounds.Top + _bounds.Height() / 4f, _paintSubGrid);
            canvas.DrawLine(_bounds.Left, _bounds.Top + 3f * _bounds.Height() / 4f, _bounds.Right, _bounds.Top + 3f * _bounds.Height() / 4f, _paintSubGrid);

            if (_isSlider)
            {
                canvas.DrawText("Slider", _bounds.Left + 4f, _bounds.CenterY() - 4f, _paintGridText);
            }
            else
            {
                canvas.DrawText("Pan", _bounds.Left + 4f, _bounds.CenterY() - 4f, _paintGridText);

                var tmpRect = new Rect();
                _paintGridText.GetTextBounds("Tilt", 0, 4, tmpRect);
                canvas.DrawText("Tilt", _bounds.CenterX() + 4, _bounds.Top + tmpRect.Height() + 4f, _paintGridText);
            }

            if (_isActive)
            {
                canvas.DrawCircle(_joystickPositionRaw.X, _joystickPositionRaw.Y, 56f, _paintPos);
            }
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

                _joystickPositionRaw = new PointF(x, y);
                _isSlider = false;
                _isActive = e.Action == MotionEventActions.Down || e.Action == MotionEventActions.Move;

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

                _joystickPositionRaw = new PointF(z, _bounds.CenterY());
                _isSlider = true;
                _isActive = e.Action == MotionEventActions.Down || e.Action == MotionEventActions.Move;

                var scaleZ = _bounds.Width() / 2f;

                z = (z - scaleZ) * 100f / scaleZ;
            }
            else
            {
                _isSlider = false;
                _isActive = false;
                return true;
            }

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    OnJoystickStart(x, y, z);
                    break;
                case MotionEventActions.Up:
                    OnJoystickStop();
                    break;
                case MotionEventActions.Move:
                    OnJoystickMove(x, y, z);
                    break;
            }

            Invalidate();
            return true;
        }

        protected void OnJoystickStart(float x, float y, float z)
        {
            _joystickPosition = new MDKControl.Core.Models.Point(x, y, z);
            _joystickStartSubject.OnNext(_joystickPosition);
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
