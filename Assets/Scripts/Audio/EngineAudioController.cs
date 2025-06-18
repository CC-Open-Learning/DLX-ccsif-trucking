using UnityEngine;
using UnityEngine.Events;

namespace VARLab.CCSIF
{
    public class EngineAudioController : AudioController
    {
        private AudioSource audioSource;
        [SerializeField] private AudioSO audioClip;
        public UnityEvent OnPlayAudio;
        public UnityEvent OnPauseAudio;

        void Start()
        {
            if (OnPlayAudio == null) { OnPlayAudio = new UnityEvent(); }
            if (OnPauseAudio == null) { OnPauseAudio = new UnityEvent(); }
            if (!TryGetComponent<AudioSource>(out audioSource))
            {
                Debug.LogWarning("Warning: EngineAudioController is not attached to an instance of AudioSource");
                return;
            }
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

        //This command "Loads" the settings present in AudioSO into the AudioSource Controller
        //Can be expanded on as needed, and should match everything present in AudioSO
        protected override void loadOptions()
        {
            audioSource.loop = audioClip.Loop;
            audioSource.clip = audioClip.clip;
        }
    }
}