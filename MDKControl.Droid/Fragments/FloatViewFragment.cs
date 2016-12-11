using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MDKControl.Droid.Fragments
{
    public class FloatViewFragment : DialogFragment
    {
        private Button _closeButton;
        private Button _cancelButton;
        private NumberPicker _hundredPicker;
        private NumberPicker _tenPicker;
        private NumberPicker _onePicker;
        private NumberPicker _tenthPicker;
        private NumberPicker _hundredthsPicker;

        public event EventHandler Canceled;
        public event EventHandler<float> Closed;

        private string _titleLabel = "Shots";
        private float _value;

        public static FloatViewFragment NewInstance(string titleLabel, float value) 
        {
            var args = new Bundle();
            args.PutString("titleLabel", titleLabel);
            args.PutFloat("value", value);

            var f = new FloatViewFragment
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
            _value = Arguments.GetFloat("value");

            SetStyle(DialogFragmentStyle.Normal, 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle(_titleLabel);
            
            var view = inflater.Inflate(Resource.Layout.FloatView, container, false);
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            HundredthsPicker.MinValue = 0;
            HundredthsPicker.MaxValue = 9;
            TenthPicker.MinValue = 0;
            TenthPicker.MaxValue = 9;
            OnePicker.MinValue = 0;
            OnePicker.MaxValue = 9;
            TenPicker.MinValue = 0;
            TenPicker.MaxValue = 9;
            HundredPicker.MinValue = 0;
            HundredPicker.MaxValue = 9;

            Value = _value;

            CloseButton.Click += (o, e) => 
                { 
                    Closed?.Invoke(this, Value);
                    Dismiss();  
                };
            CancelButton.Click += (o, e) => 
                { 
                    Canceled?.Invoke(this, EventArgs.Empty);
                    Dismiss(); 
                };
        }

        protected float Value
        {
            get { return HundredPicker.Value * 100f + TenPicker.Value * 10f + OnePicker.Value + TenthPicker.Value / 10f + HundredthsPicker.Value / 100f; }
            set 
            {
                HundredPicker.Value = (int)value / 100;
                TenPicker.Value = ((int)value % 100) / 10;
                OnePicker.Value = (int)value % 10;
                TenthPicker.Value = (int)(value * 10) % 10;
                HundredthsPicker.Value = (int)(value * 100) % 10;
            }
        }

        public NumberPicker HundredthsPicker => _hundredthsPicker ?? (_hundredthsPicker = View.FindViewById<NumberPicker>(Resource.Id.Hundredths));

        public NumberPicker TenthPicker => _tenthPicker ?? (_tenthPicker = View.FindViewById<NumberPicker>(Resource.Id.Tenth));

        public NumberPicker OnePicker => _onePicker ?? (_onePicker = View.FindViewById<NumberPicker>(Resource.Id.One));

        public NumberPicker TenPicker => _tenPicker ?? (_tenPicker = View.FindViewById<NumberPicker>(Resource.Id.Ten));

        public NumberPicker HundredPicker => _hundredPicker ?? (_hundredPicker = View.FindViewById<NumberPicker>(Resource.Id.Hundred));

        public Button CloseButton => _closeButton ?? (_closeButton = View.FindViewById<Button>(Resource.Id.Close));

        public Button CancelButton => _cancelButton ?? (_cancelButton = View.FindViewById<Button>(Resource.Id.Cancel));
    }
}
    