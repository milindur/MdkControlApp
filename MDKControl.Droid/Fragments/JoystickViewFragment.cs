using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Activities;
using MDKControl.Droid.Widgets;
using Reactive.Bindings;

namespace MDKControl.Droid.Fragments
{
    public class JoystickViewFragment : DialogFragment
    {
        private JoystickView _joystick;
        private SliderView _slider;
        private Button _closeButton;
        private Button _cancelButton;

        public event EventHandler Canceled;
        public event EventHandler Closed;

        private string _closeLabel = "Close";

        public static JoystickViewFragment NewInstance(string closeLabel) 
        {
            var args = new Bundle();
            args.PutString("closeLabel", closeLabel);

            var f = new JoystickViewFragment
            {
                Arguments = args,
                ShowsDialog = true
            };

            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _closeLabel = Arguments.GetString("closeLabel");

            SetStyle(DialogFragmentStyle.NoTitle, 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.JoystickView, container, false);

            view.FindViewById<Button>(Resource.Id.Close).Text = _closeLabel;

            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            CloseButton.Click += (o, e) => 
                {
                    Closed?.Invoke(this, EventArgs.Empty);
                    Dismiss();  
                };
            CancelButton.Click += (o, e) => 
                {
                    Canceled?.Invoke(this, EventArgs.Empty);
                    Dismiss(); 
                };

            Joystick.JoystickStart.SetCommand(Vm.StartJoystickCommand);
            Joystick.JoystickStop.SetCommand(Vm.StopJoystickCommand);
            Joystick.JoystickMove.SetCommand(Vm.MoveJoystickCommand);

            Slider.SliderStart.SetCommand(Vm.StartSliderCommand);
            Slider.SliderStop.SetCommand(Vm.StopSliderCommand);
            Slider.SliderMove.SetCommand(Vm.MoveSliderCommand);
        }

        public override void OnPause()
        {
            Vm.StopJoystickCommand.Execute();
            Vm.StopSliderCommand.Execute();

            base.OnPause();
        }

        public JoystickViewModel Vm => ((DeviceViewActivity)Activity).Vm.JoystickViewModel;

        public JoystickView Joystick => _joystick ?? (_joystick = View.FindViewById<JoystickView>(Resource.Id.Joystick));

        public SliderView Slider => _slider ?? (_slider = View.FindViewById<SliderView>(Resource.Id.Slider));

        public Button CloseButton => _closeButton ?? (_closeButton = View.FindViewById<Button>(Resource.Id.Close));

        public Button CancelButton => _cancelButton ?? (_cancelButton = View.FindViewById<Button>(Resource.Id.Cancel));
    }
}
    