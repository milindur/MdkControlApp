using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Activities;
using MDKControl.Droid.Widgets;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MDKControl.Droid.Fragments
{
    public class TimeViewFragment : DialogFragment
    {
        private Button _closeButton;
        private Button _cancelButton;
        private NumberPicker _hoursPicker;
        private NumberPicker _minutesPicker;
        private NumberPicker _secondsPicker;

        public event EventHandler Canceled;
        public event EventHandler<int> Closed;

        private string _titleLabel = "Exposure";
        private int _value;

        public static TimeViewFragment NewInstance(string titleLabel, int value) 
        {
            var args = new Bundle();
            args.PutString("titleLabel", titleLabel);
            args.PutInt("value", value);

            var f = new TimeViewFragment();
            f.Arguments = args;
            f.ShowsDialog = true;

            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _titleLabel = Arguments.GetString("titleLabel");
            _value = Arguments.GetInt("value");

            SetStyle(DialogFragmentStyle.Normal, 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle(_titleLabel);
            
            var view = inflater.Inflate(Resource.Layout.TimeView, container, false);
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            SecondsPicker.MinValue = 0;
            SecondsPicker.MaxValue = 59;
            MinutesPicker.MinValue = 0;
            MinutesPicker.MaxValue = 59;
            HoursPicker.MinValue = 0;
            HoursPicker.MaxValue = 24;

            Value = _value;

            CloseButton.Click += (o, e) => 
                { 
                    var handler = Closed;
                    if (handler != null) handler(this, Value);
                    Dismiss();  
                };
            CancelButton.Click += (o, e) => 
                { 
                    var handler = Canceled;
                    if (handler != null) handler(this, EventArgs.Empty);
                    Dismiss(); 
                };
        }

        protected int Value
        {
            get { return HoursPicker.Value * 60 * 60 + MinutesPicker.Value * 60 + SecondsPicker.Value; }
            set 
            {
                var tmp = value;
                SecondsPicker.Value = tmp % 60;
                tmp /= 60;
                MinutesPicker.Value = tmp % 60;
                tmp /= 60;
                HoursPicker.Value = tmp;
            }
        }

        public NumberPicker HoursPicker
        {
            get
            {
                return _hoursPicker
                    ?? (_hoursPicker = View.FindViewById<NumberPicker>(Resource.Id.Hours));
            }
        }

        public NumberPicker MinutesPicker
        {
            get
            {
                return _minutesPicker
                    ?? (_minutesPicker = View.FindViewById<NumberPicker>(Resource.Id.Minutes));
            }
        }

        public NumberPicker SecondsPicker
        {
            get
            {
                return _secondsPicker
                    ?? (_secondsPicker = View.FindViewById<NumberPicker>(Resource.Id.Seconds));
            }
        }

        public Button CloseButton
        {
            get
            {
                return _closeButton
                    ?? (_closeButton = View.FindViewById<Button>(Resource.Id.Close));
            }
        }

        public Button CancelButton
        {
            get
            {
                return _cancelButton
                    ?? (_cancelButton = View.FindViewById<Button>(Resource.Id.Cancel));
            }
        }
    }
}
    