namespace VARLab.CCSIF
{
    //Used by SettingsSaver, used to store the settings values between scenes
    public static class Settings
    {
        //These variables are set to the current default values inside of the SimpleSettingsMenu SerializedFields
        public static float VolumeSliderValue { get; set; } = 0.5f;
        public static bool SoundToggleValue { get; set; } = true;
        public static float CameraSliderValue { get; set; } = 0.8f;
    }

}