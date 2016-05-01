using System;
using System.ComponentModel;
using System.Globalization;
using Foundation;
using UIKit;

namespace MDKControl.iOS
{
    [DesignTimeVisible(true)]
    internal partial class TimePickerTableViewCell : UITableViewCell
    {
        public TimePickerTableViewCell(IntPtr handle)
            : base(handle)
        {
        }

        public UIPickerView TimePicker { get; private set; }

        public TimePickerViewModel Model { get; private set; }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            TimePicker = new UIPickerView();
            ContentView.AddSubview(TimePicker);

            TimePicker.TranslatesAutoresizingMaskIntoConstraints = false;

            //ContentView.AddConstraint(NSLayoutConstraint.Create(ContentView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 120, 0));
            ContentView.AddConstraint(NSLayoutConstraint.Create(TimePicker, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Top, 1, 0));
            ContentView.AddConstraint(NSLayoutConstraint.Create(TimePicker, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Leading, 1, 0));
            ContentView.AddConstraint(NSLayoutConstraint.Create(TimePicker, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Trailing, 1, 0));
            ContentView.AddConstraint(NSLayoutConstraint.Create(TimePicker, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Bottom, 1, 0));

            Model = new TimePickerViewModel(TimePicker);
            TimePicker.Model = Model;
        }

        public class TimePickerViewModel : UIPickerViewModel
        {
            private TimeSpan _selectedTime;
            private readonly UIPickerView _timePickerView;

            public event EventHandler<EventArgs> ValueChanged;

            public TimePickerViewModel(UIPickerView timePickerView)
            {
                _timePickerView = timePickerView;
            }

            public TimeSpan SelectedTime
            { 
                get { return _selectedTime; }
                set
                {
                    if (_selectedTime == value)
                        return;
                    
                    _selectedTime = value;

                    _timePickerView.ReloadAllComponents();
                    _timePickerView.Select(_selectedTime.Hours, 0, !_timePickerView.Hidden);
                    _timePickerView.Select(_selectedTime.Minutes, 1, !_timePickerView.Hidden);
                    _timePickerView.Select(_selectedTime.Seconds, 2, !_timePickerView.Hidden);
                    _timePickerView.Select(_selectedTime.Milliseconds / 100, 3, !_timePickerView.Hidden);
                }
            }

            public override nint GetComponentCount(UIPickerView pickerView)
            {
                return 4;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                switch (component)
                {
                    default:
                        return 0;
                    case 0: // hours
                        return 24;
                    case 1: // minutes
                        return 60;
                    case 2: // seconds
                        return 60;
                    case 3: // milliseconds
                        return 10;
                }
            }

            public override string GetTitle(UIPickerView pickerView, nint row, nint component)
            {
                switch (component)
                {
                    default:
                        return string.Empty;
                    case 0: // hours
                        return row.ToString("00'h'");
                    case 1: // minutes
                        return row.ToString("00'm'");
                    case 2: // seconds
                        return row.ToString("00's'");
                    case 3: // milliseconds
                        return "." + row.ToString(CultureInfo.InvariantCulture);
                }
            }

            public override void Selected(UIPickerView pickerView, nint row, nint component)
            {
                switch (component)
                {
                    case 0: // hours
                        _selectedTime = new TimeSpan(SelectedTime.Days, (int)row, SelectedTime.Minutes, SelectedTime.Seconds, SelectedTime.Milliseconds / 100);
                        break;
                    case 1: // minutes
                        _selectedTime = new TimeSpan(SelectedTime.Days, SelectedTime.Hours, (int)row, SelectedTime.Seconds, SelectedTime.Milliseconds / 100);
                        break;
                    case 2: // seconds
                        _selectedTime = new TimeSpan(SelectedTime.Days, SelectedTime.Hours, SelectedTime.Minutes, (int)row, SelectedTime.Milliseconds / 100);
                        break;
                    case 3: // milliseconds
                        _selectedTime = new TimeSpan(SelectedTime.Days, SelectedTime.Hours, SelectedTime.Minutes, SelectedTime.Seconds, (int)row * 100);
                        break;
                }

                ValueChanged?.Invoke(this, new EventArgs());
            }
        }
    }
}
