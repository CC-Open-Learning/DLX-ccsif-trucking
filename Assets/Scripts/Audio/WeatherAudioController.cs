using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace VARLab.CCSIF
{
    public class WeatherAudioController : AudioController
    {
        private AudioSource audioSource;
        private AudioSO currentClip;
        [SerializeField] private List<AudioSO> audioClips;
        [SerializeField] private WeatherController weatherController;
        public UnityEvent OnPlayAudio;
        public UnityEvent OnPauseAudio;

        void Start()
        {
            if (OnPlayAudio == null) { OnPlayAudio = new UnityEvent(); }
            if (OnPauseAudio == null) { OnPauseAudio = new UnityEvent(); }
            if (!TryGetComponent<AudioSource>(out audioSource))
            {
                Debug.LogWarning("Warning: WeatherAudioController is not attached to an instance of AudioSource");
                return;
            }
            determineWeatherSounds();
            PlayAudio();
        }

        public override void PauseAudio()
        {
            audioSource.Pause();
            OnPauseAudio?.Invoke();
        }

        public void PlayAudio()
        {
            loadOptions();
            audioSource.Play();
            OnPlayAudio?.Invoke();
        }

        private void determineWeatherSounds()
        {
            if (weatherController.CurrentWeather == WeatherType.Sunny)
                currentClip = audioClips[0];
            if (weatherController.CurrentWeather == WeatherType.Rainy)
                currentClip = audioClips[1];
        }

        //This command "Loads" the settings present in AudioSO into the AudioSource Controller
        //Can be expanded on as needed, and should match everything present in AudioSO
        protected override void loadOptions()
        {
            audioSource.loop = currentClip.Loop;
            audioSource.clip = currentClip.clip;
        }
    }
}
