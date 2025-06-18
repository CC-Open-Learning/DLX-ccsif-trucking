using UnityEngine;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.CCSIF
{
    public class SettingsSaver : MonoBehaviour
    {
        [SerializeField] private SettingsMenu settingsMenu;

        //Called by SceneSwitcher OnSceneExit
        public void SaveSettings()
        {
            Settings.VolumeSliderValue = settingsMenu.Root.Q<TemplateContainer>("VolumeSlider").Q<FillSlider>().value;
            Settings.SoundToggleValue = settingsMenu.Root.Q<TemplateContainer>("SoundToggle").Q<SlideToggle>().value;
            Settings.CameraSliderValue = settingsMenu.Root.Q<TemplateContainer>("SensitivitySlider").Q<FillSlider>().value;
        }

        //Called by SceneSwitcher OnSceneEnter
        public void LoadSettings()
        {
            settingsMenu.Root.Q<TemplateContainer>("VolumeSlider").Q<FillSlider>().value = Settings.VolumeSliderValue;
            settingsMenu.Root.Q<TemplateContainer>("SoundToggle").Q<SlideToggle>().value = Settings.SoundToggleValue;
            settingsMenu.Root.Q<TemplateContainer>("SensitivitySlider").Q<FillSlider>().value = Settings.CameraSliderValue;
        }
    }
}