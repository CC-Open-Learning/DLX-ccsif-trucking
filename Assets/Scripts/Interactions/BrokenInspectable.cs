using UnityEngine;

namespace VARLab.CCSIF
{
    /// <summary>
    /// This is added as a component to each GameObject that represents a broken state of an inspectable.
    /// </summary>
    public class BrokenInspectable : MonoBehaviour
    {
        public BrokenStateSO BrokenStateSO;
        [HideInInspector] public bool HasSeparatedDamage;
    }
}
