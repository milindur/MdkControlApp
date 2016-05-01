using System;
using System.ComponentModel;
using System.Globalization;
using Foundation;
using UIKit;

namespace MDKControl.iOS
{
    [DesignTimeVisible(true)]
    internal partial class NumberPickerTableViewCell : UITableViewCell
    {
        public NumberPickerTableViewCell(IntPtr handle)
            : base(handle)
        {
        }

        public UIPickerView NumberPicker { get; private set; }

        public NumberPickerViewModel Model { get; private set; }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            NumberPicker = new UIPickerView();
            ContentView.AddSubview(NumberPicker);

            NumberPicker.TranslatesAutoresizingMaskIntoConstraints = false;

            //ContentView.AddConstraint(NSLayoutConstraint.Create(ContentView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 120, 0));
            ContentView.AddConstraint(NSLayoutConstraint.Create(NumberPicker, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Top, 1, 0));
            ContentView.AddConstraint(NSLayoutConstraint.Create(NumberPicker, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Leading, 1, 0));
            ContentView.AddConstraint(NSLayoutConstraint.Create(NumberPicker, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Trailing, 1, 0));
            ContentView.AddConstraint(NSLayoutConstraint.Create(NumberPicker, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Bottom, 1, 0));

            Model = new NumberPickerViewModel(NumberPicker);
            NumberPicker.Model = Model;
        }

        public class NumberPickerViewModel : UIPickerViewModel
        {
            private int _selectedNumber;
            private readonly UIPickerView _numberPickerView;

            public event EventHandler<EventArgs> ValueChanged;

            public NumberPickerViewModel(UIPickerView numberPickerView)
            {
                _numberPickerView = numberPickerView;
            }

            public int SelectedNumber
            { 
                get { return _selectedNumber; }
                set
                {
                    if (_selectedNumber == value)
                        return;
                    
                    _selectedNumber = value;

                    _numberPickerView.ReloadAllComponents();
                    _numberPickerView.Select((_selectedNumber / 1000) % 10, 0, !_numberPickerView.Hidden);
                    _numberPickerView.Select((_selectedNumber / 100) % 10, 1, !_numberPickerView.Hidden);
                    _numberPickerView.Select((_selectedNumber / 10) % 10, 2, !_numberPickerView.Hidden);
                    _numberPickerView.Select(_selectedNumber % 10, 3, !_numberPickerView.Hidden);
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
                    case 0:
                        return 100;
                    case 1:
                        return 10;
                    case 2:
                        return 10;
                    case 3:
                        return 10;
                }
            }

            public override string GetTitle(UIPickerView pickerView, nint row, nint component)
            {
                return row.ToString(CultureInfo.CurrentCulture);
            }

            public override void Selected(UIPickerView pickerView, nint row, nint component)
            {
                _selectedNumber = _selectedNumber - ((int)Math.Floor(_selectedNumber / Math.Pow(10, 3 - (int)component)) % 10) * (int)Math.Pow(10, 3 - (int)component);
                _selectedNumber = _selectedNumber + (int)row * (int)Math.Pow(10, 3 - (int)component);

                ValueChanged?.Invoke(this, new EventArgs());
            }
        }
    }
}
