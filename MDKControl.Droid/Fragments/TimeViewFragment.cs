using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MDKControl.Droid.Fragments
{
    public class TimeViewFragment : DialogFragment
    {
        private Button _closeButton;
        private Button _cancelButton;
        private NumberPicker _hoursPicker;
        private NumberPicker _minutesPicker;
        private NumberPicker _secondsPicker;
        private NumberPicker _hundrethsPicker;

        public event EventHandler Canceled;
        public event EventHandler<decimal> Closed;

        private string _titleLabel = "Exposure";
        private decimal _value;

        public static TimeViewFragment NewInstance(string titleLabel, decimal value) 
        {
            var args = new Bundle();
            args.PutString("titleLabel", titleLabel);
            args.PutFloat("value", (float)value);

            var f = new TimeViewFragment
            {
                Arguments = args,
                ShowsDialog = true
            };

            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _titleLabel = Arguments.GetString("titleLabel");
            _value = (decimal)Arguments.GetFloat("value");

            SetStyle(DialogFragmentStyle.Normal, 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle(_titleLabel);
            
            return inflater.Inflate(Resource.Layout.TimeView, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            HundrethsPicker.MinValue = 0;
            HundrethsPicker.MaxValue = 9;
            SecondsPicker.MinValue = 0;
            SecondsPicker.MaxValue = 59;
            MinutesPicker.MinValue = 0;
            MinutesPicker.MaxValue = 59;
            HoursPicker.MinValue = 0;
            HoursPicker.MaxValue = 24;

            Value = _value;

            CloseButton.Click += (o, e) => 
                { 
                    Dismiss();  
                    Closed?.Invoke(this, Value);
                };
            CancelButton.Click += (o, e) => 
                { 
                    Dismiss(); 
                    Canceled?.Invoke(this, EventArgs.Empty);
                };
        }

        protected decimal Value
        {
            get { return HoursPicker.Value * 60m * 60m + MinutesPicker.Value * 60m + SecondsPicker.Value + HundrethsPicker.Value / 10m; }
            set 
            {
                var tmp = (int)(value * 10m);
                HundrethsPicker.Value = tmp % 10;
                tmp /= 10;
                SecondsPicker.Value = tmp % 60;
                tmp /= 60;
                MinutesPicker.Value = tmp % 60;
                tmp /= 60;
                HoursPicker.Value = tmp;
            }
        }

        public NumberPicker HoursPicker => _hoursPicker ?? (_hoursPicker = View.FindViewById<NumberPicker>(Resource.Id.Hours));

        public NumberPicker MinutesPicker => _minutesPicker ?? (_minutesPicker = View.FindViewById<NumberPicker>(Resource.Id.Minutes));

        public NumberPicker SecondsPicker => _secondsPicker ?? (_secondsPicker = View.FindViewById<NumberPicker>(Resource.Id.Seconds));

        public NumberPicker HundrethsPicker => _hundrethsPicker ?? (_hundrethsPicker = View.FindViewById<NumberPicker>(Resource.Id.Hundredths));

        public Button CloseButton => _closeButton ?? (_closeButton = View.FindViewById<Button>(Resource.Id.Close));

        public Button CancelButton => _cancelButton ?? (_cancelButton = View.FindViewById<Button>(Resource.Id.Cancel));
    }
}
    