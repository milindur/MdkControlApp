namespace MDKControl.Core.Models
{
    public enum MoCoBusCameraCommand : byte
    {
        NoOp = 0,
        Reserved,
        SetCameraEnable,
        ExposeNow,
        SetTriggerTime,
        SetFocusTime,
        SetMaxShots,
        SetExposureDelay,
        SetFocusWithShutter,
        SetMirrorUp,
        SetInterval,
        SetCameraTestMode,

        GetCameraEnable = 100,
        GetExposingNow,
        GetTriggerTime,
        GetFocusTime,
        GetMaxShots,
        GetExposureDelay,
        GetFocusWithShutter,
        GetMirrorUp,
        GetIntervalTime,
        GetCurrentShots,
        GetCameraTestMode
    }
}
