using UnityEngine;

namespace VARLab.CCSIF
{
    //Describes abstract class base for all AudioControllers
    //currently is inherited by: Effects, Weather, and Engine AudioControllers
    public abstract class AudioController: MonoBehaviour
    {
        //All derived classes use this method to invoke their respective pause events
        public abstract void PauseAudio();

        //While all derived classes use a form of "Play Audio", the methods used vary, with some having multiple methods to play audio
        //ie, PlayUIClickAudio, and PlayInteractionAudio in EffectsAudioController

        //All derived classes use this to "load" the audioClip.
        //Implementation depends on number of audio clips, ie, list vs single 
        protected abstract void loadOptions();
    }
}
