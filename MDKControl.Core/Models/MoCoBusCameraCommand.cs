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

        SetPreDelay = 0x40,
        SetPanoRepititions = 0x41,

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
        GetCameraTestMode,

        GetPreDelay = 0xe0,
        GetPanoRepititions = 0xe1,
    }
}
