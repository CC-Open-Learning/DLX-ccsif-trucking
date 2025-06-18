using UnityEngine;
using UnityEngine.Audio;

namespace VARLab.CCSIF
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;

        private float previousVolume;
        private const float MinVolume = -80.0f;
        private const float StartingVolume = -6.0f;
        private const string MasterTag = "Volume";

        private bool audioEnabled = true;
        public  bool AudioEnabled
        {
            get { return audioEnabled; }
            set { audioEnabled = value; ToggleVolume(value); }
        }

        /// Invoked from <see cref="SettingsMenuSimple.OnSoundTogglePressed"/>
        public void ToggleVolume(bool enabled)
        {
            if (!enabled)
            {
                audioMixer.GetFloat(MasterTag, out previousVolume);
                audioMixer.SetFloat(MasterTag, MinVolume);
            }
            else
            {
                audioMixer.SetFloat(MasterTag, previousVolume);
            }
        }

        /// Invoked from <see cref="SettingsMenuSimple.OnVolumeSliderChanged"/>
        public void ChangeVolume(string volumeTag, float newVolume)
        {
            audioMixer.SetFloat(volumeTag, newVolume);
        }
    }
}
