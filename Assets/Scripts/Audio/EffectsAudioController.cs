using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace VARLab.CCSIF
{
    public class EffectsAudioController : AudioController
    {
        private AudioSource audioSource;
        private AudioSO currentClip;
        [SerializeField] private List<AudioSO> audioClips;
        public UnityEvent OnPlayAudio;
        public UnityEvent OnPauseAudio;

        private const string PitchTag = "Pitch";

        private const float minPitch = 0.9f;
        private const float maxPitch = 1.1f;

        void Start()
        {
            if (OnPlayAudio == null) { OnPlayAudio = new UnityEvent(); }
            if (OnPauseAudio == null) { OnPauseAudio = new UnityEvent(); }
            if (!TryGetComponent<AudioSource>(out audioSource))
            {
                Debug.LogWarning("Warning: EffectsAudioController is not attached to an instance of AudioSource");
                return;
            }
        }

        public override void PauseAudio()
        {
            audioSource.Pause();
            OnPauseAudio?.Invoke();
        }

        public void PlayUIClickAudio()
        {
            currentClip = audioClips[0];
            loadOptions();
            audioSource.Play();
            OnPlayAudio?.Invoke();
        }
        public void PlayInspectionFixedShortAudio()
        {
            currentClip = audioClips[1];
            loadOptions();
            audioSource.Play();
            OnPlayAudio?.Invoke();
        }
        public void PlayInspectionFixedLongAudio()
        {
            currentClip = audioClips[2];
            loadOptions();
            audioSource.Play();
            OnPlayAudio?.Invoke();
        }

        //This command "Loads" the settings present in AudioSO into the AudioSource Controller
        //Can be expanded on as needed, and should match everything present in AudioSO
        protected override void loadOptions()
        {
            audioSource.loop = currentClip.Loop;
            audioSource.clip = currentClip.clip;
            float randomPitch = Random.Range(minPitch, maxPitch);
            audioSource.outputAudioMixerGroup.audioMixer.SetFloat(PitchTag, randomPitch);
        }
    }
}
