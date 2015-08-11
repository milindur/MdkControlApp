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
    public class JoystickViewFragment : DialogFragment
    {
        private JoystickView _joystick;
        private Button _closeButton;
        private Button _cancelButton;

        public event EventHandler Canceled;
        public event EventHandler Closed;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetStyle(DialogFragmentStyle.NoTitle, 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.JoystickView, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            CloseButton.Click += (o, e) => 
                { 
                    var handler = Closed;
                    if (handler != null) handler(this, EventArgs.Empty);
                    Dismiss();  
                };
            CancelButton.Click += (o, e) => 
                { 
                    var handler = Canceled;
                    if (handler != null) handler(this, EventArgs.Empty);
                    Dismiss(); 
                };

            Joystick.JoystickStart.SetCommand(Vm.StartJoystickCommand);
            Joystick.JoystickStop.SetCommand(Vm.StopJoystickCommand);
            Joystick.JoystickMove.SetCommand(Vm.MoveJoystickCommand);
        }

        public JoystickViewModel Vm
        {
            get
            {
                return ((DeviceViewActivity)Activity).Vm.JoystickViewModel;
            }
        }

        public JoystickView Joystick
        {
            get
            {
                return _joystick
                    ?? (_joystick = View.FindViewById<JoystickView>(Resource.Id.Joystick));
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
    