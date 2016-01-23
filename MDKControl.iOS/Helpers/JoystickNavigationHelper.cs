using System;
using MDKControl.Core.ViewModels;

namespace MDKControl.iOS.Helpers
{
    public class JoystickNavigationHelper
    {
        public JoystickNavigationHelper(JoystickViewModel _joystickViewModel, Action _dismissed)
        {
            JoystickViewModel = _joystickViewModel;
            Dismissed = _dismissed;
        }
        
        public JoystickViewModel JoystickViewModel { get; private set; }
        public Action Dismissed { get; private set; }
    }
}
