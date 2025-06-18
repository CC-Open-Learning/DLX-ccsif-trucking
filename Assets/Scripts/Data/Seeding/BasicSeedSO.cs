using System.Collections.Generic;
using UnityEngine;

namespace VARLab.CCSIF
{

    /// <summary>
    /// Holds the inspection seeding data used to decided with inspections 
    /// start broken at the beginning of a module A inspection
    /// </summary>
    [CreateAssetMenu(fileName = "BasicSeedSO", menuName = "ScriptableObjects/Seeding/BasicSeedSO")]
    public class BasicSeedSO : ScriptableObject
    {
        // A list of the broken state groups that will be used for the seed
        public List<BrokenStateSelectorSO> BrokenStateSelectors;
    }
}