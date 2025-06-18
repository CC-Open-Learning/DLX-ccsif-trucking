using System.Collections.Generic;
using UnityEngine;

namespace VARLab.CCSIF
{
    [CreateAssetMenu(fileName = "BranchingDialogSegmentSO", menuName = "ScriptableObjects/Dialogs/BranchingDialogSegmentSO")]
    public class BranchingDialogSegmentSO : DialogSegmentSO
    {
        public List<string> PlayerResponses;
        public List<string> ManagerReplies;
    }
}
