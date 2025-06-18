using UnityEngine;

namespace VARLab.CCSIF
{
    [CreateAssetMenu(fileName = "RoadChoiceResultsSO", menuName = "ScriptableObjects/RoadChoiceResultsSO")]
    public class RoadChoiceResultsSO : ScriptableObject
    {
        public string Title;
        public string Subtitle;
        public Texture RoadChoiceIcon;
        public string DamagedPartsList;
    }
}
