using UnityEngine;

namespace VARLab.CCSIF
{
    [CreateAssetMenu(fileName = "BasicDialogSegmentSO", menuName = "ScriptableObjects/Dialogs/BasicDialogSegmentSO")]
    public class BasicDialogSegmentSO : DialogSegmentSO
    {
        public string DialogMessage;
        public bool AlignLeft;
    }
}
