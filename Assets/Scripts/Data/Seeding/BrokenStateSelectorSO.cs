using System.Collections.Generic;
using UnityEngine;

namespace VARLab.CCSIF
{
    /// <summary>
    /// A scriptable object to store a group of broken states and how many to randomly select for a module seed 
    /// </summary>
    [CreateAssetMenu(fileName = "BrokenStateSelectorSO", menuName = "ScriptableObjects/Seeding/BrokenStateSelectorSO")]
    public class BrokenStateSelectorSO : ScriptableObject
    {
        public int SelectCount;
        public List<BrokenStateSO> BrokenStates;
    }
}
