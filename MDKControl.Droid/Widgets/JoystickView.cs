using System;
using Android.Views;
using Android.Content;
using Android.Util;
using Android.Graphics;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace MDKControl.Droid.Widgets
{
    public class JoystickView : View
    {
        private readonly Subject<MDKControl.Core.Models.Point> _joystickMoveSubject = new Subject<MDKControl.Core.Models.Point>();

        private RectF _bounds = new RectF();
        private Paint _paint;

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
            _paint = new Paint(PaintFlags.AntiAlias);
            _paint.Color = Color.Red;
            _paint.SetStyle(Paint.Style.Stroke);
            _paint.StrokeWidth = 5;
        }

        public MDKControl.Core.Models.Point JoystickPosition 
        {
            get { return _joystickPosition; } 
            set { _joystickPosition = value; } 
        }

        public IObservable<MDKControl.Core.Models.Point> JoystickMove { get { return _joystickMoveSubject.AsObservable(); } }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, widthMeasureSpec);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            var xpad = (float) (PaddingLeft + PaddingRight);
            var ypad = (float) (PaddingTop + PaddingBottom);

            var ww = (float) w - xpad;
            var hh = (float) h - ypad;

            _bounds = new RectF(0.0f, 0.0f, ww, hh);
            _bounds.OffsetTo(PaddingLeft, PaddingTop);
        }

        protected override void OnDraw(Canvas canvas)
        {
            canvas.DrawRect(_bounds, _paint);
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
            //JoystickStart?.Invoke(this, EventArgs.Empty);
        }

        protected void OnJoystickStop()
        {
            //JoystickStop?.Invoke(this, EventArgs.Empty);
        }

        protected void OnJoystickMove(float x, float y, float z)
        {
            _joystickPosition = new MDKControl.Core.Models.Point(x, y, z);
            _joystickMoveSubject.OnNext(_joystickPosition); 
        }
    }
}
    