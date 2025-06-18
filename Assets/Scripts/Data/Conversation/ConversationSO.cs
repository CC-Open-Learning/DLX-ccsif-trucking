using System.Collections.Generic;
using UnityEngine;

namespace VARLab.CCSIF
{
    [CreateAssetMenu(fileName = "ConversationSO", menuName = "ScriptableObjects/Dialogs/ConversationSO")]
    public class ConversationSO : ScriptableObject
    {
        public List<DialogSegmentSO> DialogSegments;
    }
}
