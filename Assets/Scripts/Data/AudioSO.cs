using UnityEngine;

namespace VARLab.CCSIF
{
    [CreateAssetMenu(fileName = "AudioSO", menuName = "ScriptableObjects/AudioSO")]
    public class AudioSO : ScriptableObject
    {
        public AudioClip clip;
        public bool Loop;
    }
}
