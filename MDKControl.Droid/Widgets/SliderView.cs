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
    public class SliderView : View
    {
        private readonly EventLoopScheduler _scheduler = new EventLoopScheduler();
        private readonly Subject<float> _sliderStartSubject = new Subject<float>();
        private readonly Subject<Unit> _sliderStopSubject = new Subject<Unit>();
        private readonly Subject<float> _sliderMoveSubject = new Subject<float>();

        private RectF _bounds = new RectF();
        private Paint _paintBorder;
        private Paint _paintGrid;
        private Paint _paintSubGrid;
        private Paint _paintGridText;
        private Paint _paintPos;

        private bool _isActive = false;
        private PointF _sliderPositionRaw = new PointF();
        private float _sliderPosition = 0;

        public SliderView(Context context)
            : base(context)
        {
            Initialize();
            Focusable = true;
        }

        public SliderView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Initialize();
            Focusable = true;
        }

        public SliderView(Context context, IAttributeSet attrs)
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

        public IObservable<float> SliderStart { get { return _sliderStartSubject.ObserveOn(_scheduler); } }

        public IObservable<Unit> SliderStop { get { return _sliderStopSubject.ObserveOn(_scheduler); } }

        public IObservable<float> SliderMove { get { return _sliderMoveSubject.ObserveOn(_scheduler); } }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            const int desiredWidth = 300;

            var widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            var widthSize = MeasureSpec.GetSize(widthMeasureSpec);
            var heightMode = MeasureSpec.GetMode(heightMeasureSpec);
            var heightSize = MeasureSpec.GetSize(heightMeasureSpec);

            int width;
            int height;

            if (widthMode == MeasureSpecMode.Exactly)
            {
                width = widthSize;
            }
            else if (widthMode == MeasureSpecMode.AtMost)
            {
                width = widthSize;
            }
            else
            {
                width = desiredWidth;
            }

            if (heightMode == MeasureSpecMode.Exactly)
            {
                height = heightSize;
            }
            else if (heightMode == MeasureSpecMode.AtMost)
            {
                height = Math.Min(width/5, heightSize);
            }
            else
            {
                height = width/5;
            }

            SetMeasuredDimension(width, height);
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
            try
            {
                canvas.DrawRect(_bounds, _paintBorder);

                canvas.DrawLine(_bounds.CenterX(), _bounds.Top, _bounds.CenterX(), _bounds.Bottom, _paintGrid);
                canvas.DrawLine(_bounds.Left, _bounds.CenterY(), _bounds.Right, _bounds.CenterY(), _paintGrid);

                canvas.DrawLine(
                    _bounds.Left + _bounds.Width() / 4f, 
                    _bounds.Top, 
                    _bounds.Left + _bounds.Width() / 4f, 
                    _bounds.Bottom, 
                    _paintSubGrid);
                canvas.DrawLine(_bounds.Left + 3f * _bounds.Width() / 4f, 
                    _bounds.Top, 
                    _bounds.Left + 3f * _bounds.Width() / 4f, 
                    _bounds.Bottom, 
                    _paintSubGrid);

                canvas.DrawText("Slider", _bounds.Left + 4f, _bounds.CenterY() - 4f, _paintGridText);

                if (_isActive)
                {
                    canvas.DrawCircle(_sliderPositionRaw.X, _sliderPositionRaw.Y, 56f, _paintPos);
                }
            }
            catch (Exception)
            {
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            var x = 0f;

            if (e.PointerCount == 1)
            {
                x = e.GetX();

                if (x < _bounds.Left)
                    x = _bounds.Left;
                if (x > _bounds.Right)
                    x = _bounds.Right;

                _sliderPositionRaw = new PointF(x, _bounds.CenterY());
                _isActive = e.Action == MotionEventActions.Down || e.Action == MotionEventActions.Move;

                var scaleX = _bounds.Width() / 2f;

                x = (x - scaleX) * 100f / scaleX;
            }
            else if (e.PointerCount == 2)
            {
                x = (e.GetX(0) + e.GetX(1)) / 2f;

                if (x < _bounds.Left) 
                    x = _bounds.Left;
                if (x > _bounds.Right) 
                    x = _bounds.Right;

                _sliderPositionRaw = new PointF(x, _bounds.CenterY());
                _isActive = e.Action == MotionEventActions.Down || e.Action == MotionEventActions.Move;

                var scaleX = _bounds.Width() / 2f;

                x = (x - scaleX) * 10f / scaleX;
            }
            else
            {
                _isActive = false;
                return true;
            }

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    OnSliderStart(x);
                    break;
                case MotionEventActions.Up:
                    OnSliderStop();
                    break;
                case MotionEventActions.Move:
                    OnSliderMove(x);
                    break;
            }

            Invalidate();
            return true;
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
