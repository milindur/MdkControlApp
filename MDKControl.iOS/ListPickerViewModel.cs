using System;
using System.Collections.Generic;
using UIKit;

namespace MDKControl.iOS
{
    public class ListPickerViewModel<TItem> : UIPickerViewModel
    {
        public TItem SelectedItem { get; private set; }
        public int SelectedIndex { get; private set; }

        public event EventHandler<EventArgs> ValueChanged;

        IList<TItem> _items;
        public IList<TItem> Items
        {
            get { return _items; }
            set { _items = value; Selected(null, 0, 0); }
        }

        public ListPickerViewModel()
        {
        }

        public ListPickerViewModel(IList<TItem> items)
        {
            Items = items;
        }

        public override nint GetRowsInComponent(UIPickerView picker, nint component)
        {
            if (NoItem())
                return 1;
            return Items.Count;
        }

        public override string GetTitle(UIPickerView picker, nint row, nint component)
        {
            if (NoItem((int)row))
                return "";
            var item = Items[(int)row];
            return GetTitleForItem(item);
        }

        public override void Selected(UIPickerView picker, nint row, nint component)
        {
            if (NoItem((int)row))
            {
                SelectedItem = default(TItem);
                SelectedIndex = -1;
            }
            else
            {
                SelectedItem = Items[(int)row];
                SelectedIndex = (int)row;
            }

            ValueChanged?.Invoke(this, new EventArgs());
        }

        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public virtual string GetTitleForItem(TItem item)
        {
            return item.ToString();
        }

        bool NoItem(int row = 0)
        {
            return Items == null || row >= Items.Count;
        }
    }
}
