using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MDKControl.Core.Models;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Activities;
using MDKControl.Droid.Widgets;
using Reactive.Bindings;

namespace MDKControl.Droid.Fragments
{
    public class JoystickViewFragment : DialogFragment
    {
        private RadioButton _allAxisRadioButton;
        private RadioButton _panAxisRadioButton;
        private RadioButton _tiltAxisRadioButton;
        private JoystickView _joystick;
        private SliderView _slider;
        private Button _closeButton;
        private Button _cancelButton;

        private Binding _allAxisRadioBinding;
        private Binding _panAxisRadioBinding;
        private Binding _tiltAxisRadioBinding;

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

        public override void OnResume()
        {
            base.OnResume();

            if (JoystickView.Motors.HasFlag(Motors.MotorPan) && JoystickView.Motors.HasFlag(Motors.MotorTilt))
            {
                AllAxisRadioButton.Checked = true;
            }
            else if (JoystickView.Motors.HasFlag(Motors.MotorPan))
            {
                PanAxisRadioButton.Checked = true;
            }
            else if (JoystickView.Motors.HasFlag(Motors.MotorTilt))
            {
                TiltAxisRadioButton.Checked = true;
            }

            _allAxisRadioBinding = this.SetBinding(() => AllAxisRadioButton.Checked)
                .WhenSourceChanges(() =>
                {
                    if (AllAxisRadioButton.Checked)
                    {
                        JoystickView.Motors = Motors.MotorPan | Motors.MotorTilt;
                    }
                });

            _panAxisRadioBinding = this.SetBinding(() => PanAxisRadioButton.Checked)
                .WhenSourceChanges(() =>
                {
                    if (PanAxisRadioButton.Checked)
                    {
                        JoystickView.Motors = Motors.MotorPan;
                    }
                });

            _tiltAxisRadioBinding = this.SetBinding(() => TiltAxisRadioButton.Checked)
                .WhenSourceChanges(() =>
                {
                    if (TiltAxisRadioButton.Checked)
                    {
                        JoystickView.Motors = Motors.MotorTilt;
                    }
                });
        }

        public override void OnPause()
        {
            Vm.StopJoystickCommand.Execute();
            Vm.StopSliderCommand.Execute();

            _allAxisRadioBinding?.Detach();
            _panAxisRadioBinding?.Detach();
            _tiltAxisRadioBinding?.Detach();

            base.OnPause();
        }

        public JoystickViewModel Vm => ((DeviceViewActivity)Activity).Vm.JoystickViewModel;

        public RadioButton AllAxisRadioButton => _allAxisRadioButton ?? (_allAxisRadioButton = View.FindViewById<RadioButton>(Resource.Id.AllAxis));

        public RadioButton PanAxisRadioButton => _panAxisRadioButton ?? (_panAxisRadioButton = View.FindViewById<RadioButton>(Resource.Id.PanAxis));

        public RadioButton TiltAxisRadioButton => _tiltAxisRadioButton ?? (_tiltAxisRadioButton = View.FindViewById<RadioButton>(Resource.Id.TiltAxis));

        public JoystickView Joystick => _joystick ?? (_joystick = View.FindViewById<JoystickView>(Resource.Id.Joystick));

        public SliderView Slider => _slider ?? (_slider = View.FindViewById<SliderView>(Resource.Id.Slider));

        public Button CloseButton => _closeButton ?? (_closeButton = View.FindViewById<Button>(Resource.Id.Close));

        public Button CancelButton => _cancelButton ?? (_cancelButton = View.FindViewById<Button>(Resource.Id.Cancel));
    }
}
    