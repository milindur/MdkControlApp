using System;
using MDKControl.Core.ViewModels;

namespace MDKControl.iOS.Helpers
{
    public class JoystickNavigationHelper
    {
        public JoystickNavigationHelper(JoystickViewModel joystickViewModel, Action dismissed)
        {
            JoystickViewModel = joystickViewModel;
            Dismissed = dismissed;
        }
        
        public JoystickViewModel JoystickViewModel { get; private set; }
        public Action Dismissed { get; private set; }
    }
}
