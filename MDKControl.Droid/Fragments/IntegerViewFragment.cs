using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MDKControl.Droid.Fragments
{
    public class IntegerViewFragment : DialogFragment
    {
        private Button _closeButton;
        private Button _cancelButton;
        private NumberPicker _thousandPicker;
        private NumberPicker _hundredPicker;
        private NumberPicker _tenPicker;
        private NumberPicker _onePicker;

        public event EventHandler Canceled;
        public event EventHandler<int> Closed;

        private string _titleLabel = "Shots";
        private int _value;

        public static IntegerViewFragment NewInstance(string titleLabel, int value) 
        {
            var args = new Bundle();
            args.PutString("titleLabel", titleLabel);
            args.PutInt("value", value);

            var f = new IntegerViewFragment
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
            _value = Arguments.GetInt("value");

            SetStyle(DialogFragmentStyle.Normal, 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle(_titleLabel);
            
            var view = inflater.Inflate(Resource.Layout.IntegerView, container, false);
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            OnePicker.MinValue = 0;
            OnePicker.MaxValue = 9;
            TenPicker.MinValue = 0;
            TenPicker.MaxValue = 9;
            HundredPicker.MinValue = 0;
            HundredPicker.MaxValue = 9;
            ThousandPicker.MinValue = 0;
            ThousandPicker.MaxValue = 99;

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

        protected int Value
        {
            get { return ThousandPicker.Value * 1000 + HundredPicker.Value * 100 + TenPicker.Value * 10 + OnePicker.Value; }
            set 
            {
                ThousandPicker.Value = value / 1000;
                HundredPicker.Value = (value % 1000) / 100;
                TenPicker.Value = (value % 100) / 10;
                OnePicker.Value = value % 10;
            }
        }

        public NumberPicker OnePicker => _onePicker ?? (_onePicker = View.FindViewById<NumberPicker>(Resource.Id.One));

        public NumberPicker TenPicker => _tenPicker ?? (_tenPicker = View.FindViewById<NumberPicker>(Resource.Id.Ten));

        public NumberPicker HundredPicker => _hundredPicker ?? (_hundredPicker = View.FindViewById<NumberPicker>(Resource.Id.Hundred));

        public NumberPicker ThousandPicker => _thousandPicker ?? (_thousandPicker = View.FindViewById<NumberPicker>(Resource.Id.Thousand));

        public Button CloseButton => _closeButton ?? (_closeButton = View.FindViewById<Button>(Resource.Id.Close));

        public Button CancelButton => _cancelButton ?? (_cancelButton = View.FindViewById<Button>(Resource.Id.Cancel));
    }
}
    