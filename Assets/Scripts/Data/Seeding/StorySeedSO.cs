using System.Collections.Generic;
using UnityEngine;

namespace VARLab.CCSIF
{

    /// <summary>
    /// Holds the inspection seeding data used to decided with inspections 
    /// start broken at the beginning of a module B inspection
    /// </summary>
    [CreateAssetMenu(fileName = "StorySeedSO", menuName = "ScriptableObjects/Seeding/StorySeedSO")]
    public class StorySeedSO : ScriptableObject
    {
        public List<BrokenStateSelectorSO> BrokenStateSelectorsOne;

        public List<BrokenStateSelectorSO> BrokenStateSelectorsTwo;

        public List<LinkedInspectionsSO> LinkedInspections;

        /// <summary>
        /// A list of broken states that will be set active in the second inspection if the player makes a bad road choice.
        /// </summary>
        public List<BrokenStateSelectorSO> BadRoadChoiceConsequences;

        public bool ResetPlaythrough;

        public int PlayThroughCount;
    }
}
