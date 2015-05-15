using System;
using System.Collections.Generic;
using MDKControl.Core.ViewModels;
using Xamarin.Forms;

namespace MDKControl.Core.Views
{
    public partial class DeviceListView : ContentPage
    {
        public DeviceListView(DeviceListViewModel deviceListViewModel)
        {
            InitializeComponent();

            BindingContext = deviceListViewModel;
        }
    }
}

